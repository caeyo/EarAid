using Celeste.Mod.EarAid.Module;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.UI;

public class EarAidGroupManageUI : EarAidOverlayUI
{
    private enum UiState
    {
        List,
        Detail,
        DeleteConfirm,
        Rename
    }

    private const int MaxNameLength = 48;

    private UiState state = UiState.List;
    private UiState previousState = (UiState)(-1);
    private UiState deleteReturnState = UiState.List;

    private int listIndex;
    private int listScroll;
    private int eventListIndex;
    private int eventListScroll;
    private readonly List<string[]> soundDisplayLines = new();
    private float[] soundItemHeights = Array.Empty<float>();
    private readonly List<string[]> eventDisplayLines = new();
    private float[] eventItemHeights = Array.Empty<float>();

    private SoundGroup selectedGroup;
    private SoundGroup pendingDeleteGroup;
    private string renameText = "";
    private bool inputSuspended;
    private bool renameTyping;

    private string cachedListTitle;
    private string cachedListHints;
    private string cachedEmptyHint;
    private string cachedDetailHints;
    private string cachedDeleteConfirmPrefix;
    private string cachedDeleteHints;
    private string cachedRenameTitle;
    private string cachedRenameHints;

    public EarAidGroupManageUI(TextMenu parentMenu) : base(parentMenu) { }

    protected override bool IsAcceptingTextInput => renameTyping;

    private int ListItemCount => EarAidModule.Settings.SoundGroups.Count;

    private SoundGroup GetSelectedGroup() => EarAidModule.Settings.SoundGroups[listIndex];

    protected override void OnOpen()
    {
        CacheDialogStrings();
        RebuildSoundDisplayLines();
        ClampListIndex();
    }

    protected override void OnClosing() => StopRenameTyping();

    public override void Update()
    {
        base.Update();

        if (inputSuspended)
        {
            listInput.Reset();
            return;
        }

        parentMenu.Focused = false;
        ConsumeVirtualMenuInput();
        ProcessInputQueue();
        EarAidAudioMute.UpdatePreview();

        if (state != previousState)
        {
            listInput.Reset();
            previousState = state;
        }

        switch (state)
        {
            case UiState.List:
                UpdateList();
                break;
            case UiState.Detail:
                UpdateDetail();
                break;
            case UiState.DeleteConfirm:
                UpdateDeleteConfirm();
                break;
            case UiState.Rename:
                UpdateRename();
                break;
        }
    }

    public override void Render()
    {
        Draw.Rect(0f, 0f, 1920f, 1080f, Color.Black);

        switch (state)
        {
            case UiState.List:
                RenderList();
                break;
            case UiState.Detail:
                RenderDetail();
                break;
            case UiState.DeleteConfirm:
                RenderDeleteConfirm();
                break;
            case UiState.Rename:
                RenderRename();
                break;
        }
    }

    private void UpdateList()
    {
        if (KeyPressed(Keys.Escape))
        {
            Close();
            return;
        }

        listInput.UpdateVertical(MoveListSelection);

        if (KeyPressed(Keys.Enter) && ListItemCount > 0)
        {
            OpenDetail(GetSelectedGroup());
        }
        else if (KeyPressed(Keys.A))
        {
            OpenSearch(new EarAidEventSearchUI(parentMenu), () =>
            {
                RebuildSoundDisplayLines();
                ClampListIndex();
            });
        }
        else if (KeyPressed(Keys.D) && ListItemCount > 0)
        {
            OpenDeleteConfirm(GetSelectedGroup(), UiState.List);
        }
    }

    private void UpdateDetail()
    {
        if (selectedGroup == null)
        {
            state = UiState.List;
            return;
        }

        if (KeyPressed(Keys.Escape))
        {
            EarAidAudioMute.StopPreview();
            state = UiState.List;
            selectedGroup = null;
            RebuildSoundDisplayLines();
            ClampListIndex();
            return;
        }

        listInput.UpdateVertical(MoveEventListSelection);

        if (KeyPressed(Keys.P) && selectedGroup.EventPaths.Count > 0)
        {
            EarAidAudioMute.TogglePreview(selectedGroup.EventPaths[eventListIndex]);
        }
        else if (KeyPressed(Keys.A))
        {
            OpenSearch(new EarAidEventSearchUI(parentMenu, selectedGroup), RebuildEventDisplayLines);
        }
        else if (KeyPressed(Keys.R))
        {
            renameText = selectedGroup.DisplayName;
            state = UiState.Rename;
            StartRenameTyping();
        }
        else if (KeyPressed(Keys.X) && selectedGroup.EventPaths.Count > 1)
        {
            RemoveSelectedEvent();
        }
        else if (KeyPressed(Keys.D))
        {
            OpenDeleteConfirm(selectedGroup, UiState.Detail);
        }
    }

