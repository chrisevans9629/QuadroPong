using System;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public static class PlayerNameExt
    {
        public static Color ToColor(this PlayerName playerName)
        {
            return playerName switch 
                {
                PlayerName.PlayerOne => Color.Red,
                PlayerName.PlayerTwo => Color.Blue,
                PlayerName.PlayerThree => Color.Green,
                PlayerName.PlayerFour => Color.Purple,
                _ => throw new NotImplementedException()
                };
        }
    }
}