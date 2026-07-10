using Celeste.Mod.EarAid.UI;
using Monocle;

namespace Celeste.Mod.EarAid.Module;

public static class EarAidMenu
{
    private static int sectionStartIndex = -1;
    private static int sectionLength;
    private static TextMenu trackedMenu;
    private static bool trackedInGame;
    private static TextMenu.Item enabledItem;
    private static TextMenu.Item manageItem;

    public static void CreateMenu(TextMenu menu, bool inGame)
    {
        trackedMenu = menu;
        trackedInGame = inGame;
        sectionStartIndex = menu.Items.Count;
        sectionLength = AppendSection(menu, inGame);
    }

    public static void RefreshMenu()
    {
        if (trackedMenu == null || sectionStartIndex < 0)
        {
            return;
        }

        int selection = trackedMenu.Selection;
        if (selection >= sectionStartIndex && selection < sectionStartIndex + sectionLength)
        {
            trackedMenu.Items[selection].OnLeave?.Invoke();
        }

        for (int i = 0; i < sectionLength; i++)
        {
            EarAidMenuSection.RemoveItem(trackedMenu, sectionStartIndex);
        }

        sectionLength = InsertSection(trackedMenu, sectionStartIndex, trackedInGame);
        EarAidMenuSection.ApplySelection(trackedMenu, manageItem);
    }

    private static int AppendSection(TextMenu menu, bool inGame)
    {
        int startCount = menu.Items.Count;
        BuildSection(menu, inGame, (item) => menu.Add(item), (button, description) =>
        {
            button.AddDescription(menu, description);
        });
        return menu.Items.Count - startCount;
    }

    private static int InsertSection(TextMenu menu, int startIndex, bool inGame)
    {
        int index = startIndex;
        BuildSection(menu, inGame,
            (item) => index = EarAidMenuSection.InsertItem(menu, index, item),
            (button, description) => index = EarAidMenuSection.InsertDescription(menu, index, button, description));
        return index - startIndex;
    }

    private static void BuildSection(
        TextMenu menu,
        bool inGame,
        System.Action<TextMenu.Item> addItem,
        System.Action<TextMenu.Item, string> addDescription)
    {
        enabledItem = new TextMenu.OnOff(Dialog.Clean("EAR_AID_ENABLED"), EarAidModule.Settings.Enabled);
        ((TextMenu.OnOff)enabledItem).Change(value =>
        {
            EarAidModule.Settings.Enabled = value;

            foreach (SoundGroup group in EarAidModule.Settings.SoundGroups)
            {
                int volume = value ? group.Volume : VolumeConstants.DefaultVolume;
                Mixer.MixExistingInstances(group.EventPaths, volume);
            }

            if (value)
            {
                EarAidModule.Instance.Load();
            }
            else
            {
                EarAidModule.Instance.Unload();
            }
        });
        addItem(enabledItem);

        TextMenu.Button manageButton = new(Dialog.Clean("EAR_AID_OPEN_MANAGE"));
        manageButton.Pressed(() => OpenOverlay(menu, new EarAidGroupManageUI(menu)));
        manageItem = manageButton;
        addItem(manageButton);
        addDescription(manageButton, Dialog.Clean("EAR_AID_OPEN_MANAGE_SUBTEXT"));

        foreach (SoundGroup group in EarAidModule.Settings.SoundGroups)
        {
            SoundGroup capturedGroup = group;

            VolumeSlider slider = new VolumeSlider(group.DisplayName, group.Volume);
            slider.Change(value =>
            {
                capturedGroup.Volume = value;
                Events.SetGroupVolume(capturedGroup);
                Mixer.MixExistingInstances(capturedGroup.EventPaths, value);
            });
            addItem(slider);
        }

    }

    private static void OpenOverlay(TextMenu menu, Entity overlay)
    {
        menu.Focused = false;

        if (overlay is EarAidEventSearchUI searchUi)
        {
            searchUi.OnClose = () => CloseOverlay(menu);
        }
        else if (overlay is EarAidGroupManageUI manageUi)
        {
            manageUi.OnClose = () => CloseOverlay(menu);
        }

        Engine.Scene.Add(overlay);
        Engine.Scene.OnEndOfFrame += () => Engine.Scene.Entities.UpdateLists();
    }

    private static void CloseOverlay(TextMenu menu)
    {
        RefreshMenu();
        menu.Focused = true;
    }
}
