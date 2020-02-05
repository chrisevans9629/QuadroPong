using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame
{
    public class GameTimer
    {
        public float CountDuration { get; set; } = 4f; //every  2s.

        public float CurrentTime { get; set; }

        public bool IsRunning { get; set; }
        public bool IsCompleted { get; set; }
        public void Update(GameTime gameTime)
        {
            if (IsRunning)
            {
                CurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 

                if (CurrentTime >= CountDuration)
                {
                    IsRunning = false;
                    CurrentTime = 0;
                    IsCompleted = true;
                }
            }
        }

        public SpriteFont Font { get; set; }
        public void Restart()
        {
            IsCompleted = false;
            IsRunning = true;
            CurrentTime = 1f;
        }

        public void Draw(SpriteBatch spriteBatch, int width, int height)
        {
            if (IsRunning)
            {
                spriteBatch.DrawString(Font, CurrentTime.ToString("0"), new Vector2(width/2f,height/4f ), Color.White);
            }
        }
    }
}