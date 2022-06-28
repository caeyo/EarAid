using Celeste.Mod.EarAid.Module;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Utils;

public static class EventConsts
{
    public const string RidgeWind = "event:/env/amb/04_main";
    public const string FarewellWind = "event:/new_content/env/10_voidspiral";
    public const string GoldenDeath = "event:/new_content/char/madeline/death_golden";
    public const string LightningStrike = "event:/new_content/game/10_farewell/lightning_strike";
    public const string FireballIdle = "event:/env/local/09_core/fireballs_idle";

    public static readonly HashSet<string> Paths = new()
    {
        RidgeWind,
        FarewellWind,
        GoldenDeath,
        LightningStrike,
        FireballIdle
    };

    public static int PathToSetting(string path) => path switch
    {
        RidgeWind => EarAidModule.Settings.RidgeWind,
        FarewellWind => EarAidModule.Settings.FarewellWind,
        GoldenDeath => EarAidModule.Settings.GoldenDeath,
        LightningStrike => EarAidModule.Settings.LightningStrike,
        FireballIdle => EarAidModule.Settings.FireballIdle,
        _ => 10
    };
}
