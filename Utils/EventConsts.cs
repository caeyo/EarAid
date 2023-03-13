using Celeste.Mod.EarAid.Module;
using IL.Celeste;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Utils;

public static class EventConsts
{
    public const string DialogueBadeline = "event:/char/dialogue/badeline";
    public const string DialogueEx = "event:/char/dialogue/ex";
    public const string DialogueGranny = "event:/char/dialogue/granny";
    public const string DialogueMadeline = "event:/char/dialogue/madeline";
    public const string DialogueMadelineMirror = "event:/char/dialogue/madeline_mirror";
    public const string DialogueMom = "event:/char/dialogue/mom";
    public const string DialogueOshiro = "event:/char/dialogue/oshiro";
    public const string DialogueTheo = "event:/char/dialogue/theo";
    public const string DialogueTheoMirror = "event:/char/dialogue/theo_mirror";
    public const string DialogueTheoWebcam = "event:/char/dialogue/theo_webcam";
    
    public const string Cherry_ItemCrystalDeath = "event:/cherryhelper/itemcrystal_death";

    public const string Communal_MoveBlockMove = "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_move";
    public const string Communal_MoveBlockBreak = "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_break";

    public const string Communal_ZipMoverMoonStart = "event:/CommunalHelperEvents/game/zipMover/moon/start";
    public const string Communal_ZipMoverMoonImpact = "event:/CommunalHelperEvents/game/zipMover/moon/impact";
    public const string Communal_ZipMoverMoonReturn = "event:/CommunalHelperEvents/game/zipMover/moon/return";
    public const string Communal_ZipMoverMoonFinish = "event:/CommunalHelperEvents/game/zipMover/moon/finish";
    public const string Communal_ZipMoverMoonTick = "event:/CommunalHelperEvents/game/zipMover/moon/tick";
    public const string Communal_ZipMoverNormalStart = "event:/CommunalHelperEvents/game/zipMover/normal/start";
    public const string Communal_ZipMoverNormalImpact = "event:/CommunalHelperEvents/game/zipMover/normal/impact";
    public const string Communal_ZipMoverNormalReturn = "event:/CommunalHelperEvents/game/zipMover/normal/return";
    public const string Communal_ZipMoverNormalFinish = "event:/CommunalHelperEvents/game/zipMover/normal/finish";
    public const string Communal_ZipMoverNormalTick = "event:/CommunalHelperEvents/game/zipMover/normal/tick";
    public const string Communal_ZipMoverDreamReturn = "event:/CommunalHelperEvents/game/dreamZipMover/return";
    public const string Communal_ZipMoverDreamFinish = "event:/CommunalHelperEvents/game/dreamZipMover/finish";
    public const string Communal_ZipMoverDreamStart = "event:/CommunalHelperEvents/game/dreamZipMover/start";
    public const string Communal_ZipMoverDreamTick = "event:/CommunalHelperEvents/game/dreamZipMover/tick";
    public const string Communal_ZipMoverDreamImpact = "event:/CommunalHelperEvents/game/dreamZipMover/impact";

    public const string SJ_DrumSwapBlockMove = "event:/strawberry_jam_2021/game/drum_swapblock/drum_swapblock_move";
    public const string SJ_DrumSwapBlockMoveEnd = "event:/strawberry_jam_2021/game/drum_swapblock/drum_swapblock_move_end";

    public static readonly HashSet<string> Paths = new()
    {
        SFX.game_gen_bird_squawk,
        SFX.env_loc_03_brokenwindow_large_loop,
        SFX.env_loc_03_brokenwindow_small_loop,
        SFX.game_09_conveyor_activate,
        SFX.env_loc_09_conveyer_idle,
        SFX.char_mad_death,
        SFX.char_mad_predeath,
        SFX.char_mad_revive,
        SFX.char_mad_death_golden,
        DialogueBadeline, 
        DialogueEx, 
        DialogueGranny,
        DialogueMadeline,
        DialogueMadelineMirror,
        DialogueMom,
        DialogueOshiro,
        DialogueTheo,
        DialogueTheoMirror,
        DialogueTheoWebcam,
        SFX.char_mad_dreamblock_enter,
        SFX.char_mad_dreamblock_exit,
        SFX.char_mad_dreamblock_travel,
        SJ_DrumSwapBlockMove,
        SJ_DrumSwapBlockMoveEnd,
        SFX.env_loc_09_fireball_idle,
        SFX.game_gen_crystalheart_blue_get,
        SFX.game_gen_crystalheart_red_get,
        SFX.game_gen_crystalheart_gold_get,
        Cherry_ItemCrystalDeath,
        SFX.env_amb_10_electricity,
        SFX.game_10_lightning_strike,
        SFX.game_04_arrowblock_activate,
        SFX.game_04_arrowblock_break,
        SFX.game_04_arrowblock_move_loop,
        SFX.game_04_arrowblock_reappear,
        SFX.game_04_arrowblock_reform_begin,
        SFX.game_04_arrowblock_side_depress,
        SFX.game_04_arrowblock_side_release,
        Communal_MoveBlockMove,
        Communal_MoveBlockBreak,
        SFX.char_oshiro_boss_charge,
        SFX.char_oshiro_boss_enterscreen,
        SFX.char_oshiro_boss_precharge,
        SFX.char_oshiro_boss_reform,
        SFX.game_10_pico8_flag,
        SFX.game_gen_spring,
        SFX.game_gen_touchswitch_last_cutoff,
        SFX.game_gen_touchswitch_last_oneshot,
        SFX.env_amb_10_voidspiral,
        SFX.env_amb_04_main,
        SFX.game_01_zipmover,
        SFX.game_10_zip_mover,
        Communal_ZipMoverDreamFinish,
        Communal_ZipMoverDreamImpact,
        Communal_ZipMoverDreamReturn,
        Communal_ZipMoverDreamStart,
        Communal_ZipMoverDreamTick,
        Communal_ZipMoverMoonFinish,
        Communal_ZipMoverMoonImpact,
        Communal_ZipMoverMoonReturn,
        Communal_ZipMoverMoonStart,
        Communal_ZipMoverMoonTick,
        Communal_ZipMoverNormalFinish,
        Communal_ZipMoverNormalImpact,
        Communal_ZipMoverNormalReturn,
        Communal_ZipMoverNormalStart,
        Communal_ZipMoverNormalTick,
    };

