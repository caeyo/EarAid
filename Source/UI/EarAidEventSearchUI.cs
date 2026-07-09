using Celeste.Mod.EarAid.EarAid;
using Celeste.Mod.EarAid.Module;
using FMOD;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.EarAid.UI;

public class EarAidEventSearchUI : Entity
{
    private enum UiState
    {
        Search,
        Naming
    }

    private const int VisibleListRows = 20;
    private const float RowHeight = 36f;

    public Action OnClose;

    private readonly TextMenu parentMenu;
    private UiState state = UiState.Search;

    private string searchQuery = "";
    private string displayName = "";
    private List<string> filteredPaths = new();
    private readonly HashSet<string> selectedPaths = new();
    private HashSet<string> assignedPaths = new();

    private int listIndex;
    private int listScroll;
    private EventInstance previewInstance;

    private Dictionary<EventInstance, float> silencedBackgroundAudio = new();
    private readonly Queue<char> inputQueue = new();

    private bool searchTyping;
    private bool namingTyping;
    private bool textInputHooked;
    private bool previousEngineCommandsEnabled;
    private bool groupSaved;

    private bool IsAcceptingTextInput => searchTyping || namingTyping;

    public EarAidEventSearchUI(TextMenu parentMenu)
    {
        this.parentMenu = parentMenu;
        Tag = Tags.HUD;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        RefreshAssignedPaths();
        Refilter();
        SilenceAllEvents();

        parentMenu.Focused = false;
        previousEngineCommandsEnabled = Engine.Commands.Enabled;
        Engine.Commands.Enabled = false;
    }

    public override void Removed(Scene scene)
    {
        StopSearchTyping();
        StopNamingTyping();
        Engine.Commands.Enabled = previousEngineCommandsEnabled;
        StopPreview();
        RestoreAllEvents();
        base.Removed(scene);
    }

    public override void Update()
    {
        base.Update();

        parentMenu.Focused = false;
        ConsumeVirtualMenuInput();

        ProcessInputQueue();

        if (previewInstance != null
            && previewInstance.getPlaybackState(out PLAYBACK_STATE playbackState) == RESULT.OK
            && playbackState == PLAYBACK_STATE.STOPPED)
        {
            StopPreview();
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
        Draw.Rect(0f, 0f, 1920f, 1080f, Color.Black * 0.85f);

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
            if (KeyPressed(Keys.Enter) || KeyPressed(Keys.Escape))
            {
                StopSearchTyping();
            }

            return;
        }

        if (KeyPressed(Keys.Up))
        {
            MoveListSelection(-1);
        }
        else if (KeyPressed(Keys.Down))
        {
            MoveListSelection(1);
        }
        else if (KeyPressed(Keys.R))
        {
            StartSearchTyping();
        }
        else if (KeyPressed(Keys.P))
        {
            PlayPreview();
        }
        else if (KeyPressed(Keys.A))
        {
            AddCurrentToSelection();
        }
        else if (KeyPressed(Keys.C) || KeyPressed(Keys.Enter))
        {
            if (selectedPaths.Count > 0)
            {
                state = UiState.Naming;
                displayName = "";
                StartNamingTyping();
            }
        }
        else if (KeyPressed(Keys.Escape))
        {
            Close();
        }
    }

    private void UpdateNaming()
    {
        if (!namingTyping)
        {
            return;
        }

        if (KeyPressed(Keys.Escape))
        {
            StopNamingTyping();
            state = UiState.Search;
        }
    }

    private static bool KeyPressed(Keys key) => MInput.Keyboard.Pressed(key);

    private static void ConsumeVirtualMenuInput()
    {
        Input.MenuUp.ConsumePress();
        Input.MenuDown.ConsumePress();
        Input.MenuLeft.ConsumePress();
        Input.MenuRight.ConsumePress();
        Input.MenuConfirm.ConsumePress();
        Input.MenuCancel.ConsumePress();
        Input.ESC.ConsumePress();
        Input.QuickRestart.ConsumePress();
        Input.Pause.ConsumePress();
    }

