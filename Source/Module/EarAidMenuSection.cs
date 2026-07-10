using Monocle;
using System;

namespace Celeste.Mod.EarAid.Module;

internal static class EarAidMenuSection
{
    public static int InsertItem(TextMenu menu, int index, TextMenu.Item item)
    {
        menu.Items.Insert(index, item);
        item.Container = menu;
        menu.Add(item.ValueWiggler = Wiggler.Create(0.25f, 3f, null, false, false));
        menu.Add(item.SelectWiggler = Wiggler.Create(0.25f, 3f, null, false, false));
        item.ValueWiggler.UseRawDeltaTime = true;
        item.SelectWiggler.UseRawDeltaTime = true;
        item.Added();
        menu.RecalculateSize();
        return index + 1;
    }

    public static int InsertDescription(TextMenu menu, int index, TextMenu.Item button, string description)
    {
        TextMenuExt.EaseInSubHeaderExt subHeader = new(description, false, menu)
        {
            HeightExtra = 0f
        };
        index = InsertItem(menu, index, subHeader);
        button.OnEnter = () => subHeader.FadeVisible = true;
        button.OnLeave = () => subHeader.FadeVisible = false;
        return index;
    }

    public static void RemoveItem(TextMenu menu, int index)
    {
        TextMenu.Item item = menu.Items[index];
        menu.Items.RemoveAt(index);

        if (item.ValueWiggler != null)
        {
            menu.Remove(item.ValueWiggler);
        }

        if (item.SelectWiggler != null)
        {
            menu.Remove(item.SelectWiggler);
        }

        menu.RecalculateSize();
    }

    public static void ApplySelection(TextMenu menu, TextMenu.Item item)
    {
        int targetIndex = menu.IndexOf(item);
        if (targetIndex < 0)
        {
            return;
        }

        int previousSelection = menu.Selection;
        if (previousSelection >= 0
            && previousSelection < menu.Items.Count
            && previousSelection != targetIndex)
        {
            menu.Items[previousSelection].OnLeave?.Invoke();
        }

        menu.Selection = targetIndex;
        menu.Items[targetIndex].OnEnter?.Invoke();
        menu.Items[targetIndex].SelectWiggler?.Start();
    }
}
