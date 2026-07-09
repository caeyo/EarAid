using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.EarAid.Module.Migration;

internal static class V15Migration
{
    private readonly record struct Category(string PropertyName, string DisplayName, string[] EventPaths);

    private static readonly Category[] Categories =
    {
        new("BirdSquawk", "Bird Squawk", new[] { SFX.game_gen_bird_squawk }),
        new("BrokenWindow", "Broken Window", new[] { SFX.env_loc_03_brokenwindow_large_loop, SFX.env_loc_03_brokenwindow_small_loop }),
        new("Conveyor", "Conveyor", new[] { SFX.game_09_conveyor_activate, SFX.env_loc_09_conveyer_idle }),
        new("CoreBlock", "Core Block", new[]
        {
            SFX.game_09_bounceblock_break, SFX.game_09_bounceblock_reappear, SFX.game_09_bounceblock_touch,
            SFX.game_09_iceblock_reappear, SFX.game_09_iceblock_touch
        }),
        new("Death", "Death", new[] { SFX.char_mad_death, SFX.char_mad_predeath }),
        new("Respawn", "Respawn", new[] { SFX.char_mad_revive }),
        new("GoldenDeath", "Golden Death", new[] { SFX.char_mad_death_golden }),
        new("Dialogue", "Dialogue", new[]
        {
            "event:/char/dialogue/badeline", "event:/char/dialogue/ex", "event:/char/dialogue/granny",
            "event:/char/dialogue/madeline", "event:/char/dialogue/madeline_mirror", "event:/char/dialogue/mom",
            "event:/char/dialogue/oshiro", "event:/char/dialogue/theo", "event:/char/dialogue/theo_mirror",
            "event:/char/dialogue/theo_webcam"
        }),
        new("DreamBlock", "Dream Block", new[] { SFX.char_mad_dreamblock_enter, SFX.char_mad_dreamblock_exit, SFX.char_mad_dreamblock_travel }),
        new("DrumSwapBlock", "Drum Swap Block", new[]
        {
            "event:/strawberry_jam_2021/game/drum_swapblock/drum_swapblock_move",
            "event:/strawberry_jam_2021/game/drum_swapblock/drum_swapblock_move_end"
        }),
        new("FireballIdle", "Fireball", new[] { SFX.env_loc_09_fireball_idle }),
        new("HeartCollect", "Heart Collect", new[] { SFX.game_gen_crystalheart_blue_get, SFX.game_gen_crystalheart_red_get, SFX.game_gen_crystalheart_gold_get }),
        new("ItemCrystalDeath", "Item Crystal Death", new[] { "event:/cherryhelper/itemcrystal_death" }),
        new("KevinBlock", "Kevin Block", new[]
        {
            SFX.game_06_crushblock_activate, SFX.game_06_crushblock_impact, SFX.game_06_crushblock_move_loop,
            SFX.game_06_crushblock_rest, SFX.game_06_crushblock_rest_waypoint, SFX.game_06_crushblock_return_loop
        }),
        new("LavaBarrier", "Lava Barrier", new[] { SFX.env_loc_09_lavagate_idle }),
        new("LightningAmbience", "Lightning Ambience", new[] { SFX.env_amb_10_electricity }),
        new("LightningStrike", "Lightning Strike", new[] { SFX.game_10_lightning_strike }),
        new("MoveBlock", "Move Block", new[]
        {
            SFX.game_04_arrowblock_activate, SFX.game_04_arrowblock_break, SFX.game_04_arrowblock_move_loop,
            SFX.game_04_arrowblock_reappear, SFX.game_04_arrowblock_reform_begin, SFX.game_04_arrowblock_side_depress,
            SFX.game_04_arrowblock_side_release,
            "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_break",
            "event:/CommunalHelperEvents/game/redirectMoveBlock/arrowblock_move"
        }),
        new("OshiroBoss", "Oshiro Boss", new[]
        {
            SFX.char_oshiro_boss_charge, SFX.char_oshiro_boss_enterscreen, SFX.char_oshiro_boss_precharge,
            SFX.char_oshiro_boss_reform
        }),
        new("Pico8Flag", "Pico-8 Flag", new[] { SFX.game_10_pico8_flag }),
        new("Seeker", "Seeker", new[]
        {
            SFX.game_05_seeker_aggro, SFX.game_05_seeker_booped, SFX.game_05_seeker_dash,
            SFX.game_05_seeker_dash_turn, SFX.game_05_seeker_death, SFX.game_05_seeker_impact_lightwall,
            SFX.game_05_seeker_impact_normal, SFX.game_05_seeker_revive, SFX.game_05_seeker_statuebreak
        }),
        new("Spring", "Spring", new[] { SFX.game_gen_spring }),
        new("TouchSwitchComplete", "Touch Switch Complete", new[] { SFX.game_gen_touchswitch_last_cutoff, SFX.game_gen_touchswitch_last_oneshot }),
        new("FarewellWind", "Farewell Wind", new[] { SFX.env_amb_10_voidspiral }),
        new("RidgeWind", "Ridge Wind", new[] { SFX.env_amb_04_main }),
        new("ZipMover", "Zip Mover", new[]
        {
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
        }),
    };

    public static bool MigrateIfNeeded(EarAidSettings settings)
    {
        if (settings.SchemaVersion >= EarAidSettings.CurrentSchemaVersion)
        {
            return false;
        }

        HashSet<string> claimedPaths = new();
        Type settingsType = typeof(EarAidSettings);

        foreach (Category category in Categories)
        {
            PropertyInfo property = settingsType.GetProperty(category.PropertyName);
            if (property?.GetValue(settings) is not int volume || volume == 10)
            {
                continue;
            }

            List<string> eventPaths = new();
            foreach (string path in category.EventPaths)
            {
                if (claimedPaths.Add(path))
                {
                    eventPaths.Add(path);
                }
            }

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

            property.SetValue(settings, 10);
        }

        settings.SchemaVersion = EarAidSettings.CurrentSchemaVersion;
        return true;
    }
}