    private void RenderSearch()
    {
        Vector2 topLeft = new(120f, 80f);

        ActiveFont.DrawOutline(Dialog.Clean("EAR_AID_SEARCH_TITLE"), topLeft, Vector2.Zero, Vector2.One, Color.White, 2f, Color.Black);

        Color queryColor = searchTyping ? Color.Yellow : Color.White;
        ActiveFont.DrawOutline(searchQuery + (searchTyping ? "_" : ""), topLeft + new Vector2(0f, 60f), Vector2.Zero, Vector2.One * 1.1f, queryColor, 2f, Color.Black);

        string hintsKey = searchTyping ? "EAR_AID_SEARCH_TYPING_HINTS" : "EAR_AID_SEARCH_HINTS";
        ActiveFont.Draw(Dialog.Clean(hintsKey), topLeft + new Vector2(0f, 130f), Vector2.Zero, Vector2.One * 0.8f, Color.Gray);

        Vector2 listTop = topLeft + new Vector2(480f, 130f);
        bool hasAbove = listScroll > 0;
        int contentRows = GetVisibleContentRowCount();
        bool hasBelow = listScroll + contentRows < filteredPaths.Count;

        int row = 0;
        if (hasAbove)
        {
            ActiveFont.DrawOutline("  ^", listTop + new Vector2(0f, row * RowHeight), Vector2.Zero, Vector2.One * 0.75f, Color.Gray, 2f, Color.Black);
            row++;
        }

        for (int i = 0; i < contentRows; i++)
        {
            int pathIndex = listScroll + i;
            if (pathIndex >= filteredPaths.Count)
            {
                break;
            }

            string path = filteredPaths[pathIndex];
            bool highlighted = pathIndex == listIndex;
            bool staged = selectedPaths.Contains(path);
            bool assigned = assignedPaths.Contains(path);

            Color color = highlighted ? Color.Yellow
                : assigned ? Color.DarkSlateGray
                : staged ? Calc.HexToColor("84FF54")
                : Color.White;
            string prefix = staged ? "+ " : "  ";
            ActiveFont.DrawOutline(prefix + FormatEventPathForDisplay(path), listTop + new Vector2(0f, row * RowHeight), Vector2.Zero, Vector2.One * 0.75f, color, 2f, Color.Black);
            row++;
        }

        if (hasBelow)
        {
            ActiveFont.DrawOutline("  v", listTop + new Vector2(0f, row * RowHeight), Vector2.Zero, Vector2.One * 0.75f, Color.Gray, 2f, Color.Black);
        }
    }

    private void RenderNaming()
    {
        Vector2 center = new(960f, 400f);

        ActiveFont.DrawOutline(Dialog.Clean("EAR_AID_NAMING_TITLE"), center - new Vector2(0f, 80f), new Vector2(0.5f, 0f), Vector2.One, Color.White, 2f, Color.Black);
        ActiveFont.DrawOutline(displayName + "_", center, new Vector2(0.5f, 0f), Vector2.One * 1.2f, Color.Yellow, 2f, Color.Black);
        ActiveFont.Draw(Dialog.Clean("EAR_AID_NAMING_HINTS"), center + new Vector2(0f, 80f), new Vector2(0.5f, 0f), Vector2.One * 0.8f, Color.Gray);
    }

    private void ProcessInputQueue()
    {
        while (inputQueue.Count > 0 && IsAcceptingTextInput)
        {
            char c = inputQueue.Dequeue();

            if (searchTyping)
            {
                HandleSearchTextInput(c);
            }
            else if (namingTyping)
            {
                HandleNamingTextInput(c);
            }
        }
    }

