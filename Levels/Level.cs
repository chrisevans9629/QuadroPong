using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame.Levels
{
    public class Level : IDisposable
    {
        protected readonly IPongGame PongGame;

        public Level(IPongGame pongGame)
        {
            PongGame = pongGame;
        }
        public virtual void Initialize()
        {

        }

        public virtual void LoadContent(ContentManager Content, Point windowSize)
        {

        }

        public virtual void Update(GameTime gameTime, GameState gameState)
        {
            var cap = GamePad.GetCapabilities(PlayerIndex.One);
            if (cap.IsConnected && cap.HasStartButton && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
            {
                PongGame.ShowMainMenu();
            }
        }

        public virtual void WindowResized()
        {

        }

        public virtual void Draw(SpriteBatch batch, GameTime gameTime, Point window)
        {

        }

        public virtual void Dispose()
        {
        }
    }
}