using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyGame
{
    public class Sprite
    {
        public Vector2 Position { get; set; }

        public Vector2 Center => Position - new Vector2(0, Texture2D.Height / 2f);

        public Texture2D Texture2D { get; set; }
        public Vector2 Size { get; set; } = new Vector2(1);
        public float Speed { get; set; } = 0;
        public Vector2 Acceleration { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture2D, Position,null, Color,0,Vector2.Zero, Size, SpriteEffects.None, 0);
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Sprite Ball;
        Random random ;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
        }

        private float RandomFloat => (float)(random.NextDouble() * 2) - 1f;
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Ball = new Sprite();
            Ball.Speed = 1;
            Ball.Size = new Vector2(0.5f);
            random = new Random();
            Ball.Acceleration = new Vector2(RandomFloat * 40f,RandomFloat * 40f);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var texture = Content.Load<Texture2D>("ball");
            Ball.Texture2D = texture;
            Ball.Position = new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f) - new Vector2(0, Ball.Texture2D.Height / 2f);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            Ball.Acceleration.Normalize();
            Ball.Position = Vector2.Clamp( Ball.Position + Ball.Acceleration * (float)(Ball.Speed * gameTime.ElapsedGameTime.TotalSeconds),Vector2.Zero, GraphicsDevice.Viewport.Bounds.Size.ToVector2());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            Ball.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
