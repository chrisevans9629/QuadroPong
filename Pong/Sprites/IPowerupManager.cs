using System;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public interface IPowerupManager
    {
        void AddTimedPowerup(Sprite sprite,PowerUpType type, float seconds, Action completeAction);
        void UpdateTimedPowerup(GameTime gameTimer);
    }
}