    private void UpdateDeleteConfirm()
    {
        if (KeyPressed(Keys.Escape))
        {
            pendingDeleteGroup = null;
            state = deleteReturnState;

            if (deleteReturnState == UiState.List)
            {
                selectedGroup = null;
            }
        }
        else if (KeyPressed(Keys.Enter))
        {
            ConfirmDelete();
        }
    }

    private void UpdateRename()
    {
        if (renameTyping && KeyPressed(Keys.Escape))
        {
            StopRenameTyping();
            state = UiState.Detail;
        }
    }

    private void RenderList()
    {
        Vector2 topLeft = new(ContentLeft, ContentTop);
        List<SoundGroup> groups = EarAidModule.Settings.SoundGroups;

        ActiveFont.DrawOutline(cachedListTitle, topLeft, Vector2.Zero, Vector2.One, Color.White, 2f, Color.Black);
        ActiveFont.Draw(cachedListHints, topLeft + new Vector2(0f, HintsOffsetY), Vector2.Zero, Vector2.One * 0.8f, Color.Gray);

        Vector2 listTop = topLeft + new Vector2(ListColumnOffsetX, HintsOffsetY);
        EarAidWrappedList.Draw(
            listTop,
            soundDisplayLines,
            soundItemHeights,
            listScroll,
            ListViewportHeight,
            WrappedLineStep,
            ListRowScale,
            index => index == listIndex ? Color.Yellow : Color.White);

        if (groups.Count == 0)
        {
            ActiveFont.DrawOutline(cachedEmptyHint, listTop + new Vector2(0f, WrappedLineStep), Vector2.Zero, Vector2.One, Color.DarkSlateGray, 2f, Color.Black);
        }
    }

    private void RenderDetail()
    {
        Vector2 topLeft = new(ContentLeft, ContentTop);

        ActiveFont.DrawOutline(selectedGroup.DisplayName, topLeft, Vector2.Zero, Vector2.One * 1.1f, Color.White, 2f, Color.Black);
        ActiveFont.Draw(cachedDetailHints, topLeft + new Vector2(0f, HintsOffsetY), Vector2.Zero, Vector2.One * 0.8f, Color.Gray);

        Vector2 listTop = topLeft + new Vector2(ListColumnOffsetX, HintsOffsetY);
        EarAidWrappedList.Draw(
            listTop,
            eventDisplayLines,
            eventItemHeights,
            eventListScroll,
            ListViewportHeight,
            WrappedLineStep,
            ListRowScale,
            index => index == eventListIndex ? Color.Yellow : Color.White);
    }

    private void RenderDeleteConfirm()
    {
        Vector2 center = new(960f, 400f);
        string groupName = pendingDeleteGroup?.DisplayName ?? "";
        string message = $"{cachedDeleteConfirmPrefix} \"{groupName}\"?";

        ActiveFont.DrawOutline(message, center - new Vector2(0f, 40f), new Vector2(0.5f, 0f), Vector2.One, Color.White, 2f, Color.Black);
        ActiveFont.Draw(cachedDeleteHints, center + new Vector2(0f, 40f), new Vector2(0.5f, 0f), Vector2.One * 0.8f, Color.Gray);
    }

    private void RenderRename()
    {
        Vector2 center = new(960f, 400f);

        ActiveFont.DrawOutline(cachedRenameTitle, center - new Vector2(0f, 80f), new Vector2(0.5f, 0f), Vector2.One, Color.White, 2f, Color.Black);
        ActiveFont.DrawOutline(renameText + "_", center, new Vector2(0.5f, 0f), Vector2.One * 1.2f, Color.Yellow, 2f, Color.Black);
        ActiveFont.Draw(cachedRenameHints, center + new Vector2(0f, 80f), new Vector2(0.5f, 0f), Vector2.One * 0.8f, Color.Gray);
    }

    private void OpenDetail(SoundGroup group)
    {
        selectedGroup = group;
        eventListIndex = 0;
        eventListScroll = 0;
        RebuildEventDisplayLines();
        state = UiState.Detail;
    }

    private void OpenDeleteConfirm(SoundGroup group, UiState returnState)
    {
        pendingDeleteGroup = group;
        deleteReturnState = returnState;
        state = UiState.DeleteConfirm;
    }

    private void OpenSearch(EarAidEventSearchUI searchUi, Action afterClose = null)
    {
        inputSuspended = true;
        EarAidAudioMute.StopPreview();

        searchUi.OnClose = () =>
        {
            inputSuspended = false;
            parentMenu.Focused = false;
            EarAidMenu.RefreshMenu();
            afterClose?.Invoke();
        };

        Engine.Scene.Add(searchUi);
        Engine.Scene.OnEndOfFrame += () => Engine.Scene.Entities.UpdateLists();
    }

    private void ConfirmDelete()
    {
        if (pendingDeleteGroup == null)
        {
            state = deleteReturnState;
            return;
        }

        SoundGroupOperations.DeleteGroup(pendingDeleteGroup);
        pendingDeleteGroup = null;
        selectedGroup = null;
        RebuildSoundDisplayLines();
        ClampListIndex();
        state = UiState.List;
        EarAidMenu.RefreshMenu();
    }

