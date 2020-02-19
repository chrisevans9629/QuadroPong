using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame;

namespace PongGame.Sprites
{
    public class AstroidManager : IDisposable
    {
        private readonly IRandomizer _randomizer;

        public AstroidManager(IRandomizer randomizer)
        {
            _randomizer = randomizer;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    _astroidPictures.Add(new Rectangle(i*100,j*100,100,100));
                }
            }
        }

        public List<Astroid> Sprites { get; set; } = new List<Astroid>();

        private readonly List<Rectangle> _astroidPictures = new List<Rectangle>();

        public void Reset()
        {
            foreach (var sprite in Sprites)
            {
                sprite.Reset();
            }
        }

        public void Update(GameTime gameTime, List<Ball> balls, int width, int height)
        {
            foreach (var astroid in Sprites)
            {
                astroid.Update(gameTime, width, height, balls);
            }
        }

        public void Load(Texture2D texture2D)
        {
            for (int i = 0; i < 10; i++)
            {
                var sprite = new Astroid(_randomizer);
                sprite.Texture2D = texture2D;
                sprite.Source = _astroidPictures[_randomizer.Next(_astroidPictures.Count)];
                Sprites.Add(sprite);
            }
            Reset();
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            foreach (var astroid in Sprites)
            {
                astroid.Draw(spriteBatch);
            }
        }

        public void Dispose()
        {
            foreach (var astroid in this.Sprites)
            {
                astroid?.Dispose();
            }
        }
    }
}
