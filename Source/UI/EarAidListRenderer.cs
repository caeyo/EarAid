using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.EarAid.UI;

/// <summary>
/// Draws a vertically scrolling list with "more above/below" arrows. The caller
/// supplies a delegate that renders a single item at a given position, keeping
/// row styling (colour, label) local to each screen.
/// </summary>
internal static class EarAidListRenderer
{
    private const float ArrowScale = 0.75f;

    /// <summary>
    /// Renders the visible window of the list starting at <paramref name="top"/> and
    /// returns the number of rows drawn (including any scroll arrows), which callers
    /// can use to position content below the list.
    /// </summary>
    public static int Draw(Vector2 top, int itemCount, int scroll, int maxRows, float rowHeight, Action<int, Vector2> drawItem)
    {
        int contentRows = EarAidListScroll.GetVisibleContentRowCount(itemCount, scroll, maxRows);
        int row = 0;

        if (scroll > 0)
        {
            DrawArrow("  ^", top);
            row++;
        }

        for (int i = 0; i < contentRows; i++)
        {
            int index = scroll + i;
            if (index >= itemCount)
            {
                break;
            }

            drawItem(index, top + new Vector2(0f, row * rowHeight));
            row++;
        }

        if (scroll + contentRows < itemCount)
        {
            DrawArrow("  v", top + new Vector2(0f, row * rowHeight));
        }

        return row;
    }

    private static void DrawArrow(string arrow, Vector2 position)
    {
        ActiveFont.DrawOutline(arrow, position, Vector2.Zero, Vector2.One * ArrowScale, Color.Gray, 2f, Color.Black);
    }
}
