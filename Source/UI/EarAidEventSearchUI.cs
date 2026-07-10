using Celeste.Mod.EarAid.Module;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.UI;

public class EarAidEventSearchUI : EarAidOverlayUI
{
    private enum UiState
    {
        Search,
        Naming
    }

    private const int MaxNameLength = 48;
    private const int MaxQueryLength = 64;

    private static readonly Color StagedColor = Calc.HexToColor("84FF54");
    private static readonly Color BlockedSelectionColor = Calc.HexToColor("9E9E2C");

    private readonly SoundGroup addingToGroup;
    private readonly HashSet<string> existingGroupPaths = new();

    private UiState state = UiState.Search;
    private UiState previousState = (UiState)(-1);

    private string searchQuery = "";
    private string displayName = "";
    private readonly List<string> filteredPaths = new();
    private readonly List<string[]> filteredDisplayLines = new();
    private float[] filteredItemHeights = Array.Empty<float>();
    private readonly HashSet<string> selectedPaths = new();
    private HashSet<string> assignedPaths = new();

    private int listIndex;
    private int listScroll;

    private bool searchTyping;
    private bool namingTyping;
    private bool groupSaved;

    private string cachedSearchTitle;
    private string cachedSearchHints;
    private string cachedSearchTypingHints;
    private string cachedNamingTitle;
    private string cachedNamingHints;
    private string cachedSearchQueryDisplay = "";

    public EarAidEventSearchUI(TextMenu parentMenu, SoundGroup addingToGroup = null) : base(parentMenu)
    {
        this.addingToGroup = addingToGroup;
    }

    protected override bool IsAcceptingTextInput => searchTyping || namingTyping;

    private const string SearchContinuationIndent = "  ";

    private bool IsAddEventsMode => addingToGroup != null;

    protected override void OnOpen()
    {
        CacheDialogStrings();
        assignedPaths = Events.GetAssignedEventPaths(EarAidModule.Settings.SoundGroups, addingToGroup);

        if (IsAddEventsMode)
        {
            existingGroupPaths.UnionWith(addingToGroup.EventPaths);
        }

        Refilter();
    }

    protected override void OnClosing()
    {
        StopSearchTyping();
        StopNamingTyping();
    }

    public override void Update()
    {
        base.Update();

        parentMenu.Focused = false;
        ConsumeVirtualMenuInput();
        ProcessInputQueue();
        EarAidAudioMute.UpdatePreview();

        if (state != previousState)
        {
            listInput.Reset();
            previousState = state;
        }

        if (state == UiState.Search)
        {
            UpdateSearch();
        }
        else
        {
            UpdateNaming();
        }
    }

    public override void Render()
    {
        Draw.Rect(0f, 0f, 1920f, 1080f, Color.Black);

        if (state == UiState.Search)
        {
            RenderSearch();
        }
        else
        {
            RenderNaming();
        }
    }

    private void UpdateSearch()
    {
        if (searchTyping)
        {
            listInput.Reset();

            if (KeyPressed(Keys.Enter) || KeyPressed(Keys.Escape))
            {
                StopSearchTyping();
            }

            return;
        }

        listInput.UpdateVertical(MoveListSelection);

        if (KeyPressed(Keys.R))
        {
            StartSearchTyping();
        }
        else if (KeyPressed(Keys.P))
        {
            PlayPreview();
        }
        else if (KeyPressed(Keys.A))
        {
            ToggleCurrentSelection();
        }
        else if (KeyPressed(Keys.Enter))
        {
            ConfirmSearch();
        }
        else if (KeyPressed(Keys.Escape))
        {
            Close();
        }
    }

    private void UpdateNaming()
    {
        if (namingTyping && KeyPressed(Keys.Escape))
        {
            StopNamingTyping();
            state = UiState.Search;
        }
    }

    private void ConfirmSearch()
    {
        if (IsAddEventsMode)
        {
            SaveAddedEvents();
        }
        else if (selectedPaths.Count > 0)
        {
            state = UiState.Naming;
            displayName = "";
            StartNamingTyping();
        }
    }

