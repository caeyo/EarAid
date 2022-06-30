﻿using Celeste.Mod.EarAid.Module;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Utils;

public static class EventConsts
{
    public const string Death = "event:/char/madeline/death";
    public const string PreDeath = "event:/char/madeline/predeath";
    public const string GoldenDeath = "event:/new_content/char/madeline/death_golden";
    public const string DreamBlockEnter = "event:/char/madeline/dreamblock_enter";
    public const string DreamBlockExit = "event:/char/madeline/dreamblock_exit";
    public const string DreamBlockTravel = "event:/char/madeline/dreamblock_travel";
    public const string FireballIdle = "event:/env/local/09_core/fireballs_idle";
    public const string BlueHeartCollect = "event:/game/general/crystalheart_blue_get";
    public const string RedHeartCollect = "event:/game/general/crystalheart_red_get";
    public const string GoldHeartCollect = "event:/game/general/crystalheart_gold_get";
    public const string LightningStrike = "event:/new_content/game/10_farewell/lightning_strike";
    public const string FarewellWind = "event:/new_content/env/10_voidspiral";
    public const string RidgeWind = "event:/env/amb/04_main";
    public const string CityZipMover = "event:/game/01_forsaken_city/zip_mover";
    public const string FarewellZipMover = "event:/new_content/game/10_farewell/zip_mover";

    public static readonly HashSet<string> Paths = new()
    {
        Death,
        PreDeath,
        GoldenDeath,
        DreamBlockEnter,
        DreamBlockExit,
        DreamBlockTravel,
        FireballIdle,
        BlueHeartCollect,
        RedHeartCollect,
        GoldHeartCollect,
        LightningStrike,
        FarewellWind,
        RidgeWind,
        CityZipMover,
        FarewellZipMover
    };

    public static int PathToSetting(string path) => path switch
    {
        Death or PreDeath => EarAidModule.Settings.Death,
        GoldenDeath => EarAidModule.Settings.GoldenDeath,
        DreamBlockEnter or DreamBlockExit or DreamBlockTravel => EarAidModule.Settings.DreamBlock,
        FireballIdle => EarAidModule.Settings.FireballIdle,
        BlueHeartCollect or RedHeartCollect or GoldHeartCollect => EarAidModule.Settings.HeartCollect,
        LightningStrike => EarAidModule.Settings.LightningStrike,
        FarewellWind => EarAidModule.Settings.FarewellWind,
        RidgeWind => EarAidModule.Settings.RidgeWind,
        CityZipMover or FarewellZipMover => EarAidModule.Settings.ZipMover,
        _ => 10
    };
}
