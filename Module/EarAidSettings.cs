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

    public int Conveyor { get; set; } = 10;
    public int Death { get; set; } = 10;
    public int GoldenDeath { get; set; } = 10;
    public int Dialogue { get; set; } = 10;
    public int DreamBlock { get; set; } = 10;
    public int FireballIdle { get; set; } = 10;
    public int HeartCollect { get; set; } = 10;
    public int ItemCrystalDeath { get; set; } = 10;
    public int LightningStrike { get; set; } = 10;
    public int MoveBlock { get; set; } = 10;
    public int OshiroBoss { get; set; } = 10;
    public int Spring { get; set; } = 10;
    public int TouchSwitchComplete { get; set; } = 10;
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

    private void CreateGenericEntry(TextMenu menu, string labelId, string subtextId, int initValue, Action<int> onChange)
    {
        TextMenu.Item item = new EaseInVolumeSlider(Dialog.Clean(labelId), menu, initValue).Change(onChange);
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(subtextId));
        options.Add(item);
    }

    public void CreateConveyorEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuConveyor, DialogIds.MenuConveyorSubtext, Conveyor, (value) =>
        {
            Conveyor = value;
            Mixer.MixExistingInstances(SFX.game_09_conveyor_activate, value);
            Mixer.MixExistingInstances(SFX.env_loc_09_conveyer_idle, value);
        });

    public void CreateDeathEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuDeath, DialogIds.MenuDeathSubtext, Death, (value) =>
        {
            Death = value;
            Mixer.MixExistingInstances(SFX.char_mad_death, value);
            Mixer.MixExistingInstances(SFX.char_mad_predeath, value);
        });

    public void CreateGoldenDeathEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuGoldenDeath, DialogIds.MenuGoldenDeathSubtext, GoldenDeath, (value) =>
        {
            GoldenDeath = value;
            Mixer.MixExistingInstances(SFX.char_mad_death_golden, value);
        });
    
    public void CreateDialogueEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuDialogue, DialogIds.MenuDialogueSubtext, Dialogue, (value) =>
        {
            Dialogue = value;
            Mixer.MixExistingInstances(EventConsts.DialogueBadeline, value);
            Mixer.MixExistingInstances(EventConsts.DialogueEx, value);
            Mixer.MixExistingInstances(EventConsts.DialogueGranny, value);
            Mixer.MixExistingInstances(EventConsts.DialogueMadeline, value);
            Mixer.MixExistingInstances(EventConsts.DialogueMadelineMirror, value);
            Mixer.MixExistingInstances(EventConsts.DialogueMom, value);
            Mixer.MixExistingInstances(EventConsts.DialogueOshiro, value);
            Mixer.MixExistingInstances(EventConsts.DialogueTheo, value);
            Mixer.MixExistingInstances(EventConsts.DialogueTheoMirror, value);
            Mixer.MixExistingInstances(EventConsts.DialogueTheoWebcam, value);
        });

    public void CreateDreamBlockEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuDreamBlock, DialogIds.MenuDreamBlockSubtext, DreamBlock, (value) =>
        {
            DreamBlock = value;
            Mixer.MixExistingInstances(SFX.char_mad_dreamblock_enter, value);
            Mixer.MixExistingInstances(SFX.char_mad_dreamblock_exit, value);
            Mixer.MixExistingInstances(SFX.char_mad_dreamblock_travel, value);
        });
    
    public void CreateFireballIdleEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuFireballIdle, DialogIds.MenuFireballIdleSubtext, FireballIdle, (value) =>
        {
            FireballIdle = value;
            Mixer.MixExistingInstances(SFX.env_loc_09_fireball_idle, value);
        });
    
    public void CreateHeartCollectEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuHeartCollect, DialogIds.MenuHeartCollectSubtext, HeartCollect, (value) =>
        {
            HeartCollect = value;
            Mixer.MixExistingInstances(SFX.game_gen_crystalheart_blue_get, value);
            Mixer.MixExistingInstances(SFX.game_gen_crystalheart_red_get, value);
            Mixer.MixExistingInstances(SFX.game_gen_crystalheart_gold_get, value);
        });

    public void CreateItemCrystalDeathEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuItemCrystalDeath, DialogIds.MenuItemCrystalDeathSubtext, ItemCrystalDeath, (value) =>
        {
            ItemCrystalDeath = value;
            Mixer.MixExistingInstances(EventConsts.ItemCrystalDeath, value);
        });
    
    public void CreateLightningStrikeEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuLightningStrike, DialogIds.MenuLightningStrikeSubtext, LightningStrike, (value) =>
        {
            LightningStrike = value;
            Mixer.MixExistingInstances(SFX.game_10_lightning_strike, value);
        });
    
    public void CreateMoveBlockEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuMoveBlock, DialogIds.MenuMoveBlockSubtext, MoveBlock, (value) =>
        {
            MoveBlock = value;
            Mixer.MixExistingInstances(SFX.game_04_arrowblock_activate, value);
            Mixer.MixExistingInstances(SFX.game_04_arrowblock_break, value);
            Mixer.MixExistingInstances(SFX.game_04_arrowblock_move_loop, value);
            Mixer.MixExistingInstances(SFX.game_04_arrowblock_reappear, value);
            Mixer.MixExistingInstances(SFX.game_04_arrowblock_reform_begin, value);
            Mixer.MixExistingInstances(SFX.game_04_arrowblock_side_depress, value);
            Mixer.MixExistingInstances(SFX.game_04_arrowblock_side_release, value);
            Mixer.MixExistingInstances(EventConsts.MoveBlockMoveCommunal, value);
            Mixer.MixExistingInstances(EventConsts.MoveBlockBreakCommunal, value);
        });
    
    public void CreateOshiroBossEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuOshiroBoss, DialogIds.MenuOshiroBossSubtext, OshiroBoss, (value) =>
        {
            OshiroBoss = value;
            Mixer.MixExistingInstances(SFX.char_oshiro_boss_charge, value);
            Mixer.MixExistingInstances(SFX.char_oshiro_boss_enterscreen, value);
            Mixer.MixExistingInstances(SFX.char_oshiro_boss_precharge, value);
            Mixer.MixExistingInstances(SFX.char_oshiro_boss_reform, value);
        });
    
    public void CreateSpringEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuSpring, DialogIds.MenuSpringSubtext, Spring, (value) => 
        {
            Spring = value;
            Mixer.MixExistingInstances(SFX.game_gen_spring, value);
        });

    public void CreateTouchSwitchCompleteEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuTouchSwitchComplete, DialogIds.MenuTouchSwitchCompleteSubtext, TouchSwitchComplete, (value) =>
        {
            TouchSwitchComplete = value;
            Mixer.MixExistingInstances(SFX.game_gen_touchswitch_last_cutoff, value);
            Mixer.MixExistingInstances(SFX.game_gen_touchswitch_last_oneshot, value);
        });
    
    public void CreateFarewellWindEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuFarewellWind, DialogIds.MenuFarewellWindSubtext, FarewellWind, (value) =>
        {
            FarewellWind = value;
            Mixer.MixExistingInstances(SFX.env_amb_10_voidspiral, value);
        });
    
    public void CreateRidgeWindEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuRidgeWind, DialogIds.MenuRidgeWindSubtext, RidgeWind, (value) =>
        {
            RidgeWind = value;
            Mixer.MixExistingInstances(SFX.env_amb_04_main, value);
        });
    
    public void CreateZipMoverEntry(TextMenu menu, bool inGame) => CreateGenericEntry(menu, DialogIds.MenuZipMover, DialogIds.MenuZipMoverSubtext, ZipMover, (value) =>
        {
            ZipMover = value;
            Mixer.MixExistingInstances(SFX.game_01_zipmover, value);
            Mixer.MixExistingInstances(SFX.game_10_zip_mover, value);
        });
}

// Bastard child born of a nasty threeway between TextMenuExt.EaseInSubHeaderExt, TextMenuExt.IntSlider, and SRTool's EaseInSubMenu
internal class EaseInVolumeSlider : TextMenuExt.IntSlider
{
    public bool FadeVisible { get; private set; } = true;

    private readonly TextMenu containingMenu;
    private float Alpha;
    private float uneasedAlpha;

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

    public void SetFadeVisible(int value)
    {
        FadeVisible = EarAidModule.Settings.Enabled && (!EarAidModule.Settings.HideUnusedOptions || value < 10);
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
