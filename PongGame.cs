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
        private Paddle AiPaddle;
        private Goal goal;
        private Goal goalAi;
        private SpriteFont font;
        Random random;
        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            random = new Random();
            Ball = new Ball(random);
            AiPaddle = new Paddle(new AiPlayer());
            AiPaddle.Speed = 300;
            Paddle = new Paddle(new Player());
            Paddle.Speed = 300;
            goal = new Goal();
            goalAi = new Goal();
            base.Initialize();
        }

        public int Width => GraphicsDevice.Viewport.Width;
        public int Height => GraphicsDevice.Viewport.Height;
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var texture = Content.Load<Texture2D>("ball2");
            font = Content.Load<SpriteFont>("arial");
            goalAi.SpriteFont = font;
            goalAi.Rectangle = new Rectangle(5,0,1, Height);
            goalAi.Offset = -30;

            goal.Rectangle = new Rectangle(Width-5,0,1,Height);
            goal.SpriteFont = font;
            goal.Offset = 30;
            Ball.Texture2D = texture;
            Ball.Reset(Width, Height);
            var paddle = Content.Load<Texture2D>("paddle");

            AiPaddle.Texture2D = paddle;
            AiPaddle.Position = new Vector2(30, Height /2f) - new Vector2(0, AiPaddle.Texture2D.Height / 2f);

            Paddle.Texture2D = paddle;
            Paddle.Position = new Vector2(Width - 30, Height / 2f) - new Vector2(0, Paddle.Texture2D.Height / 2f);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var viewPort = GraphicsDevice.Viewport.Bounds.Size.ToVector2();
            Paddle.Update(gameTime, viewPort, Ball);
            AiPaddle.Update(gameTime, viewPort, Ball);
            goal.Update(Ball, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            goalAi.Update(Ball, Width, Height);
            Ball.Update(gameTime, viewPort);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            Ball.Draw(_spriteBatch);
            Paddle.Draw(_spriteBatch);
            AiPaddle.Draw(_spriteBatch);
            goal.Draw(_spriteBatch, Width);
            goalAi.Draw(_spriteBatch, Width);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
