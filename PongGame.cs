using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame
{
    public class PongGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Ball Ball;
        private Paddle Paddle;
        Random random;
        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        private float RandomFloat => (float)(random.NextDouble() * 2) - 1f;
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Ball = new Ball();
            Ball.Speed = 300;
            random = new Random();
            Ball.Acceleration = new Vector2(RandomFloat, RandomFloat);
            Paddle = new Paddle();
            Paddle.Speed = 300;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var texture = Content.Load<Texture2D>("ball2");
            Ball.Texture2D = texture;
            Ball.Position = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f) - new Vector2(0, Ball.Texture2D.Height / 2f);

            Paddle.Texture2D = Content.Load<Texture2D>("paddle");
            Paddle.Position = new Vector2(GraphicsDevice.Viewport.Width-10, GraphicsDevice.Viewport.Height / 2f) - new Vector2(0, Paddle.Texture2D.Height / 2f);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Ball.Update(gameTime, GraphicsDevice.Viewport.Bounds.Size.ToVector2());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            Ball.Draw(_spriteBatch);
            Paddle.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public class Paddle : Sprite
    {
        public void Update(GameTime time, Vector2 viewPortSize)
        {
            var min = Vector2.Zero;
            var max = viewPortSize - EndPoint;

            var newPos = new Vector2(2);

            Position = Vector2.Clamp(newPos, min, max);
        }
    }
}
