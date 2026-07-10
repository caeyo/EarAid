using Microsoft.Xna.Framework.Input;
using Monocle;
using System;

namespace Celeste.Mod.EarAid.UI;

internal sealed class ListInput
{
    private const float InitialHoldDelay = 0.4f;
    private const float RepeatInterval = 0.045f;

    private int direction;
    private float holdTime;
    private float repeatTimer;

    public void Reset()
    {
        direction = 0;
        holdTime = 0f;
        repeatTimer = 0f;
    }

    public void UpdateVertical(Action<int> moveSelection)
    {
        int newDirection = 0;
        if (MInput.Keyboard.Check(Keys.Up) && !MInput.Keyboard.Check(Keys.Down))
        {
            newDirection = -1;
        }
        else if (MInput.Keyboard.Check(Keys.Down) && !MInput.Keyboard.Check(Keys.Up))
        {
            newDirection = 1;
        }

        if (newDirection == 0)
        {
            Reset();
            return;
        }

        if (newDirection != direction)
        {
            direction = newDirection;
            holdTime = 0f;
            repeatTimer = 0f;
            moveSelection(direction);
            return;
        }

        holdTime += Engine.DeltaTime;
        if (holdTime < InitialHoldDelay)
        {
            return;
        }

        repeatTimer += Engine.DeltaTime;
        int steps = (int)(repeatTimer / RepeatInterval);
        if (steps <= 0)
        {
            return;
        }

        repeatTimer -= steps * RepeatInterval;
        for (int i = 0; i < steps; i++)
        {
            moveSelection(direction);
        }
    }
}
