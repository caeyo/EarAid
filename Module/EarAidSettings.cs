using Celeste.Mod.EarAid.EarAid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;

namespace Celeste.Mod.EarAid.Module;

[SettingName("EAR_AID")]
public class EarAidSettings : EverestModuleSettings
{
    public bool Enabled { get; set; } = true;
    public bool HideUnusedOptions { get; set; } = false;

    [VolumeSettingEvents(SFX.game_gen_bird_squawk)]
    public int BirdSquawk { get; set; } = 10;

    [VolumeSettingEvents(new string[] { SFX.env_loc_03_brokenwindow_large_loop, SFX.env_loc_03_brokenwindow_small_loop })]
    public int BrokenWindow { get; set; } = 10;

    [VolumeSettingEvents(new string[] { SFX.game_09_conveyor_activate, SFX.env_loc_09_conveyer_idle })]
    public int Conveyor { get; set; } = 10;

    [VolumeSettingEvents(new string[] { SFX.char_mad_death, SFX.char_mad_predeath })]
    public int Death { get; set; } = 10;

    [VolumeSettingEvents(SFX.char_mad_revive)]
    public int Respawn { get; set; } = 10;

    [VolumeSettingEvents(SFX.char_mad_death_golden)]
    public int GoldenDeath { get; set; } = 10;

    [VolumeSettingEvents(new string[] { Events.DialogueBadeline, Events.DialogueEx, Events.DialogueGranny, 
        Events.DialogueMadeline, Events.DialogueMadelineMirror, Events.DialogueMom, Events.DialogueOshiro, 
        Events.DialogueTheo, Events.DialogueTheoMirror, Events.DialogueTheoWebcam })]
    public int Dialogue { get; set; } = 10;

    [VolumeSettingEvents(new string[] { SFX.char_mad_dreamblock_enter, SFX.char_mad_dreamblock_exit, SFX.char_mad_dreamblock_travel })]
    public int DreamBlock { get; set; } = 10;

    [VolumeSettingEvents(new string[] { Events.SJ_DrumSwapBlockMove, Events.SJ_DrumSwapBlockMoveEnd }, modded: true)]
    public int DrumSwapBlock { get; set; } = 10;

    [VolumeSettingEvents(SFX.env_loc_09_fireball_idle)]
    public int FireballIdle { get; set; } = 10;

    [VolumeSettingEvents(new string[] { SFX.game_gen_crystalheart_blue_get, SFX.game_gen_crystalheart_red_get, 
        SFX.game_gen_crystalheart_gold_get })]
    public int HeartCollect { get; set; } = 10;

    [VolumeSettingEvents(Events.Cherry_ItemCrystalDeath, modded: true)]
    public int ItemCrystalDeath { get; set; } = 10;

    [VolumeSettingEvents(SFX.env_amb_10_electricity)]
    public int LightningAmbience { get; set; } = 10;

    [VolumeSettingEvents(SFX.game_10_lightning_strike)]
    public int LightningStrike { get; set; } = 10;

    [VolumeSettingEvents(new string[] { SFX.game_04_arrowblock_activate, SFX.game_04_arrowblock_break, SFX.game_04_arrowblock_move_loop,
        SFX.game_04_arrowblock_reappear, SFX.game_04_arrowblock_reform_begin, SFX.game_04_arrowblock_side_depress, 
        SFX.game_04_arrowblock_side_release })]
    [VolumeSettingEvents(new string[] { Events.Communal_MoveBlockBreak, Events.Communal_MoveBlockMove }, modded: true)]
    public int MoveBlock { get; set; } = 10;

    [VolumeSettingEvents(new string[] { SFX.char_oshiro_boss_charge, SFX.char_oshiro_boss_enterscreen, SFX.char_oshiro_boss_precharge, 
        SFX.char_oshiro_boss_reform })]
    public int OshiroBoss { get; set; } = 10;

    [VolumeSettingEvents(SFX.game_10_pico8_flag)]
    public int Pico8Flag { get; set; } = 10;

    [VolumeSettingEvents(SFX.game_gen_spring)]
    public int Spring { get; set; } = 10;

    [VolumeSettingEvents(new string[] { SFX.game_gen_touchswitch_last_cutoff, SFX.game_gen_touchswitch_last_oneshot })]
    public int TouchSwitchComplete { get; set; } = 10;

    [VolumeSettingEvents(SFX.env_amb_10_voidspiral)]
    public int FarewellWind { get; set; } = 10;

    [VolumeSettingEvents(SFX.env_amb_04_main)]
    public int RidgeWind { get; set; } = 10;

    [VolumeSettingEvents(new string[] { SFX.game_01_zipmover, SFX.game_10_zip_mover })]
    [VolumeSettingEvents(new string[] { Events.Communal_ZipMoverDreamFinish, Events.Communal_ZipMoverDreamImpact, 
        Events.Communal_ZipMoverDreamReturn, Events.Communal_ZipMoverDreamStart, Events.Communal_ZipMoverDreamTick,
        Events.Communal_ZipMoverMoonFinish, Events.Communal_ZipMoverMoonImpact, Events.Communal_ZipMoverMoonReturn, 
        Events.Communal_ZipMoverMoonStart, Events.Communal_ZipMoverMoonTick, Events.Communal_ZipMoverNormalFinish, 
        Events.Communal_ZipMoverNormalImpact, Events.Communal_ZipMoverNormalReturn, Events.Communal_ZipMoverNormalStart, 
        Events.Communal_ZipMoverNormalTick }, modded: true)]
    public int ZipMover { get; set; } = 10;

    [YamlIgnore]
    public static IEnumerable<PropertyInfo> VolumeSettings { get; private set; }

    public static void CollectVolumeSettings()
    {
        VolumeSettings = typeof(EarAidSettings).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.PropertyType == typeof(int));
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
internal class VolumeSettingEventsAttribute : Attribute 
{
    public string[] EventPaths;
    public bool Modded;

    public VolumeSettingEventsAttribute(string[] eventPaths, bool modded = false) 
    {
        EventPaths = eventPaths; 
        Modded = modded;
    }

    public VolumeSettingEventsAttribute(string eventPath, bool modded = false)
    {
        EventPaths = new string[] { eventPath };
        Modded = modded;
    }
}
