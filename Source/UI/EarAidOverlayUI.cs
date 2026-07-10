using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.UI;

/// <summary>
/// Shared behaviour for EarAid's fullscreen overlays: text-input plumbing, mute
/// lifecycle, engine-command suspension, and common input helpers. Subclasses own
/// their own state machine, rendering, and per-frame update logic.
/// </summary>
public abstract class EarAidOverlayUI : Entity
{
    protected const int VisibleListRows = 20;
    protected const float RowHeight = 36f;
    protected const string EventPrefix = "event:/";

    protected const float ContentLeft = 120f;
    protected const float ContentTop = 80f;
    protected const float HintsOffsetY = 130f;
    protected const float ListColumnOffsetX = 480f;
    protected const float ContentRight = 1800f;
    protected const float ListRowScale = 0.75f;
    protected const float ListMaxWidth = ContentRight - ContentLeft - ListColumnOffsetX;
    protected const float ListViewportBottomMargin = 50f;
    protected static float ListViewportHeight => 1080f - (ContentTop + HintsOffsetY) - ListViewportBottomMargin;
    protected static float WrappedLineStep => EarAidWrappedList.LineStep(ListRowScale);

    public Action OnClose;

    protected readonly TextMenu parentMenu;
    private protected readonly EarAidListInput listInput = new();

    private readonly Queue<char> inputQueue = new();
    private bool textInputHooked;
    private bool previousEngineCommandsEnabled;

    protected EarAidOverlayUI(TextMenu parentMenu)
    {
        this.parentMenu = parentMenu;
        Tag = Tags.HUD | Tags.PauseUpdate;
    }

    /// <summary>Whether keystrokes should currently be captured as text input.</summary>
    protected abstract bool IsAcceptingTextInput { get; }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        OnOpen();
        EarAidAudioMute.PushMute();

        parentMenu.Focused = false;
        previousEngineCommandsEnabled = Engine.Commands.Enabled;
        Engine.Commands.Enabled = false;
    }

    public override void Removed(Scene scene)
    {
        OnClosing();
        UnhookTextInput();
        EarAidAudioMute.PopMute();
        Engine.Commands.Enabled = previousEngineCommandsEnabled;
        base.Removed(scene);
    }

    /// <summary>Runs once when the overlay is added, before mute/focus is applied.</summary>
    protected abstract void OnOpen();

    /// <summary>Runs once while the overlay is being removed, before shared teardown.</summary>
    protected virtual void OnClosing() { }

    /// <summary>Consumes one buffered character while text input is active.</summary>
    protected abstract void HandleTextInput(char c);

    protected void ProcessInputQueue()
    {
        while (inputQueue.Count > 0 && IsAcceptingTextInput)
        {
            HandleTextInput(inputQueue.Dequeue());
        }
    }

    protected void HookTextInput()
    {
        if (!textInputHooked)
        {
            TextInput.OnInput += OnTextInput;
            textInputHooked = true;
        }
    }

    protected void UnhookTextInputIfIdle()
    {
        if (!IsAcceptingTextInput)
        {
            UnhookTextInput();
        }
    }

    protected void ClearInputQueue() => inputQueue.Clear();

    protected void CloseOverlay()
    {
        OnClose?.Invoke();
        RemoveSelf();
    }

    protected static bool KeyPressed(Keys key) => MInput.Keyboard.Pressed(key);

    protected static string FormatEventPathForDisplay(string path)
        => path.StartsWith(EventPrefix) ? path[EventPrefix.Length..] : path;

    protected static void ConsumeVirtualMenuInput()
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

    private void UnhookTextInput()
    {
        if (textInputHooked)
        {
            TextInput.OnInput -= OnTextInput;
            textInputHooked = false;
        }
    }

    private void OnTextInput(char c)
    {
        if (IsAcceptingTextInput)
        {
            inputQueue.Enqueue(c);
        }
    }
}
