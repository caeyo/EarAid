using Celeste.Mod.EarAid.Utils;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.Module;

[SettingName(DialogIds.MenuEarAid)]
public class EarAidSettings : EverestModuleSettings
{
    public bool Enabled { get; set; } = true;
    public int RidgeWind { get; set; } = 10;
    public int FarewellWind { get; set; } = 10;
    public int LightningStrike { get; set; } = 10;
    public int GoldenDeath { get; set; } = 10;
    public int FireballIdle { get; set; } = 10;

    private List<TextMenu.Item> options = new();

    public void CreateEnabledEntry(TextMenu menu, bool inGame)
    {
        menu.Add(new TextMenu.OnOff(Dialog.Clean(DialogIds.MenuEnabled), Enabled).Change(value =>
        {
            Enabled = value;

            foreach (TextMenu.Item item in options)
            {
                item.Disabled = !value;
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

    public void CreateRidgeWindEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new TextMenuExt.IntSlider(Dialog.Clean(DialogIds.MenuRidgeWind), 0, 10, RidgeWind).Change(value =>
        {
            RidgeWind = value;
            Mixer.MixExistingInstances(EventConsts.RidgeWind, value);
        });
        item.Disabled = !Enabled;
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuRidgeWindSubtext));
        options.Add(item);
    }

    public void CreateFarewellWindEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new TextMenuExt.IntSlider(Dialog.Clean(DialogIds.MenuFarewellWind), 0, 10, FarewellWind).Change(value =>
        {
            FarewellWind = value;
            Mixer.MixExistingInstances(EventConsts.FarewellWind, value);
        });
        item.Disabled = !Enabled;
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuFarewellWindSubtext));
        options.Add(item);
    }

    public void CreateLightningStrikeEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new TextMenuExt.IntSlider(Dialog.Clean(DialogIds.MenuLightningStrike), 0, 10, LightningStrike).Change(value =>
        {
            LightningStrike = value;
            Mixer.MixExistingInstances(EventConsts.LightningStrike, value);
        });
        item.Disabled = !Enabled;
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuLightningStrikeSubtext));
        options.Add(item);
    }

    public void CreateGoldenDeathEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new TextMenuExt.IntSlider(Dialog.Clean(DialogIds.MenuGoldenDeath), 0, 10, GoldenDeath).Change(value =>
        {
            GoldenDeath = value;
            Mixer.MixExistingInstances(EventConsts.GoldenDeath, value);
        });
        item.Disabled = !Enabled;
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuGoldenDeathSubtext));
        options.Add(item);
    }

    public void CreateFireballIdleEntry(TextMenu menu, bool inGame)
    {
        TextMenu.Item item = new TextMenuExt.IntSlider(Dialog.Clean(DialogIds.MenuFireballIdle), 0, 10, FireballIdle).Change(value =>
        {
            FireballIdle = value;
            Mixer.MixExistingInstances(EventConsts.FireballIdle, value);
        });
        item.Disabled = !Enabled;
        menu.Add(item);
        item = item.AddDescription(menu, Dialog.Clean(DialogIds.MenuFireballIdleSubtext));
        options.Add(item);
    }


}
