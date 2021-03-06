﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame;

namespace PongGame
{
    public class GameResult
    {
        public GameResult()
        {
            Goal = 10;
        }

        public int Goal { get; set; }

        public PongPlayer? Winner { get; set; }
        public void Update(List<PongPlayer> players)
        {
            var player = players.FirstOrDefault(p => p.Paddle.Score >= Goal);

            if (player != null)
            {
                Winner = player;
                return;
            }

            if (players.Count(p => !p.Goal.Died) == 1)
            {
                player = players.FirstOrDefault(p => p.Goal.Died != true);
                Winner = player;
                return;
            }
        }
        
        public void Reset()
        {
            Winner = null;
        }
    }
}
