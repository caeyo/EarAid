﻿using Celeste.Mod.EarAid.EarAid;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.EarAid.Module;

public static class EarAidMenu
{
    private static List<TextMenu.Item> options;

    public static void CreateMenu(TextMenu menu)
    {
        // Create and add 'Enabled'
        menu.Add(new TextMenu.OnOff(Dialog.Clean("EAR_AID_ENABLED"), EarAidModule.Settings.Enabled).Change(value =>
        {
            EarAidModule.Settings.Enabled = value;

            foreach (TextMenu.Item item in options)
            {
                if (item is EaseInVolumeSlider slider)
                {
                    slider.SetFadeVisible(slider.Index);
                }
                else if (item is EaseInOnOff onoff)
                {
                    onoff.FadeVisible = value;
                }
            }

            foreach (KeyValuePair<string, MethodInfo> kvp in Events.PathToSettingGetter)
            {
                Mixer.MixExistingInstances(kvp.Key, value ? (int)kvp.Value.Invoke(EarAidModule.Settings, null) : 10);
            }

            if (value)
            {
                EarAidModule.Instance.Load();
            }
            else
            {
                EarAidModule.Instance.Unload();
            }
        }));

        // Wipe options because otherwise it just keeps stacking lol
        options = new();

        // Create 'Hide Unused Options'
        TextMenu.Item hideUnusedItem = new EaseInOnOff(Dialog.Clean("EAR_AID_HIDE_UNUSED_OPTIONS"), EarAidModule.Settings.HideUnusedOptions, 
            EarAidModule.Settings.Enabled, menu).Change(value =>
        {
            EarAidModule.Settings.HideUnusedOptions = value;

            foreach (TextMenu.Item item in options)
            {
                if (item is EaseInVolumeSlider slider)
                {
                    slider.SetFadeVisible(slider.Index);
                }
            }
        });
        menu.Add(hideUnusedItem);
        hideUnusedItem = hideUnusedItem.AddDescription(menu, Dialog.Clean("EAR_AID_HIDE_UNUSED_OPTIONS_SUBTEXT"));
        options.Add(hideUnusedItem);

        // Create all volume sliders
        foreach (PropertyInfo volSetting in EarAidSettings.VolumeSettings)
        {
            string labelId = "EAR_AID_" + volSetting.Name.ToUpper();
            string subtextId = labelId + "_SUBTEXT";

            TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(labelId), menu, (int)volSetting.GetValue(EarAidModule.Settings)).Change(value =>
            {
                volSetting.SetValue(EarAidModule.Settings, value);
                foreach (VolumeSettingEventsAttribute attrib in volSetting.GetCustomAttributes().Cast<VolumeSettingEventsAttribute>())
                {
                    Mixer.MixExistingInstances(attrib.EventPaths, value);
                }
            });
            menu.Add(item);
            item = item.AddDescription(menu, Dialog.Clean(subtextId));
            options.Add(item);
        }
    }
}