    private void RenderSearch()
    {
        Vector2 topLeft = new(ContentLeft, ContentTop);

        ActiveFont.DrawOutline(cachedSearchTitle, topLeft, Vector2.Zero, Vector2.One, Color.White, 2f, Color.Black);

        Color queryColor = searchTyping ? Color.Yellow : Color.White;
        ActiveFont.DrawOutline(cachedSearchQueryDisplay, topLeft + new Vector2(0f, 60f), Vector2.Zero, Vector2.One * 0.9f, queryColor, 2f, Color.Black);

        string hints = searchTyping ? cachedSearchTypingHints : cachedSearchHints;
        ActiveFont.Draw(hints, topLeft + new Vector2(0f, HintsOffsetY), Vector2.Zero, Vector2.One * 0.8f, Color.Gray);

        Vector2 listTop = topLeft + new Vector2(ListColumnOffsetX, HintsOffsetY);
        EarAidWrappedList.Draw(
            listTop,
            filteredDisplayLines,
            filteredItemHeights,
            listScroll,
            ListViewportHeight,
            WrappedLineStep,
            ListRowScale,
            index =>
            {
                string path = filteredPaths[index];
                bool blocked = assignedPaths.Contains(path) || existingGroupPaths.Contains(path);
                bool highlighted = index == listIndex && !searchTyping;

                if (highlighted && blocked)
                {
                    return BlockedSelectionColor;
                }

                if (highlighted)
                {
                    return Color.Yellow;
                }

                if (blocked)
                {
                    return Color.DarkSlateGray;
                }

                return selectedPaths.Contains(path) ? StagedColor : Color.White;
            });
    }

    private void RenderNaming()
    {
        Vector2 center = new(960f, 400f);

        ActiveFont.DrawOutline(cachedNamingTitle, center - new Vector2(0f, 80f), new Vector2(0.5f, 0f), Vector2.One, Color.White, 2f, Color.Black);
        ActiveFont.DrawOutline(displayName + "_", center, new Vector2(0.5f, 0f), Vector2.One * 1.2f, Color.Yellow, 2f, Color.Black);
        ActiveFont.Draw(cachedNamingHints, center + new Vector2(0f, 80f), new Vector2(0.5f, 0f), Vector2.One * 0.8f, Color.Gray);
    }

    protected override void HandleTextInput(char c)
    {
        if (searchTyping)
        {
            HandleSearchTextInput(c);
        }
        else if (namingTyping)
        {
            HandleNamingTextInput(c);
        }
    }

    private void HandleSearchTextInput(char c)
    {
        if (c is '\r' or '\n')
        {
            StopSearchTyping();
            return;
        }

        if (c == (char)8)
        {
            if (searchQuery.Length > 0)
            {
                searchQuery = searchQuery[..^1];
                Refilter();
            }

            return;
        }

        if (!char.IsControl(c) && searchQuery.Length < MaxQueryLength)
        {
            searchQuery += c;
            Refilter();
        }
    }

    private void HandleNamingTextInput(char c)
    {
        if (c is '\r' or '\n')
        {
            SaveGroup();
            return;
        }

        if (c == (char)8)
        {
            if (displayName.Length > 0)
            {
                displayName = displayName[..^1];
            }

            return;
        }

        if (!char.IsControl(c) && displayName.Length < MaxNameLength && ActiveFont.FontSize.Characters.ContainsKey(c))
        {
            displayName += c;
        }
    }

    private void StartSearchTyping()
    {
        if (searchTyping)
        {
            return;
        }

        searchTyping = true;
        UpdateSearchQueryDisplay();
        HookTextInput();
    }

    private void StopSearchTyping()
    {
        if (!searchTyping)
        {
            return;
        }

        searchTyping = false;
        ClearInputQueue();
        UpdateSearchQueryDisplay();
        UnhookTextInputIfIdle();
    }

    private void StartNamingTyping()
    {
        namingTyping = true;
        HookTextInput();
    }

    private void StopNamingTyping()
    {
        if (!namingTyping)
        {
            return;
        }

        namingTyping = false;
        ClearInputQueue();
        UnhookTextInputIfIdle();
    }

    private void Refilter()
    {
        string query = searchQuery.Trim();
        filteredPaths.Clear();

        foreach (string path in Events.SortedKnownPaths)
        {
            if (query.Length == 0 || path.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                filteredPaths.Add(path);
            }
        }

        listIndex = 0;
        listScroll = 0;
        RebuildFilteredDisplayLines();
        UpdateSearchQueryDisplay();
    }

