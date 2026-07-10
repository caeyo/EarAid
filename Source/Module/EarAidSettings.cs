using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Module;

[SettingName("EAR_AID")]
public class EarAidSettings : EverestModuleSettings
{
    public const int CurrentSchemaVersion = 2;

    public bool Enabled { get; set; } = true;
    public int SchemaVersion { get; set; } = 0;
    public List<SoundGroup> SoundGroups { get; set; } = new();

    // Legacy 1.5 properties — kept so existing YAML deserializes during migration.
    [SettingIgnore] public string DEPRECATED_SETTINGS_BELOW { get; set; } = "";
    [SettingIgnore] public int BirdSquawk { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int BrokenWindow { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int Conveyor { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int CoreBlock { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int Death { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int Respawn { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int GoldenDeath { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int Dialogue { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int DreamBlock { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int DrumSwapBlock { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int FireballIdle { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int HeartCollect { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int ItemCrystalDeath { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int KevinBlock { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int LavaBarrier { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int LightningAmbience { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int LightningStrike { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int MoveBlock { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int OshiroBoss { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int Pico8Flag { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int Seeker { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int Spring { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int TouchSwitchComplete { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int FarewellWind { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int RidgeWind { get; set; } = VolumeConstants.DefaultVolume;
    [SettingIgnore] public int ZipMover { get; set; } = VolumeConstants.DefaultVolume;
}
