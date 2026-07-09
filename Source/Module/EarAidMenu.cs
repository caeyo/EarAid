using Celeste.Mod.EarAid.EarAid;
using Celeste.Mod.EarAid.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.EarAid.Module;

public static class EarAidMenu
{
    public static void CreateMenu(TextMenu menu, bool inGame)
    {
        menu.Add(new TextMenu.OnOff(Dialog.Clean("EAR_AID_ENABLED"), EarAidModule.Settings.Enabled).Change(value =>
        {
            EarAidModule.Settings.Enabled = value;

            foreach (KeyValuePair<string, int> kvp in Events.PathToVolume)
            {
                Mixer.MixExistingInstances(kvp.Key, value ? kvp.Value : 10);
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

        foreach (SoundGroup group in EarAidModule.Settings.SoundGroups)
        {
            SoundGroup capturedGroup = group;

            menu.Add(new VolumeSlider(group.DisplayName, group.Volume).Change(value =>
            {
                capturedGroup.Volume = value;
                Events.RebuildRegistry(EarAidModule.Settings.SoundGroups);
                Mixer.MixExistingInstances(capturedGroup.EventPaths, value);
            }));

            TextMenu.Button deleteButton = new("    " + Dialog.Clean("EAR_AID_DELETE_GROUP"));
            deleteButton.Pressed(() =>
            {
                Mixer.MixExistingInstances(capturedGroup.EventPaths, 10);
                EarAidModule.Settings.SoundGroups.Remove(capturedGroup);
                Events.RebuildRegistry(EarAidModule.Settings.SoundGroups);
                Audio.Play(SFX.ui_main_button_lowkey);
            });
            menu.Add(deleteButton);
            deleteButton.AddDescription(menu, Dialog.Clean("EAR_AID_DELETE_GROUP_SUBTEXT"));
        }

        TextMenu.Button searchButton = new(Dialog.Clean("EAR_AID_OPEN_SEARCH"));
        searchButton.Pressed(() =>
        {
            menu.Focused = false;
            Engine.Scene.Add(new EarAidEventSearchUI(menu)
            {
                OnClose = () => menu.Focused = true
            });
            Engine.Scene.OnEndOfFrame += () => Engine.Scene.Entities.UpdateLists();
        });
        menu.Add(searchButton);
        searchButton.AddDescription(menu, Dialog.Clean("EAR_AID_OPEN_SEARCH_SUBTEXT"));
    }
}

internal class VolumeSlider : TextMenuExt.IntSlider
{
    private static readonly FieldInfo intSliderSine = typeof(TextMenuExt.IntSlider).GetField("sine", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo intSliderLastDir = typeof(TextMenuExt.IntSlider).GetField("lastDir", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo intSliderMin = typeof(TextMenuExt.IntSlider).GetField("min", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly FieldInfo intSliderMax = typeof(TextMenuExt.IntSlider).GetField("max", BindingFlags.NonPublic | BindingFlags.Instance);

    public VolumeSlider(string label, int value) : base(label, 0, 20, value) { }

    public override float RightWidth()
    {
        return ActiveFont.Measure(((int)intSliderMax.GetValue(this)).ToString() + "0%").X + 120f;
    }

    public override void Render(Vector2 position, bool highlighted)
    {
        float sine = (float)intSliderSine.GetValue(this);
        int lastDir = (int)intSliderLastDir.GetValue(this);
        int min = (int)intSliderMin.GetValue(this);
        int max = (int)intSliderMax.GetValue(this);

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