    private void OnTextInput(char c)
    {
        if (IsAcceptingTextInput)
        {
            inputQueue.Enqueue(c);
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

        if (!char.IsControl(c) && searchQuery.Length < 64)
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

        if (!char.IsControl(c) && displayName.Length < 48 && ActiveFont.FontSize.Characters.ContainsKey(c))
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
        HookTextInput();
    }

    private void StopSearchTyping()
    {
        if (!searchTyping)
        {
            return;
        }

        searchTyping = false;
        inputQueue.Clear();
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
        inputQueue.Clear();
        UnhookTextInputIfIdle();
    }

    private void Refilter()
    {
        string query = searchQuery.Trim();
        List<string> allPaths = Events.GetAllKnownEventPaths().ToList();
        allPaths.Sort();

        if (query.Length == 0)
        {
            filteredPaths = allPaths;
        }
        else
        {
            filteredPaths = allPaths
                .Where(path => path.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        listIndex = Calc.Clamp(listIndex, 0, Math.Max(0, filteredPaths.Count - 1));
        UpdateListScroll();
    }

    private void MoveListSelection(int delta)
    {
        if (filteredPaths.Count == 0)
        {
            return;
        }

        listIndex = (listIndex + delta + filteredPaths.Count) % filteredPaths.Count;
        UpdateListScroll();
    }

    private int GetVisibleContentRowCount()
    {
        int contentRows = VisibleListRows;
        if (listScroll > 0)
        {
            contentRows--;
        }

        if (listScroll + contentRows < filteredPaths.Count)
        {
            contentRows--;
        }

        return Math.Max(contentRows, 1);
    }

    private void UpdateListScroll()
    {
        int contentRows = GetVisibleContentRowCount();

        if (listIndex < listScroll)
        {
            listScroll = listIndex;
        }
        else if (listIndex >= listScroll + contentRows)
        {
            listScroll = listIndex - contentRows + 1;
        }
    }

    private void PlayPreview()
    {
        if (IsPreviewPlaying())
        {
            StopPreview();
            return;
        }

        if (filteredPaths.Count == 0)
        {
            return;
        }

        string path = filteredPaths[listIndex];
        if (!Events.IsEventAvailable(path))
        {
            return;
        }

        StopPreview();
        previewInstance = Audio.Play(path);
    }

    private bool IsPreviewPlaying()
    {
        return Audio.IsPlaying(previewInstance);
    }

    private void AddCurrentToSelection()
    {
        if (filteredPaths.Count == 0)
        {
            return;
        }

        string path = filteredPaths[listIndex];
        if (assignedPaths.Contains(path))
        {
            return;
        }

        if (!selectedPaths.Add(path))
        {
            selectedPaths.Remove(path);
        }
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

        EarAidModule.Settings.SoundGroups.Add(new Module.SoundGroup
        {
            DisplayName = name,
            Volume = 10,
            EventPaths = selectedPaths.ToList()
        });

        Events.RebuildRegistry(EarAidModule.Settings.SoundGroups);
        Close();
    }

    private static string FormatEventPathForDisplay(string path)
    {
        const string eventPrefix = "event:/";
        return path.StartsWith(eventPrefix) ? path[eventPrefix.Length..] : path;
    }

    private void Close()
    {
        StopSearchTyping();
        StopNamingTyping();
        OnClose?.Invoke();
        RemoveSelf();
    }

    private void RefreshAssignedPaths()
    {
        assignedPaths = Events.GetAssignedEventPaths(EarAidModule.Settings.SoundGroups);
    }

    private void StopPreview()
    {
        Audio.Stop(previewInstance);
    }

    private void SilenceAllEvents()
    {
        silencedBackgroundAudio.Clear();
        foreach (string path in Events.GetAllKnownEventPaths())
        {
            EventDescription evt = Audio.GetEventDescription(path);
            if (evt.getInstanceList(out EventInstance[] instances) != FMOD.RESULT.OK) 
                continue;
            foreach (EventInstance instance in instances)
            {
                if (instance.isValid() && instance.getVolume(out float vol, out float finalvolume) == FMOD.RESULT.OK)
                {
                    if (vol > 0f)
                    {
                        silencedBackgroundAudio[instance] = vol;
                        instance.setVolume(0f);
                    }
                }
            }
        }
    }
    
    private void RestoreAllEvents()
    {
        foreach (var kvp in silencedBackgroundAudio)
        {
            EventInstance instance = kvp.Key;
            float originalVolume = kvp.Value;

            if (instance.isValid())
            {
                instance.setVolume(originalVolume);
            }
        }
    
        silencedBackgroundAudio.Clear();
    }

    private void HookTextInput()
    {
        if (!textInputHooked)
        {
            TextInput.OnInput += OnTextInput;
            textInputHooked = true;
        }
    }

    private void UnhookTextInputIfIdle()
    {
        if (textInputHooked && !IsAcceptingTextInput)
        {
            TextInput.OnInput -= OnTextInput;
            textInputHooked = false;
        }
    }
}
