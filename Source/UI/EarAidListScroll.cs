using System;

namespace Celeste.Mod.EarAid.UI;

internal static class EarAidListScroll
{
    public static int GetVisibleContentRowCount(int itemCount, int scroll, int maxRows)
    {
        int contentRows = maxRows;
        if (scroll > 0)
        {
            contentRows--;
        }

        if (scroll + contentRows < itemCount)
        {
            contentRows--;
        }

        return Math.Max(contentRows, 1);
    }

    public static void EnsureIndexVisible(ref int scroll, int index, int itemCount, int maxRows)
    {
        if (itemCount == 0)
        {
            scroll = 0;
            return;
        }

        if (index < scroll)
        {
            scroll = index;
            return;
        }

        while (index >= scroll + GetVisibleContentRowCount(itemCount, scroll, maxRows))
        {
            scroll++;
        }
    }
}
