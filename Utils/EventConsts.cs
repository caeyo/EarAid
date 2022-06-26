using Celeste.Mod.EarAid.Module;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Utils
{
    public static class EventConsts
    {
        public const string RidgeWind = "event:/env/amb/04_main";
        public const string FarewellWind = "event:/new_content/env/10_voidspiral";
        public const string LightningStrike = "event:/new_content/game/10_farewell/lightning_strike";
        public const string GoldenDeath = "event:/new_content/char/madeline/death_golden";

        public static readonly HashSet<string> Paths = new()
        {
            RidgeWind,
            FarewellWind,
            LightningStrike,
            GoldenDeath
        };

        public static int PathToSetting(string path) => path switch
        {
            RidgeWind => EarAidModule.Settings.RidgeWind,
            FarewellWind => EarAidModule.Settings.FarewellWind,
            LightningStrike => EarAidModule.Settings.LightningStrike,
            GoldenDeath => EarAidModule.Settings.GoldenDeath,
            _ => 10
        };
    }
}
