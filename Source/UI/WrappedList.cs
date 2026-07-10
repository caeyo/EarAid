using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.UI;

internal static class WrappedList
{
    private const float ItemPadding = 8f;
    private const float ArrowScale = 0.75f;

    public static float LineStep(float scale) => ActiveFont.LineHeight * scale + 2f;

    private static float GetItemHeight(string[] lines, float lineStep) => lines.Length * lineStep + ItemPadding;

    public static float[] ComputeHeights(IReadOnlyList<string[]> items, float lineStep)
    {
        float[] heights = new float[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            heights[i] = GetItemHeight(items[i], lineStep);
        }

        return heights;
    }

    public static void EnsureIndexVisible(
        ref int scroll,
        int index,
        IReadOnlyList<float> heights,
        float viewportHeight,
        float arrowHeight)
    {
        if (heights.Count == 0)
        {
            scroll = 0;
            return;
        }

        index = Math.Clamp(index, 0, heights.Count - 1);

        if (index < scroll)
        {
            scroll = index;
            return;
        }

        while (GetItemBottom(scroll, index, heights) > GetVisibleHeight(scroll, heights.Count, viewportHeight, arrowHeight))
        {
            if (scroll >= index)
            {
                break;
            }

            scroll++;
        }
    }

    public static void Draw(
        Vector2 top,
        IReadOnlyList<string[]> items,
        IReadOnlyList<float> heights,
        int scroll,
        float viewportHeight,
        float lineStep,
        float scale,
        Func<int, Color> getColor)
    {
        float y = top.Y;
        float arrowHeight = lineStep;

        if (scroll > 0)
        {
            DrawArrow(top.X, y, "  ^");
            y += arrowHeight;
        }

        float bottomLimit = top.Y + viewportHeight;
        int lastDrawn = scroll - 1;

        for (int i = scroll; i < items.Count; i++)
        {
            float itemHeight = heights[i];
            bool moreBelow = i < items.Count - 1;
            float reservedBelow = moreBelow ? arrowHeight : 0f;

            if (y + itemHeight > bottomLimit - reservedBelow)
            {
                break;
            }

            Text.DrawOutlinedLines(items[i], new Vector2(top.X, y), lineStep, scale, getColor(i));
            y += itemHeight;
            lastDrawn = i;
        }

        if (lastDrawn < items.Count - 1)
        {
            DrawArrow(top.X, y, "  v");
        }
    }

    private static float GetItemBottom(int scroll, int index, IReadOnlyList<float> heights)
    {
        float offset = 0f;
        for (int i = scroll; i < index; i++)
        {
            offset += heights[i];
        }

        return offset + heights[index];
    }

    private static float GetVisibleHeight(int scroll, int itemCount, float viewportHeight, float arrowHeight)
    {
        float visible = viewportHeight;
        if (scroll > 0)
        {
            visible -= arrowHeight;
        }

        if (scroll < itemCount - 1)
        {
            visible -= arrowHeight;
        }

        return visible;
    }

    private static void DrawArrow(float x, float y, string arrow)
    {
        ActiveFont.DrawOutline(arrow, new Vector2(x, y), Vector2.Zero, Vector2.One * ArrowScale, Color.Gray, 2f, Color.Black);
    }
}
