using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public class PowerupManager : IPowerupManager
    {
        class PowerupStore
        {
            public GameTimer GameTimer { get; set; }
            public Action Complete { get; set; }
        }

        

        Dictionary<Sprite, Dictionary<PowerUpType, PowerupStore>> TimedPowerups { get; set; } = new Dictionary<Sprite, Dictionary<PowerUpType, PowerupStore>>();

        public void AddTimedPowerup(Sprite sprite,PowerUpType type, float seconds, Action completeAction)
        {
            if (TimedPowerups.ContainsKey(sprite))
            {
                if (TimedPowerups[sprite].ContainsKey(type))
                {
                    TimedPowerups[sprite][type].GameTimer.EveryNumOfSeconds += seconds;
                }
                else
                {
                    var timer = new GameTimer() { EveryNumOfSeconds = seconds };
                    timer.Restart();
                    TimedPowerups[sprite].Add(type, new PowerupStore(){GameTimer = timer, Complete = completeAction});
                }
            }
            else
            {
                var dict = new Dictionary<PowerUpType, PowerupStore>();
                var timer = new GameTimer() { EveryNumOfSeconds = seconds };
                timer.Restart();
                dict.Add(type, new PowerupStore(){GameTimer = timer, Complete = completeAction});
                TimedPowerups.Add(sprite, dict);
            }

            
        }

        public void UpdateTimedPowerup(GameTime gameTimer)
        {
            foreach (var keyValuePair in TimedPowerups.ToList())
            {
                foreach (var powerupStore in keyValuePair.Value.ToList())
                {
                    powerupStore.Value.GameTimer.Update(gameTimer);
                    if (powerupStore.Value.GameTimer.IsCompleted)
                    {
                        powerupStore.Value.Complete();
                        TimedPowerups.Remove(keyValuePair.Key);
                    }
                }

                if (keyValuePair.Value.Any() != true)
                {
                    TimedPowerups.Remove(keyValuePair.Key);
                }
            }
        }
    }
}