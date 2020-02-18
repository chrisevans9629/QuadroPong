using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame
{
    public class PlayerStats
    {
        public SpriteFont? SpriteFont { get; set; }
        public PlayerStatsState State { get; set; } = new PlayerStatsState();
        public void Draw(SpriteBatch spriteBatch)
        {
            var one = new Vector2(0,1);
            spriteBatch.DrawString(SpriteFont, State.PlayerName.ToString(), State.Position, State.PlayerName.ToColor());
            spriteBatch.DrawString(SpriteFont,$"Score: {State.Score}", State.Position + one * 30f,Color.White);
            spriteBatch.DrawString(SpriteFont,$"Health: {State.Health}", State.Position + one * 60f,Color.White);
        }
    }
}