    public static int PathToSetting(string path) => path switch
    {
        SFX.game_gen_bird_squawk => EarAidModule.Settings.BirdSquawk,
        SFX.env_loc_03_brokenwindow_large_loop or SFX.env_loc_03_brokenwindow_small_loop => EarAidModule.Settings.BrokenWindow,
        SFX.game_09_conveyor_activate or SFX.env_loc_09_conveyer_idle => EarAidModule.Settings.Conveyor,
        SFX.char_mad_death or SFX.char_mad_predeath => EarAidModule.Settings.Death,
        SFX.char_mad_revive => EarAidModule.Settings.Respawn,
        SFX.char_mad_death_golden => EarAidModule.Settings.GoldenDeath,
        DialogueBadeline or DialogueEx or DialogueGranny or DialogueMadeline or DialogueMadelineMirror or DialogueMom or DialogueOshiro or
            DialogueTheo or DialogueTheoMirror or DialogueTheoWebcam => EarAidModule.Settings.Dialogue,
        SFX.char_mad_dreamblock_enter or SFX.char_mad_dreamblock_exit or SFX.char_mad_dreamblock_travel => EarAidModule.Settings.DreamBlock,
        SJ_DrumSwapBlockMove or SJ_DrumSwapBlockMoveEnd => EarAidModule.Settings.DrumSwapBlock,
        SFX.env_loc_09_fireball_idle => EarAidModule.Settings.FireballIdle,
        SFX.game_gen_crystalheart_blue_get or SFX.game_gen_crystalheart_red_get or 
            SFX.game_gen_crystalheart_gold_get => EarAidModule.Settings.HeartCollect,
        Cherry_ItemCrystalDeath => EarAidModule.Settings.ItemCrystalDeath,
        SFX.env_amb_10_electricity => EarAidModule.Settings.LightningAmbience,
        SFX.game_10_lightning_strike => EarAidModule.Settings.LightningStrike,
        SFX.game_04_arrowblock_activate or SFX.game_04_arrowblock_break or SFX.game_04_arrowblock_move_loop or SFX.game_04_arrowblock_reappear or 
            SFX.game_04_arrowblock_reform_begin or SFX.game_04_arrowblock_side_depress or SFX.game_04_arrowblock_side_release or
            Communal_MoveBlockMove or Communal_MoveBlockBreak => EarAidModule.Settings.MoveBlock,
        SFX.char_oshiro_boss_charge or SFX.char_oshiro_boss_enterscreen or SFX.char_oshiro_boss_precharge or 
            SFX.char_oshiro_boss_reform => EarAidModule.Settings.OshiroBoss,
        SFX.game_10_pico8_flag => EarAidModule.Settings.Pico8Flag,
        SFX.game_gen_spring => EarAidModule.Settings.Spring,
        SFX.game_gen_touchswitch_last_cutoff or SFX.game_gen_touchswitch_last_oneshot => EarAidModule.Settings.TouchSwitchComplete,
        SFX.env_amb_10_voidspiral => EarAidModule.Settings.FarewellWind,
        SFX.env_amb_04_main => EarAidModule.Settings.RidgeWind,
        SFX.game_01_zipmover or SFX.game_10_zip_mover or Communal_ZipMoverDreamFinish or Communal_ZipMoverDreamImpact or 
            Communal_ZipMoverDreamReturn or Communal_ZipMoverDreamStart or Communal_ZipMoverDreamTick or Communal_ZipMoverMoonFinish or 
            Communal_ZipMoverMoonImpact or Communal_ZipMoverMoonReturn or Communal_ZipMoverMoonStart or Communal_ZipMoverMoonTick or 
            Communal_ZipMoverNormalFinish or Communal_ZipMoverNormalImpact or Communal_ZipMoverNormalReturn or Communal_ZipMoverNormalStart or 
            Communal_ZipMoverNormalTick => EarAidModule.Settings.ZipMover,
        _ => 10
    };
}
