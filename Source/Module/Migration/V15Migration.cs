using Celeste.Mod.EarAid.Control;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.EarAid.Module.Migration;

internal static class V15Migration
{
    private readonly record struct Category(
        string DisplayName,
        string[] EventPaths,
        Func<EarAidSettings, int> GetVolume,
        Action<EarAidSettings, int> SetVolume);

    private static readonly Category[] Categories =
    [
        new("Bird Squawk", [SFX.game_gen_bird_squawk], s => s.BirdSquawk, (s, v) => s.BirdSquawk = v),
        new("Broken Window", [SFX.env_loc_03_brokenwindow_large_loop, SFX.env_loc_03_brokenwindow_small_loop], s => s.BrokenWindow, (s, v) => s.BrokenWindow = v),
        new("Conveyor", [SFX.game_09_conveyor_activate, SFX.env_loc_09_conveyer_idle], s => s.Conveyor, (s, v) => s.Conveyor = v),
        new("Core Block", [
            SFX.game_09_bounceblock_break, SFX.game_09_bounceblock_reappear, SFX.game_09_bounceblock_touch,
            SFX.game_09_iceblock_reappear, SFX.game_09_iceblock_touch
        ], s => s.CoreBlock, (s, v) => s.CoreBlock = v),
        new("Death", [SFX.char_mad_death, SFX.char_mad_predeath], s => s.Death, (s, v) => s.Death = v),
        new("Respawn", [SFX.char_mad_revive], s => s.Respawn, (s, v) => s.Respawn = v),
        new("Golden Death", [SFX.char_mad_death_golden], s => s.GoldenDeath, (s, v) => s.GoldenDeath = v),
        new("Dialogue", [
            "event:/char/dialogue/badeline", "event:/char/dialogue/ex", "event:/char/dialogue/granny",
            "event:/char/dialogue/madeline", "event:/char/dialogue/madeline_mirror", "event:/char/dialogue/mom",
            "event:/char/dialogue/oshiro", "event:/char/dialogue/theo", "event:/char/dialogue/theo_mirror",
            "event:/char/dialogue/theo_webcam"
        ], s => s.Dialogue, (s, v) => s.Dialogue = v),
        new("Dream Block", [SFX.char_mad_dreamblock_enter, SFX.char_mad_dreamblock_exit, SFX.char_mad_dreamblock_travel], s => s.DreamBlock, (s, v) => s.DreamBlock = v),
        new("Drum Swap Block", [
            "event:/strawberry_jam_2021/game/drum_swapblock/drum_swapblock_move",
            "event:/strawberry_jam_2021/game/drum_swapblock/drum_swapblock_move_end"
        ], s => s.DrumSwapBlock, (s, v) => s.DrumSwapBlock = v),
        new("Fireball", [SFX.env_loc_09_fireball_idle], s => s.FireballIdle, (s, v) => s.FireballIdle = v),
        new("Heart Collect", [SFX.game_gen_crystalheart_blue_get, SFX.game_gen_crystalheart_red_get, SFX.game_gen_crystalheart_gold_get], s => s.HeartCollect, (s, v) => s.HeartCollect = v),
        new("Item Crystal Death", ["event:/cherryhelper/itemcrystal_death"], s => s.ItemCrystalDeath, (s, v) => s.ItemCrystalDeath = v),
        new("Kevin Block", [
            SFX.game_06_crushblock_activate, SFX.game_06_crushblock_impact, SFX.game_06_crushblock_move_loop,
            SFX.game_06_crushblock_rest, SFX.game_06_crushblock_rest_waypoint, SFX.game_06_crushblock_return_loop
        ], s => s.KevinBlock, (s, v) => s.KevinBlock = v),
        new("Lava Barrier", [SFX.env_loc_09_lavagate_idle], s => s.LavaBarrier, (s, v) => s.LavaBarrier = v),
        new("Lightning Ambience", [SFX.env_amb_10_electricity], s => s.LightningAmbience, (s, v) => s.LightningAmbience = v),
        new("Lightning Strike", [SFX.game_10_lightning_strike], s => s.LightningStrike, (s, v) => s.LightningStrike = v),
        new("Move Block", [
            SFX.game_04_arrowblock_activate, SFX.game_04_arrowblock_break, SFX.game_04_arrowblock_move_loop,
            SFX.game_04_arrowblock_reappear, SFX.game_04_arrowblock_reform_begin, SFX.game_04_arrowblock_side_depress,
            SFX.game_04_arrowblock_side_release,
            "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_break",
            "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_move"
        ], s => s.MoveBlock, (s, v) => s.MoveBlock = v),
        new("Oshiro Boss", [
            SFX.char_oshiro_boss_charge, SFX.char_oshiro_boss_enterscreen, SFX.char_oshiro_boss_precharge,
            SFX.char_oshiro_boss_reform
        ], s => s.OshiroBoss, (s, v) => s.OshiroBoss = v),
        new("Pico-8 Flag", [SFX.game_10_pico8_flag], s => s.Pico8Flag, (s, v) => s.Pico8Flag = v),
        new("Seeker", [
            SFX.game_05_seeker_aggro, SFX.game_05_seeker_booped, SFX.game_05_seeker_dash,
            SFX.game_05_seeker_dash_turn, SFX.game_05_seeker_death, SFX.game_05_seeker_impact_lightwall,
            SFX.game_05_seeker_impact_normal, SFX.game_05_seeker_revive, SFX.game_05_seeker_statuebreak
        ], s => s.Seeker, (s, v) => s.Seeker = v),
        new("Spring", [SFX.game_gen_spring], s => s.Spring, (s, v) => s.Spring = v),
        new("Touch Switch Complete", [SFX.game_gen_touchswitch_last_cutoff, SFX.game_gen_touchswitch_last_oneshot], s => s.TouchSwitchComplete, (s, v) => s.TouchSwitchComplete = v),
        new("Farewell Wind", [SFX.env_amb_10_voidspiral], s => s.FarewellWind, (s, v) => s.FarewellWind = v),
        new("Ridge Wind", [SFX.env_amb_04_main], s => s.RidgeWind, (s, v) => s.RidgeWind = v),
        new("Zip Mover", [
            SFX.game_01_zipmover, SFX.game_10_zip_mover,
            "event:/CommunalHelperEvents/game/zipMover/moon/start",
            "event:/CommunalHelperEvents/game/zipMover/moon/impact",
            "event:/CommunalHelperEvents/game/zipMover/moon/return",
            "event:/CommunalHelperEvents/game/zipMover/moon/finish",
            "event:/CommunalHelperEvents/game/zipMover/moon/tick",
            "event:/CommunalHelperEvents/game/zipMover/normal/start",
            "event:/CommunalHelperEvents/game/zipMover/normal/impact",
            "event:/CommunalHelperEvents/game/zipMover/normal/return",
            "event:/CommunalHelperEvents/game/zipMover/normal/finish",
            "event:/CommunalHelperEvents/game/zipMover/normal/tick",
            "event:/CommunalHelperEvents/game/dreamZipMover/return",
            "event:/CommunalHelperEvents/game/dreamZipMover/finish",
            "event:/CommunalHelperEvents/game/dreamZipMover/start",
            "event:/CommunalHelperEvents/game/dreamZipMover/tick",
            "event:/CommunalHelperEvents/game/dreamZipMover/impact"
        ], s => s.ZipMover, (s, v) => s.ZipMover = v)
    ];

    public static bool MigrateIfNeeded(EarAidSettings settings)
    {
        if (settings.SchemaVersion >= EarAidSettings.CurrentSchemaVersion)
        {
            return false;
        }

        HashSet<string> claimedPaths = new();

        foreach (Category category in Categories)
        {
            int volume = category.GetVolume(settings);
            if (volume == VolumeConstants.DefaultVolume)
            {
                continue;
            }

            List<string> eventPaths = category.EventPaths.Where(claimedPaths.Add).ToList();

            if (eventPaths.Count == 0)
            {
                continue;
            }

            settings.SoundGroups.Add(new SoundGroup
            {
                DisplayName = category.DisplayName,
                Volume = volume,
                EventPaths = eventPaths
            });

            category.SetVolume(settings, VolumeConstants.DefaultVolume);
        }

        settings.SchemaVersion = EarAidSettings.CurrentSchemaVersion;
        return true;
    }
}
