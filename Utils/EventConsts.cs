using Celeste.Mod.EarAid.Module;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Utils;

public static class EventConsts
{
    public const string ConveyorActivate = "event:/game/09_core/conveyor_activate";
    public const string ConveyorIdle = "event:/env/local/09_core/conveyor_idle";

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

    public const string ItemCrystalDeath = "event:/cherryhelper/itemcrystal_death";

    public const string LightningStrike = "event:/new_content/game/10_farewell/lightning_strike";

    public const string MoveBlockActivate = "event:/game/04_cliffside/arrowblock_activate";
    public const string MoveBlockBreak = "event:/game/04_cliffside/arrowblock_break";
    public const string MoveBlockMove = "event:/game/04_cliffside/arrowblock_move";
    public const string MoveBlockReappear = "event:/game/04_cliffside/arrowblock_reappear";
    public const string MoveBlockReform = "event:/game/04_cliffside/arrowblock_reform_begin";
    public const string MoveBlockDepress = "event:/game/04_cliffside/arrowblock_side_depress";
    public const string MoveBlockRelease = "event:/game/04_cliffside/arrowblock_side_release";
    public const string MoveBlockMoveCommunal = "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_move";
    public const string MoveBlockBreakCommunal = "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_break";

    public const string OshiroBossCharge = "event:/char/oshiro/boss_charge";
    public const string OshiroBossEnterScreen = "event:/char/oshiro/boss_enter_screen";
    public const string OshiroBossPrecharge = "event:/char/oshiro/boss_precharge";
    public const string OshiroBossReform = "event:/char/oshiro/boss_reform";

    public const string TouchSwitchLast = "event:/game/general/touchswitch_last";
    public const string TouchSwitchLastCutoff = "event:/game/general/touchswitch_last_cutoff";
    public const string TouchSwitchLastOneshot = "event:/game/general/touchswitch_last_oneshot";

    public const string FarewellWind = "event:/new_content/env/10_voidspiral";

    public const string RidgeWind = "event:/env/amb/04_main";

    public const string CityZipMover = "event:/game/01_forsaken_city/zip_mover";
    public const string FarewellZipMover = "event:/new_content/game/10_farewell/zip_mover";

    public static readonly HashSet<string> Paths = new()
    {
        ConveyorActivate,
        ConveyorIdle,
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
        ItemCrystalDeath,
        LightningStrike,
        MoveBlockActivate,
        MoveBlockBreak,
        MoveBlockMove,
        MoveBlockReappear,
        MoveBlockReform,
        MoveBlockDepress,
        MoveBlockRelease,
        MoveBlockMoveCommunal,
        MoveBlockBreakCommunal,
        OshiroBossCharge,
        OshiroBossEnterScreen,
        OshiroBossPrecharge,
        OshiroBossReform,
        TouchSwitchLast,
        TouchSwitchLastCutoff,
        TouchSwitchLastOneshot,
        FarewellWind,
        RidgeWind,
        CityZipMover,
        FarewellZipMover
    };

    public static int PathToSetting(string path) => path switch
    {
        ConveyorActivate or ConveyorIdle => EarAidModule.Settings.Conveyor,
        Death or PreDeath => EarAidModule.Settings.Death,
        GoldenDeath => EarAidModule.Settings.GoldenDeath,
        DreamBlockEnter or DreamBlockExit or DreamBlockTravel => EarAidModule.Settings.DreamBlock,
        FireballIdle => EarAidModule.Settings.FireballIdle,
        BlueHeartCollect or RedHeartCollect or GoldHeartCollect => EarAidModule.Settings.HeartCollect,
        ItemCrystalDeath => EarAidModule.Settings.ItemCrystalDeath,
        LightningStrike => EarAidModule.Settings.LightningStrike,
        MoveBlockActivate or MoveBlockBreak or MoveBlockMove or MoveBlockReappear or MoveBlockReform or MoveBlockDepress or MoveBlockRelease
            or MoveBlockMoveCommunal or MoveBlockBreakCommunal => EarAidModule.Settings.MoveBlock,
        OshiroBossCharge or OshiroBossEnterScreen or OshiroBossPrecharge or OshiroBossReform => EarAidModule.Settings.OshiroBoss,
        TouchSwitchLast or TouchSwitchLastCutoff or TouchSwitchLastOneshot => EarAidModule.Settings.TouchSwitchComplete,
        FarewellWind => EarAidModule.Settings.FarewellWind,
        RidgeWind => EarAidModule.Settings.RidgeWind,
        CityZipMover or FarewellZipMover => EarAidModule.Settings.ZipMover,
        _ => 10
    };
}
