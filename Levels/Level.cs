using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame.Levels
{
    public class Level : IDisposable
    {
        public Action<object> BackToMenu;

        public virtual void Initialize()
        {

        }

        public virtual void LoadContent(ContentManager ContentManager, Point windowSize)
        {

        }

        public virtual void Update(GameTime gameTime, GameState gameState)
        {

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