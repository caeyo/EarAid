using Celeste.Mod.EarAid.Control;
using FMOD;
using FMOD.Studio;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.UI;

internal static class AudioMute
{
    private static int muteDepth;
    private static readonly Dictionary<EventInstance, float> silencedBackgroundAudio = new(64);

    private static EventInstance previewInstance;
    private static string previewPath;

    public static void PushMute()
    {
        if (muteDepth++ == 0)
        {
            SilenceAllEvents();
        }
    }

    public static void PopMute()
    {
        if (muteDepth <= 0)
        {
            return;
        }

        if (--muteDepth == 0)
        {
            StopPreview();
            RestoreAllEvents();
        }
    }

    public static void UpdatePreview()
    {
        if (previewInstance != null
            && previewInstance.getPlaybackState(out PLAYBACK_STATE playbackState) == RESULT.OK
            && playbackState == PLAYBACK_STATE.STOPPED)
        {
            StopPreview();
        }
    }

    public static void TogglePreview(string path)
    {
        if (IsPreviewPlaying() && previewPath == path)
        {
            StopPreview();
            return;
        }

        if (!Events.IsEventAvailable(path))
        {
            return;
        }

        StopPreview();
        previewInstance = Audio.Play(path);
        previewPath = path;
    }

    public static void StopPreview()
    {
        Audio.Stop(previewInstance, false);
        previewInstance = null;
        previewPath = null;
    }

    private static bool IsPreviewPlaying() => Audio.IsPlaying(previewInstance);

    private static void SilenceAllEvents()
    {
        silencedBackgroundAudio.Clear();

        foreach (string path in Events.SortedKnownPaths)
        {
            EventDescription evt = Audio.GetEventDescription(path);
            if (evt == null
                || evt.getInstanceList(out EventInstance[] instances) != RESULT.OK
                || instances.Length == 0)
            {
                continue;
            }

            foreach (EventInstance instance in instances)
            {
                if (instance.isValid()
                    && instance.getVolume(out float volume, out _) == RESULT.OK
                    && volume > 0f)
                {
                    silencedBackgroundAudio[instance] = volume;
                    instance.setVolume(0f);
                }
            }
        }
    }

    private static void RestoreAllEvents()
    {
        foreach (KeyValuePair<EventInstance, float> kvp in silencedBackgroundAudio)
        {
            EventInstance instance = kvp.Key;
            if (instance.isValid())
            {
                instance.setVolume(kvp.Value);
            }
        }

        silencedBackgroundAudio.Clear();
    }
}
