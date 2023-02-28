using Celeste.Mod.EarAid.Module;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Utils;

public static class EventConsts
{
    public const string ItemCrystalDeath = "event:/cherryhelper/itemcrystal_death";

    public const string MoveBlockMoveCommunal = "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_move";
    public const string MoveBlockBreakCommunal = "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_break";

    public static readonly HashSet<string> Paths = new()
    {
        SFX.game_09_conveyor_activate,
        SFX.env_loc_09_conveyer_idle,
        SFX.char_mad_death,
        SFX.char_mad_predeath,
        SFX.char_mad_death_golden,
        SFX.char_mad_dreamblock_enter,
        SFX.char_mad_dreamblock_exit,
        SFX.char_mad_dreamblock_travel,
        SFX.env_loc_09_fireball_idle,
        SFX.game_gen_crystalheart_blue_get,
        SFX.game_gen_crystalheart_red_get,
        SFX.game_gen_crystalheart_gold_get,
        ItemCrystalDeath,
        SFX.game_10_lightning_strike,
        SFX.game_04_arrowblock_activate,
        SFX.game_04_arrowblock_break,
        SFX.game_04_arrowblock_move_loop,
        SFX.game_04_arrowblock_reappear,
        SFX.game_04_arrowblock_reform_begin,
        SFX.game_04_arrowblock_side_depress,
        SFX.game_04_arrowblock_side_release,
        MoveBlockMoveCommunal,
        MoveBlockBreakCommunal,
        SFX.char_oshiro_boss_charge,
        SFX.char_oshiro_boss_enterscreen,
        SFX.char_oshiro_boss_precharge,
        SFX.char_oshiro_boss_reform,
        SFX.game_gen_touchswitch_last_cutoff,
        SFX.game_gen_touchswitch_last_oneshot,
        SFX.env_amb_10_voidspiral,
        SFX.env_amb_04_main,
        SFX.game_01_zipmover,
        SFX.game_10_zip_mover
    };

    public static int PathToSetting(string path) => path switch
    {
        SFX.game_09_conveyor_activate or SFX.env_loc_09_conveyer_idle => EarAidModule.Settings.Conveyor,
        SFX.char_mad_death or SFX.char_mad_predeath => EarAidModule.Settings.Death,
        SFX.char_mad_death_golden => EarAidModule.Settings.GoldenDeath,
        SFX.char_mad_dreamblock_enter or SFX.char_mad_dreamblock_exit or SFX.char_mad_dreamblock_travel => EarAidModule.Settings.DreamBlock,
        SFX.env_loc_09_fireball_idle => EarAidModule.Settings.FireballIdle,
        SFX.game_gen_crystalheart_blue_get or SFX.game_gen_crystalheart_red_get or 
            SFX.game_gen_crystalheart_gold_get => EarAidModule.Settings.HeartCollect,
        ItemCrystalDeath => EarAidModule.Settings.ItemCrystalDeath,
        SFX.game_10_lightning_strike => EarAidModule.Settings.LightningStrike,
        SFX.game_04_arrowblock_activate or SFX.game_04_arrowblock_break or SFX.game_04_arrowblock_move_loop or SFX.game_04_arrowblock_reappear or 
            SFX.game_04_arrowblock_reform_begin or SFX.game_04_arrowblock_side_depress or SFX.game_04_arrowblock_side_release or
            MoveBlockMoveCommunal or MoveBlockBreakCommunal => EarAidModule.Settings.MoveBlock,
        SFX.char_oshiro_boss_charge or SFX.char_oshiro_boss_enterscreen or SFX.char_oshiro_boss_precharge or 
            SFX.char_oshiro_boss_reform => EarAidModule.Settings.OshiroBoss,
        SFX.game_gen_touchswitch_last_cutoff or SFX.game_gen_touchswitch_last_oneshot => EarAidModule.Settings.TouchSwitchComplete,
        SFX.env_amb_10_voidspiral => EarAidModule.Settings.FarewellWind,
        SFX.env_amb_04_main => EarAidModule.Settings.RidgeWind,
        SFX.game_01_zipmover or SFX.game_10_zip_mover => EarAidModule.Settings.ZipMover,
        _ => 10
    };
}
