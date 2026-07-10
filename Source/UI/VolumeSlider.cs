using Celeste.Mod.EarAid.Control;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Reflection;

namespace Celeste.Mod.EarAid.UI;

internal class VolumeSlider : TextMenuExt.IntSlider
{
    private static readonly FieldInfo intSliderSine = typeof(TextMenuExt.IntSlider).GetField("sine", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo intSliderLastDir = typeof(TextMenuExt.IntSlider).GetField("lastDir", BindingFlags.NonPublic | BindingFlags.Instance);

    private const int min = VolumeConstants.MinVolume;
    private const int max = VolumeConstants.MaxVolume;

    public VolumeSlider(string label, int value) : base(label, VolumeConstants.MinVolume, VolumeConstants.MaxVolume, value) { }

    public override float RightWidth()
    {
        return ActiveFont.Measure(max.ToString() + "0%").X + 120f;
    }

    public override void Render(Vector2 position, bool highlighted)
    {
        float sine = (float)intSliderSine.GetValue(this);
        int lastDir = (int)intSliderLastDir.GetValue(this);

        float alpha = Container.Alpha;

        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        Color color = Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : Color.White) * alpha);
        ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);

        if ((max - min) > 0)
        {
            float rWidth = RightWidth();

            ActiveFont.DrawOutline(Index.ToString() + (Index > 0 ? "0%" : "%"), position + new Vector2(Container.Width - rWidth * 0.5f + lastDir * ValueWiggler.Value * 8f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);

            Vector2 vector = Vector2.UnitX * (float)(highlighted ? (Math.Sin(sine * 4f) * 4f) : 0f);

            Vector2 position2 = position + new Vector2(Container.Width - rWidth + 40f + ((lastDir < 0) ? (-ValueWiggler.Value * 8f) : 0f), 0f) - (Index > min ? vector : Vector2.Zero);
            ActiveFont.DrawOutline("<", position2, new Vector2(0.5f, 0.5f), Vector2.One, Index > min ? color : (Color.DarkSlateGray * alpha), 2f, strokeColor);

            position2 = position + new Vector2(Container.Width - 40f + ((lastDir > 0) ? (ValueWiggler.Value * 8f) : 0f), 0f) + (Index < max ? vector : Vector2.Zero);
            ActiveFont.DrawOutline(">", position2, new Vector2(0.5f, 0.5f), Vector2.One, Index < max ? color : (Color.DarkSlateGray * alpha), 2f, strokeColor);
        }
    }
}
