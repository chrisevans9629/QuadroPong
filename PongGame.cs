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

        private Goal goalTop;
        private Paddle paddleTop;
        private Goal goalBottom;
        private Paddle paddleBottom;

        Random random;
        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            //_graphics.IsFullScreen = true;
            _graphics.PreferMultiSampling = true;

          

            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.PreferredBackBufferWidth = 1000;

            this.Window.AllowUserResizing = true;
            Window.ClientSizeChanged += WindowOnClientSizeChanged;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            boundaries = new List<Boundary>();
            for (int i = 0; i < 4; i++)
            {
                boundaries.Add(new Boundary());
            }
        }

        private void WindowOnClientSizeChanged(object sender, EventArgs e)
        {
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            _graphics.ApplyChanges();
            SetPositions();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            random = new Random();
            gameTimer = new GameTimer();
            gameTimer.CountDuration = 3f;
            Ball = new Ball(random, gameTimer);
            Ball.Speed = 300;
            AiPaddle = new Paddle(new AiPlayer(true));
            AiPaddle.Speed = 300;
            Paddle = new Paddle(new AiPlayer(true));
            Paddle.Speed = 300;
            goal = new Goal();
            goalAi = new Goal();

            goalTop = new Goal();
            goalBottom = new Goal();
            paddleTop = new Paddle(new AiPlayer(false));
            paddleBottom = new Paddle(new AiPlayer(false));
            paddleTop.Speed = 300;
            paddleBottom.Speed = 300;

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
            goalAi.Offset = 30;

            goal.SpriteFont = font;
            goal.Offset = -30;

            goalTop.SpriteFont = font;
            goalBottom.SpriteFont = font;

            goalTop.Offset = -60;
            goalBottom.Offset = 60;

            


            Ball.Texture2D = texture;
            Ball.Reset(Width, Height);
            var paddle = Content.Load<Texture2D>("paddle");
            var boundary = Content.Load<Texture2D>("Boundary");
            AiPaddle.Texture2D = paddle;

            var paddleRot = Content.Load<Texture2D>("paddleRot");

            paddleTop.Texture2D = paddleRot;

            paddleBottom.Texture2D = paddleRot;
            




            Paddle.Texture2D = paddle;
            gameTimer.Font = font;

            foreach (var boundary1 in boundaries)
            {
                boundary1.Texture2D = boundary;
            }

            
            SetPositions();
            // TODO: use this.Content to load your game content here
        }

        public void SetPositions()
        {
            boundaries[0].Position = new Vector2(0);
            boundaries[1].Position = new Vector2(Width - boundaries[1].Texture2D.Width, Height - boundaries[1].Texture2D.Height);
            boundaries[2].Position = new Vector2(0, Height - boundaries[2].Texture2D.Height);
            boundaries[3].Position = new Vector2(Width - boundaries[3].Texture2D.Width, 0);
            paddleBottom.Position = new Vector2(Width / 2f, Height - 30) - new Vector2(paddleBottom.Texture2D.Width / 2f, 0);
            Paddle.Position = new Vector2(Width - 30, Height / 2f) - new Vector2(0, Paddle.Texture2D.Height / 2f);
            paddleTop.Position = new Vector2(Width / 2f, 30) - new Vector2(paddleTop.Texture2D.Width / 2f, 0);
            AiPaddle.Position = new Vector2(30, Height / 2f) - new Vector2(0, AiPaddle.Texture2D.Height / 2f);

            goalAi.Rectangle = new Rectangle(5, 0, 1, Height);
            goal.Rectangle = new Rectangle(Width - 5, 0, 1, Height);
            goalTop.Rectangle = new Rectangle(0, 5, Width, 1);
            goalBottom.Rectangle = new Rectangle(0, Height - 5, Width, 1);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var b = boundaries[0];

            var viewPort = GraphicsDevice.Viewport.Bounds.Size.ToVector2();

            paddleTop.Update(gameTime, new Vector2(b.Texture2D.Width, 0), viewPort - new Vector2(b.Texture2D.Width,0), Ball);
            paddleBottom.Update(gameTime, new Vector2(b.Texture2D.Width, 0), viewPort - new Vector2(b.Texture2D.Width, 0), Ball);


            Paddle.Update(gameTime, new Vector2(0, b.Texture2D.Height), viewPort - new Vector2(0,b.Texture2D.Height), Ball);
            AiPaddle.Update(gameTime, new Vector2(0, b.Texture2D.Height), viewPort - new Vector2(0,b.Texture2D.Height), Ball);
            goal.Update(Ball, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            goalAi.Update(Ball, Width, Height);
            goalTop.Update(Ball, Width, Height);
            goalBottom.Update(Ball, Width, Height);
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
            paddleTop.Draw(_spriteBatch);
            paddleBottom.Draw(_spriteBatch);
            goal.Draw(_spriteBatch, Width);
            goalAi.Draw(_spriteBatch, Width);
            goalTop.Draw(_spriteBatch, Width);
            goalBottom.Draw(_spriteBatch, Width);
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
