using System;

namespace Celeste.Mod.EarAid.Module
{
    public class EarAidModule : EverestModule
    {
        public static bool Loaded;

        public static EarAidModule Instance;

        public override Type SettingsType => typeof(EarAidSettings);
        public static EarAidSettings Settings => Instance._Settings as EarAidSettings;

        public EarAidModule()
        {
            Instance = this;
        }

        public override void Load()
        {
            if (!Loaded && Settings.Enabled)
            {
                Mixer.Load();
                Loaded = true;
            }
        }

        public override void Unload()
        {
            if (Loaded)
            {
                Mixer.Unload();
                Loaded = false;
            }
        }

    }
}
