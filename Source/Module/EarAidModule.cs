using Celeste.Mod.EarAid.Module.Migration;
using FMOD.Studio;
using System;

namespace Celeste.Mod.EarAid.Module;

public class EarAidModule : EverestModule
{
    public static EarAidModule Instance;

    public override Type SettingsType => typeof(EarAidSettings);
    public static EarAidSettings Settings => Instance._Settings as EarAidSettings;

    public static bool Loaded { get; private set; }

    private static bool gameLoadEventsScheduled;

    public EarAidModule()
    {
        Instance = this;
    }

    public override void Load()
    {
        bool migratedSettings = V15Migration.MigrateIfNeeded(Settings);
        if (migratedSettings)
        {
            SaveSettings();
        }

        ScheduleGameLoadEvents();

        if (!Loaded && Settings.Enabled)
        {
            Mixer.LoadHooks();
            Loaded = true;
        }
    }

    public override void Unload()
    {
        if (Loaded)
        {
            Mixer.UnloadHooks();
            Loaded = false;
        }
    }

    public override void CreateModMenuSection(TextMenu menu, bool inGame, EventInstance snapshot)
    {
        CreateModMenuSectionHeader(menu, inGame, snapshot);
        EarAidMenu.CreateMenu(menu, inGame);
    }

    private static void ScheduleGameLoadEvents()
    {
        if (gameLoadEventsScheduled)
        {
            return;
        }

        // Audio.Init runs on the GameLoader background thread, after module Load/Initialize.
        Everest.Events.GameLoader.OnLoadThread += GameLoadEvents;
        gameLoadEventsScheduled = true;
    }

    private static void GameLoadEvents()
    {
        Events.RebuildRegistry(Settings.SoundGroups);

        if (Settings.Enabled)
        {
            Mixer.MixAllRegisteredInstances();
        }
    }
}
