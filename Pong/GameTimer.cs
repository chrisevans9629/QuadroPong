using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame
{
    public class GameTimer
    {
        public GameTimerState State { get; set; } = new GameTimerState();

        public float EveryNumOfSeconds { get => State.EveryNumOfSeconds; set => State.EveryNumOfSeconds = value; }

        public float CurrentTime { get => State.CurrentTime; set => State.CurrentTime = value; }

        public bool IsRunning { get => State.IsRunning; set => State.IsRunning = value; }
        public bool IsCompleted { get => State.IsCompleted; set => State.IsCompleted = value; }

        public void Update(GameTime gameTime)
        {
            if (State.IsRunning)
            {
                State.CurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds; //Time passed since last Update() 

                if (State.CurrentTime >= State.EveryNumOfSeconds)
                {
                    State.IsRunning = false;
                    State.CurrentTime = 0;
                    State.IsCompleted = true;
                }
            }
        }

        public SpriteFont? Font { get; set; }
        public void Restart()
        {
            State.IsCompleted = false;
            State.IsRunning = true;
            State.CurrentTime = 0f;
        }

        public void Draw(SpriteBatch spriteBatch, int width, int height)
        {
            if (State.IsRunning)
            {
                spriteBatch.DrawString(Font, State.CurrentTime.ToString("0"), new Vector2(width / 2f, height / 4f), Color.White);
            }
        }
    }
}