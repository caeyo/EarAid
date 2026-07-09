using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Module;

[SettingName("EAR_AID")]
public class EarAidSettings : EverestModuleSettings
{
    public const int CurrentSchemaVersion = 2;

    public bool Enabled { get; set; } = true;
    public int SchemaVersion { get; set; } = CurrentSchemaVersion;
    public List<SoundGroup> SoundGroups { get; set; } = new();

    // Legacy 1.5 properties — kept so existing YAML deserializes during migration.
    [SettingIgnore] public int BirdSquawk { get; set; } = 10;
    [SettingIgnore] public int BrokenWindow { get; set; } = 10;
    [SettingIgnore] public int Conveyor { get; set; } = 10;
    [SettingIgnore] public int CoreBlock { get; set; } = 10;
    [SettingIgnore] public int Death { get; set; } = 10;
    [SettingIgnore] public int Respawn { get; set; } = 10;
    [SettingIgnore] public int GoldenDeath { get; set; } = 10;
    [SettingIgnore] public int Dialogue { get; set; } = 10;
    [SettingIgnore] public int DreamBlock { get; set; } = 10;
    [SettingIgnore] public int DrumSwapBlock { get; set; } = 10;
    [SettingIgnore] public int FireballIdle { get; set; } = 10;
    [SettingIgnore] public int HeartCollect { get; set; } = 10;
    [SettingIgnore] public int ItemCrystalDeath { get; set; } = 10;
    [SettingIgnore] public int KevinBlock { get; set; } = 10;
    [SettingIgnore] public int LavaBarrier { get; set; } = 10;
    [SettingIgnore] public int LightningAmbience { get; set; } = 10;
    [SettingIgnore] public int LightningStrike { get; set; } = 10;
    [SettingIgnore] public int MoveBlock { get; set; } = 10;
    [SettingIgnore] public int OshiroBoss { get; set; } = 10;
    [SettingIgnore] public int Pico8Flag { get; set; } = 10;
    [SettingIgnore] public int Seeker { get; set; } = 10;
    [SettingIgnore] public int Spring { get; set; } = 10;
    [SettingIgnore] public int TouchSwitchComplete { get; set; } = 10;
    [SettingIgnore] public int FarewellWind { get; set; } = 10;
    [SettingIgnore] public int RidgeWind { get; set; } = 10;
    [SettingIgnore] public int ZipMover { get; set; } = 10;
}
