using Celeste.Mod.EarAid.Utils;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Module;

[SettingName(DialogIds.MenuEarAid)]
public class EarAidSettings : EverestModuleSettings
{
    public bool Enabled { get; set; } = true;
    public bool HideUnusedOptions { get; set; } = false;

    public int Death { get; set; } = 10;
    public int GoldenDeath { get; set; } = 10;
    public int DreamBlock { get; set; } = 10;
    public int FireballIdle { get; set; } = 10;
    public int HeartCollect { get; set; } = 10;
    public int LightningStrike { get; set; } = 10;
    public int FarewellWind { get; set; } = 10;
    public int RidgeWind { get; set; } = 10;
    public int ZipMover { get; set; } = 10;

    private readonly List<TextMenu.Item> options = new();

    public void CreateEnabledEntry(TextMenu menu, bool inGame)
    {
        menu.Add(new TextMenu.OnOff(Dialog.Clean(DialogIds.MenuEnabled), Enabled).Change(value =>
        {
            Enabled = value;

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

            foreach (string path in EventConsts.Paths)
            {
                Mixer.MixExistingInstances(path, value ? EventConsts.PathToSetting(path) : 10);
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
    }

    public void CreateHideUnusedOptionsEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInOnOff(Dialog.Clean(DialogIds.MenuHideUnusedOptions), HideUnusedOptions, Enabled, menu).Change(value =>
        {
            HideUnusedOptions = value;

            foreach (TextMenu.Item item in options)
            {
                if (item is EaseInVolumeSlider slider)
                {
                    slider.SetFadeVisible(slider.Index);
                }
            }
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuHideUnusedOptionsSubtext));
        options.Add(item);
    }

    public void CreateDeathEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(DialogIds.MenuDeath), menu, Death).Change(value =>
        {
            Death = value;
            Mixer.MixExistingInstances(EventConsts.Death, value);
            Mixer.MixExistingInstances(EventConsts.PreDeath, value);
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuDeathSubtext));
        options.Add(item);
    }

    public void CreateGoldenDeathEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(DialogIds.MenuGoldenDeath), menu, GoldenDeath).Change(value =>
        {
            GoldenDeath = value;
            Mixer.MixExistingInstances(EventConsts.GoldenDeath, value);
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuGoldenDeathSubtext));
        options.Add(item);
    }

    public void CreateDreamBlockEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(DialogIds.MenuDreamBlock), menu, DreamBlock).Change(value =>
        {
            DreamBlock = value;
            Mixer.MixExistingInstances(EventConsts.DreamBlockEnter, value);
            Mixer.MixExistingInstances(EventConsts.DreamBlockExit, value);
            Mixer.MixExistingInstances(EventConsts.DreamBlockTravel, value);
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuDreamBlockSubtext));
        options.Add(item);
    }

    public void CreateFireballIdleEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(DialogIds.MenuFireballIdle), menu, FireballIdle).Change(value =>
        {
            FireballIdle = value;
            Mixer.MixExistingInstances(EventConsts.FireballIdle, value);
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuFireballIdleSubtext));
        options.Add(item);
    }

    public void CreateHeartCollectEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(DialogIds.MenuHeartCollect), menu, HeartCollect).Change(value =>
        {
            HeartCollect = value;
            Mixer.MixExistingInstances(EventConsts.BlueHeartCollect, value);
            Mixer.MixExistingInstances(EventConsts.RedHeartCollect, value);
            Mixer.MixExistingInstances(EventConsts.GoldHeartCollect, value);
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuHeartCollectSubtext));
        options.Add(item);
    }

    public void CreateLightningStrikeEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(DialogIds.MenuLightningStrike), menu, LightningStrike).Change(value =>
        {
            LightningStrike = value;
            Mixer.MixExistingInstances(EventConsts.LightningStrike, value);
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuLightningStrikeSubtext));
        options.Add(item);
    }

    public void CreateFarewellWindEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(DialogIds.MenuFarewellWind), menu, FarewellWind).Change(value =>
        {
            FarewellWind = value;
            Mixer.MixExistingInstances(EventConsts.FarewellWind, value);
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuFarewellWindSubtext));
        options.Add(item);
    }

    public void CreateRidgeWindEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(DialogIds.MenuRidgeWind), menu, RidgeWind).Change(value =>
        {
            RidgeWind = value;
            Mixer.MixExistingInstances(EventConsts.RidgeWind, value);
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuRidgeWindSubtext));
        options.Add(item);
    }

    public void CreateZipMoverEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(DialogIds.MenuZipMover), menu, ZipMover).Change(value =>
        {
            ZipMover = value;
            Mixer.MixExistingInstances(EventConsts.CityZipMover, value);
            Mixer.MixExistingInstances(EventConsts.FarewellZipMover, value);
        });
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuZipMoverSubtext));
        options.Add(item);
    }
}

// Bastard child born of a nasty threeway between TextMenuExt.EaseInSubHeaderExt, TextMenuExt.IntSlider, and SRTool's EaseInSubMenu
internal class EaseInVolumeSlider : TextMenuExt.IntSlider
{
    public bool FadeVisible { get; private set; } = true;

    private TextMenu containingMenu;
    private float Alpha;
    private float uneasedAlpha;

    public void SetFadeVisible(int value)   
    {
        FadeVisible = EarAidModule.Settings.Enabled && (!EarAidModule.Settings.HideUnusedOptions || value < 10);
    }

    public EaseInVolumeSlider(string label, TextMenu menu, int value) : base(label, 0, 10, value)
    {
        containingMenu = menu;

        SetFadeVisible(value);
        Alpha = FadeVisible ? 1 : 0;
        uneasedAlpha = Alpha;

        // Will remove option if scrolled to 10 and then cursor moves off while HideUnusedOptions
        // May not be necessary if people cry about it
        OnLeave = () => SetFadeVisible(Index);
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
        // We need to get the private fields from TextMenuExt.IntSlider
        float sine = DynamicData.For(this).Get<float>("sine");
        int lastDir = DynamicData.For(this).Get<int>("lastDir");
        int min = DynamicData.For(this).Get<int>("min");
        int max = DynamicData.For(this).Get<int>("max");

        // All this just to add the * Alpha lol
        float alpha = Container.Alpha * Alpha;

        Color strokeColor = Color.Black * (alpha * alpha * alpha);
        Color color = Disabled ? Color.DarkSlateGray : ((highlighted ? Container.HighlightColor : Color.White) * alpha);
        ActiveFont.DrawOutline(Label, position, new Vector2(0f, 0.5f), Vector2.One, color, 2f, strokeColor);

        if ((max - min) > 0)
        {
            float rWidth = RightWidth();
            ActiveFont.DrawOutline(Index.ToString(), position + new Vector2(Container.Width - rWidth * 0.5f + lastDir * ValueWiggler.Value * 8f, 0f), new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color, 2f, strokeColor);
            
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

    private TextMenu containingMenu;
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
