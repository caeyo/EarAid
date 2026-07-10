namespace Celeste.Mod.EarAid;

internal static class VolumeConstants
{
    public const int DefaultVolume = 10;
    public const int MinVolume = 0;
    public const int MaxVolume = 20;
    public const float Scale = 10f;

    public static float ToFloat(int volume) => volume / Scale;
}
