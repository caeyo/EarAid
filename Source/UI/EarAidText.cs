using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.EarAid.UI;

internal static class EarAidText
{
    private const float OutlineWidth = 2f;
    private static readonly char[] PathBreakChars = { '/', '_', '.' };

    public static string[] WrapToLines(string text, float maxWidth, float scale = 1f, string continuationIndent = "")
    {
        if (string.IsNullOrEmpty(text))
        {
            return Array.Empty<string>();
        }

        if (string.IsNullOrEmpty(continuationIndent))
        {
            return WrapPlain(text, maxWidth, scale);
        }

        float fullLimit = maxWidth / scale;
        float indentWidth = ActiveFont.Measure(continuationIndent).X;
        if (ActiveFont.Measure(text).X <= fullLimit)
        {
            return new[] { text };
        }

        List<string> lines = new();
        int start = 0;
        bool isFirstLine = true;

        while (start < text.Length)
        {
            string indent = isFirstLine ? "" : continuationIndent;
            float lineLimit = isFirstLine ? fullLimit : fullLimit - indentWidth;
            int breakAt = FindLineBreak(text, start, lineLimit);
            lines.Add(indent + text[start..breakAt]);
            start = breakAt;
            isFirstLine = false;
        }

        return lines.ToArray();
    }

    public static void DrawOutlinedLines(string[] lines, Vector2 top, float lineStep, float scale, Color color)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            ActiveFont.DrawOutline(
                lines[i],
                top + new Vector2(0f, i * lineStep),
                Vector2.Zero,
                Vector2.One * scale,
                color,
                OutlineWidth,
                Color.Black);
        }
    }

    private static string[] WrapPlain(string text, float maxWidth, float scale)
    {
        float limit = maxWidth / scale;
        if (ActiveFont.Measure(text).X <= limit)
        {
            return new[] { text };
        }

        List<string> lines = new();
        int start = 0;
        while (start < text.Length)
        {
            int breakAt = FindLineBreak(text, start, limit);
            lines.Add(text[start..breakAt]);
            start = breakAt;
        }

        return lines.ToArray();
    }

    private static int FindLineBreak(string text, int start, float limit)
    {
        int remaining = text.Length - start;
        if (remaining <= 0)
        {
            return start;
        }

        if (ActiveFont.Measure(text[start..]).X <= limit)
        {
            return text.Length;
        }

        int lo = 1;
        int hi = remaining;
        int best = 1;
        while (lo <= hi)
        {
            int mid = (lo + hi) / 2;
            if (ActiveFont.Measure(text.Substring(start, mid)).X <= limit)
            {
                best = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }

        if (start + best >= text.Length)
        {
            return text.Length;
        }

        int segmentEnd = start + best;
        int preferredBreak = FindPreferredBreak(text, start, segmentEnd);
        if (preferredBreak > start)
        {
            return preferredBreak;
        }

        return segmentEnd;
    }

    private static int FindPreferredBreak(string text, int start, int segmentEnd)
    {
        for (int i = segmentEnd - 1; i > start; i--)
        {
            if (Array.IndexOf(PathBreakChars, text[i]) >= 0)
            {
                return i + 1;
            }
        }

        return start;
    }
}