    private void RemoveSelectedEvent()
    {
        List<string> events = selectedGroup.EventPaths;
        if (events.Count <= 1 || eventListIndex < 0 || eventListIndex >= events.Count)
        {
            return;
        }

        SoundGroupOperations.RemoveEventFromGroup(selectedGroup, events[eventListIndex]);

        int remaining = selectedGroup.EventPaths.Count;
        eventListIndex = Math.Min(eventListIndex, remaining - 1);
        RebuildEventDisplayLines();
        EarAidMenu.RefreshMenu();
    }

    private void SaveRename()
    {
        string name = renameText.Trim();
        if (name.Length == 0 || selectedGroup == null)
        {
            return;
        }

        selectedGroup.DisplayName = name;
        EarAidModule.Instance.SaveSettings();
        EarAidMenu.RefreshMenu();
        StopRenameTyping();
        state = UiState.Detail;
    }

    private void MoveListSelection(int delta)
    {
        int count = ListItemCount;
        if (count == 0)
        {
            return;
        }

        listIndex = (listIndex + delta + count) % count;
        EarAidWrappedList.EnsureIndexVisible(ref listScroll, listIndex, soundItemHeights, ListViewportHeight, WrappedLineStep);
    }

    private void MoveEventListSelection(int delta)
    {
        int count = selectedGroup?.EventPaths.Count ?? 0;
        if (count <= 1)
        {
            return;
        }

        eventListIndex = (eventListIndex + delta + count) % count;
        EarAidWrappedList.EnsureIndexVisible(ref eventListScroll, eventListIndex, eventItemHeights, ListViewportHeight, WrappedLineStep);
    }

    private void RebuildEventDisplayLines()
    {
        eventDisplayLines.Clear();
        if (selectedGroup == null)
        {
            eventItemHeights = Array.Empty<float>();
            return;
        }

        foreach (string path in selectedGroup.EventPaths)
        {
            eventDisplayLines.Add(EarAidText.WrapToLines(FormatEventPathForDisplay(path), ListMaxWidth, ListRowScale));
        }

        eventItemHeights = EarAidWrappedList.ComputeHeights(eventDisplayLines, WrappedLineStep);
        EarAidWrappedList.EnsureIndexVisible(ref eventListScroll, eventListIndex, eventItemHeights, ListViewportHeight, WrappedLineStep);
    }

    private void ClampListIndex()
    {
        int count = ListItemCount;
        if (count == 0)
        {
            listIndex = 0;
            listScroll = 0;
            return;
        }

        listIndex = Math.Min(listIndex, count - 1);
        EarAidWrappedList.EnsureIndexVisible(ref listScroll, listIndex, soundItemHeights, ListViewportHeight, WrappedLineStep);
    }

    private void RebuildSoundDisplayLines()
    {
        soundDisplayLines.Clear();
        foreach (SoundGroup group in EarAidModule.Settings.SoundGroups)
        {
            soundDisplayLines.Add(EarAidText.WrapToLines(group.DisplayName, ListMaxWidth, ListRowScale));
        }

        soundItemHeights = EarAidWrappedList.ComputeHeights(soundDisplayLines, WrappedLineStep);
    }

    protected override void HandleTextInput(char c)
    {
        if (c is '\r' or '\n')
        {
            SaveRename();
            return;
        }

        if (c == (char)8)
        {
            if (renameText.Length > 0)
            {
                renameText = renameText[..^1];
            }

            return;
        }

        if (!char.IsControl(c) && renameText.Length < MaxNameLength && ActiveFont.FontSize.Characters.ContainsKey(c))
        {
            renameText += c;
        }
    }

    private void StartRenameTyping()
    {
        renameTyping = true;
        HookTextInput();
    }

    private void StopRenameTyping()
    {
        if (!renameTyping)
        {
            return;
        }

        renameTyping = false;
        ClearInputQueue();
        UnhookTextInputIfIdle();
    }

    private void CacheDialogStrings()
    {
        cachedListTitle = Dialog.Clean("EAR_AID_MANAGE_TITLE");
        cachedListHints = Dialog.Clean("EAR_AID_MANAGE_LIST_HINTS");
        cachedEmptyHint = Dialog.Clean("EAR_AID_MANAGE_EMPTY");
        cachedDetailHints = Dialog.Clean("EAR_AID_MANAGE_DETAIL_HINTS");
        cachedDeleteConfirmPrefix = Dialog.Clean("EAR_AID_MANAGE_DELETE_CONFIRM");
        cachedDeleteHints = Dialog.Clean("EAR_AID_MANAGE_DELETE_HINTS");
        cachedRenameTitle = Dialog.Clean("EAR_AID_MANAGE_RENAME_TITLE");
        cachedRenameHints = Dialog.Clean("EAR_AID_MANAGE_RENAME_HINTS");
    }

    private void Close()
    {
        EarAidAudioMute.StopPreview();
        CloseOverlay();
    }
}
