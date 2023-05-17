using Celeste.Mod.EarAid.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.EarAid.EarAid;

public static class Events
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

    public static readonly Dictionary<string, MethodInfo> PathToSettingGetter = new();
    public static readonly HashSet<string> ModdedPaths = new();

    public static void PopulateEventPaths()
    {
        foreach (PropertyInfo volSetting in EarAidSettings.VolumeSettings)
        {
            MethodInfo volSettingGetGetMethod = volSetting.GetGetMethod();

            foreach (VolumeSettingEventsAttribute attrib in volSetting.GetCustomAttributes().Cast<VolumeSettingEventsAttribute>())
            {
                foreach (string eventPath in attrib.EventPaths)
                {
                    PathToSettingGetter.Add(eventPath, volSettingGetGetMethod);
                }
                if (attrib.Modded)
                {
                    ModdedPaths.UnionWith(attrib.EventPaths);
                }
            }
        }
    }
}