// Bastard child born of a nasty threeway between TextMenuExt.EaseInSubHeaderExt, TextMenuExt.IntSlider, and SRTool's EaseInSubMenu
internal class EaseInVolumeSlider : TextMenuExt.IntSlider
{
    private static readonly FieldInfo intSliderSine = typeof(TextMenuExt.IntSlider).GetField("sine", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo intSliderLastDir = typeof(TextMenuExt.IntSlider).GetField("lastDir", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo intSliderMin = typeof(TextMenuExt.IntSlider).GetField("min", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo intSliderMax = typeof(TextMenuExt.IntSlider).GetField("max", BindingFlags.NonPublic | BindingFlags.Instance);

    public bool FadeVisible { get; private set; } = true;

    private readonly TextMenu containingMenu;
    private float Alpha;
    private float uneasedAlpha;

    public EaseInVolumeSlider(string label, TextMenu menu, int value) : base(label, 0, 20, value)
    {
        containingMenu = menu;

        SetFadeVisible(value);
        Alpha = FadeVisible ? 1 : 0;
        uneasedAlpha = Alpha;

        // Will remove option if scrolled to 10 and then cursor moves off while HideUnusedOptions
        // May not be necessary if people cry about it
        OnLeave = () => SetFadeVisible(Index);
    }

    public void SetFadeVisible(int value)
    {
        FadeVisible = EarAidModule.Settings.Enabled && (!EarAidModule.Settings.HideUnusedOptions || value != 10);
    }

    public override float Height() => MathHelper.Lerp(-containingMenu.ItemSpacing, base.Height(), Alpha);

    public override void Update()
    {
        base.Update();

        float targetAlpha = FadeVisible ? 1 : 0;
        if (Math.Abs(uneasedAlpha - targetAlpha) > 0.001f)
        {
            uneasedAlpha = Calc.Approach(uneasedAlpha, targetAlpha, Engine.RawDeltaTime * 3f);
            Alpha = FadeVisible ? Ease.SineOut(uneasedAlpha) : Ease.SineIn(uneasedAlpha);
        }

        Visible = Alpha != 0;
    }

    public override float RightWidth()
    {
        return ActiveFont.Measure(((int)intSliderMax.GetValue(this)).ToString() + "0%").X + 120f;
    }

    public override void Render(Vector2 position, bool highlighted)
    {
        // We need to get the private fields from TextMenuExt.IntSlider
        float sine = (float)intSliderSine.GetValue(this);
        int lastDir = (int)intSliderLastDir.GetValue(this);
        int min = (int)intSliderMin.GetValue(this);
        int max = (int)intSliderMax.GetValue(this);

        // All this just to add the * Alpha lol
        // It is possible to do this via IL hook, but because the on-off switch also unloads hooks it doesn't give the options long enough to disappear gracefully
        float alpha = Container.Alpha * Alpha;

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

internal class EaseInOnOff : TextMenu.OnOff
{
    public bool FadeVisible { get; set; } = true;

    private readonly TextMenu containingMenu;
    private float Alpha;
    private float uneasedAlpha;

    public EaseInOnOff(string label, bool on, bool initVisible, TextMenu menu) : base(label, on)
    {
        containingMenu = menu;

        FadeVisible = initVisible;
        Alpha = FadeVisible ? 1 : 0;
        uneasedAlpha = Alpha;
    }

    public override float Height() => MathHelper.Lerp(-containingMenu.ItemSpacing, base.Height(), Alpha);

    public override void Update()
    {
        base.Update();

        float targetAlpha = FadeVisible ? 1 : 0;
        if (Math.Abs(uneasedAlpha - targetAlpha) > 0.001f)
        {
            uneasedAlpha = Calc.Approach(uneasedAlpha, targetAlpha, Engine.RawDeltaTime * 3f);
            Alpha = FadeVisible ? Ease.SineOut(uneasedAlpha) : Ease.SineIn(uneasedAlpha);
        }

        Visible = Alpha != 0;
    }

    public override void Render(Vector2 position, bool highlighted)
    {
        float sine = DynamicData.For(this).Get<float>("sine");
        int lastDir = DynamicData.For(this).Get<int>("lastDir");

        float alpha = Container.Alpha * Alpha;

        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        Color color = Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : Color.White) * alpha);
        ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);

        if (Values.Count > 0)
        {
            float rWidth = RightWidth();
            ActiveFont.DrawOutline(Values[Index].Item1, position + new Vector2(Container.Width - rWidth * 0.5f + lastDir * ValueWiggler.Value * 8f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);

            Vector2 vector = Vector2.UnitX * (float)(highlighted ? (Math.Sin(sine * 4f) * 4f) : 0f);

            bool flag = Index > 0;
            Color color2 = flag ? color : (Color.DarkSlateGray * alpha);
            Vector2 position2 = position + new Vector2(Container.Width - rWidth + 40f + ((lastDir < 0) ? ((0f - ValueWiggler.Value) * 8f) : 0f), 0f) - (flag ? vector : Vector2.Zero);
            ActiveFont.DrawOutline("<", position2, new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);

            bool flag2 = Index < Values.Count - 1;
            color2 = flag2 ? color : (Color.DarkSlateGray * alpha);
            position2 = position + new Vector2(Container.Width - 40f + ((lastDir > 0) ? (ValueWiggler.Value * 8f) : 0f), 0f) + (flag2 ? vector : Vector2.Zero);
            ActiveFont.DrawOutline(">", position2, new Vector2(0.5f, 0.5f), Vector2.One, color2, 2f, strokeColor);
        }
    }
}
