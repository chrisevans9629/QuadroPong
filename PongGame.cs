using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.InteropServices;
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
        private GameTimer gameTimer;
        private List<Boundary> boundaries;

        Random random;
        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.PreferredBackBufferWidth = 1000;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            boundaries = new List<Boundary>();
            for (int i = 0; i < 4; i++)
            {
                boundaries.Add(new Boundary());
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            random = new Random();
            gameTimer = new GameTimer();
            gameTimer.CountDuration = 3f;
            Ball = new Ball(random, gameTimer);
            Ball.Speed = 300;
            AiPaddle = new Paddle(new AiPlayer());
            AiPaddle.Speed = 150;
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
            goalAi.Offset = 30;

            goal.Rectangle = new Rectangle(Width-5,0,1,Height);
            goal.SpriteFont = font;
            goal.Offset = -30;
            Ball.Texture2D = texture;
            Ball.Reset(Width, Height);
            var paddle = Content.Load<Texture2D>("paddle");
            var boundary = Content.Load<Texture2D>("Boundary");
            AiPaddle.Texture2D = paddle;
            AiPaddle.Position = new Vector2(30, Height /2f) - new Vector2(0, AiPaddle.Texture2D.Height / 2f);

            Paddle.Texture2D = paddle;
            Paddle.Position = new Vector2(Width - 30, Height / 2f) - new Vector2(0, Paddle.Texture2D.Height / 2f);
            gameTimer.Font = font;

            foreach (var boundary1 in boundaries)
            {
                boundary1.Texture2D = boundary;
            }

            boundaries[0].Position = new Vector2(0);
            boundaries[1].Position = new Vector2(Width-boundaries[1].Texture2D.Width,Height-boundaries[1].Texture2D.Height);
            boundaries[2].Position = new Vector2(0, Height-boundaries[2].Texture2D.Height);
            boundaries[3].Position = new Vector2(Width-boundaries[3].Texture2D.Width,0);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var b = boundaries[0];

            var viewPort = GraphicsDevice.Viewport.Bounds.Size.ToVector2();
            Paddle.Update(gameTime, new Vector2(0, b.Texture2D.Height), viewPort - new Vector2(0,b.Texture2D.Height), Ball);
            AiPaddle.Update(gameTime, new Vector2(0, b.Texture2D.Height), viewPort - new Vector2(0,b.Texture2D.Height), Ball);
            goal.Update(Ball, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            goalAi.Update(Ball, Width, Height);
            Ball.Update(gameTime, viewPort);
            gameTimer.Update(gameTime);
            foreach (var boundary in boundaries)
            {
                boundary.Update(Ball);
            }
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
            gameTimer.Draw(_spriteBatch, Width, Height);

            foreach (var boundary in boundaries)
            {
                boundary.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