    private void RebuildFilteredDisplayLines()
    {
        filteredDisplayLines.Clear();
        filteredDisplayLines.Capacity = Math.Max(filteredDisplayLines.Capacity, filteredPaths.Count);

        foreach (string path in filteredPaths)
        {
            string prefix = selectedPaths.Contains(path) ? "+ " : "  ";
            string label = prefix + FormatEventPathForDisplay(path);
            filteredDisplayLines.Add(EarAidText.WrapToLines(label, ListMaxWidth, ListRowScale, SearchContinuationIndent));
        }

        filteredItemHeights = EarAidWrappedList.ComputeHeights(filteredDisplayLines, WrappedLineStep);
        EarAidWrappedList.EnsureIndexVisible(ref listScroll, listIndex, filteredItemHeights, ListViewportHeight, WrappedLineStep);
    }

    private void UpdateSearchQueryDisplay()
    {
        cachedSearchQueryDisplay = searchQuery + (searchTyping ? "_" : "");
    }

    private void CacheDialogStrings()
    {
        cachedSearchTitle = Dialog.Clean(IsAddEventsMode ? "EAR_AID_SEARCH_ADD_TITLE" : "EAR_AID_SEARCH_TITLE");
        cachedSearchHints = Dialog.Clean(IsAddEventsMode ? "EAR_AID_SEARCH_ADD_HINTS" : "EAR_AID_SEARCH_HINTS");
        cachedSearchTypingHints = Dialog.Clean("EAR_AID_SEARCH_TYPING_HINTS");
        cachedNamingTitle = Dialog.Clean("EAR_AID_NAMING_TITLE");
        cachedNamingHints = Dialog.Clean("EAR_AID_NAMING_HINTS");
    }

    private void MoveListSelection(int delta)
    {
        if (filteredPaths.Count == 0)
        {
            return;
        }

        listIndex = (listIndex + delta + filteredPaths.Count) % filteredPaths.Count;
        EarAidWrappedList.EnsureIndexVisible(ref listScroll, listIndex, filteredItemHeights, ListViewportHeight, WrappedLineStep);
    }

    private void PlayPreview()
    {
        if (filteredPaths.Count > 0)
        {
            EarAidAudioMute.TogglePreview(filteredPaths[listIndex]);
        }
    }

    private void ToggleCurrentSelection()
    {
        if (filteredPaths.Count == 0)
        {
            return;
        }

        string path = filteredPaths[listIndex];
        if (assignedPaths.Contains(path) || existingGroupPaths.Contains(path))
        {
            return;
        }

        if (!selectedPaths.Add(path))
        {
            selectedPaths.Remove(path);
        }

        RebuildFilteredDisplayLines();
    }

    private void SaveAddedEvents()
    {
        if (groupSaved || selectedPaths.Count == 0)
        {
            return;
        }

        groupSaved = true;

        List<string> mergedPaths = new(addingToGroup.EventPaths);
        foreach (string path in selectedPaths)
        {
            if (!existingGroupPaths.Contains(path))
            {
                mergedPaths.Add(path);
            }
        }

        SoundGroupOperations.UpdateGroup(addingToGroup, addingToGroup.DisplayName, mergedPaths);
        Close();
    }

    private void SaveGroup()
    {
        if (groupSaved)
        {
            return;
        }

        string name = displayName.Trim();
        if (name.Length == 0 || selectedPaths.Count == 0)
        {
            return;
        }

        groupSaved = true;

        EarAidModule.Settings.SoundGroups.Add(new SoundGroup
        {
            DisplayName = name,
            Volume = VolumeConstants.DefaultVolume,
            EventPaths = new List<string>(selectedPaths)
        });

        Events.RebuildRegistry(EarAidModule.Settings.SoundGroups);

        if (EarAidModule.Settings.Enabled)
        {
            Mixer.MixExistingInstances(selectedPaths, VolumeConstants.DefaultVolume);
        }

        EarAidModule.Instance.SaveSettings();
        Close();
    }

    private void Close() => CloseOverlay();
}
