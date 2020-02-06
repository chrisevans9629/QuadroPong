using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    public class PongGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Ball Ball;
        private SpriteFont font;
        private GameTimer gameTimer;
        private List<PongPlayer> players = new List<PongPlayer>();
        private List<Boundary> boundaries = new List<Boundary>();

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
            for (int i = 0; i < 4; i++)
            {
                boundaries.Add(new Boundary());
            }

            foreach (var name in Enum.GetNames(typeof(Position)))
            {
                var pos = (Position)Enum.Parse(typeof(Position), name);
                players.Add(new PongPlayer(new AiPlayer(pos == Position.Left || pos == Position.Right), pos));
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

            base.Initialize();
        }

        public int Width => GraphicsDevice.Viewport.Width;
        public int Height => GraphicsDevice.Viewport.Height;
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var texture = Content.Load<Texture2D>("ball2");
            font = Content.Load<SpriteFont>("arial");
            var paddle = Content.Load<Texture2D>("paddle");
            var paddleRot = Content.Load<Texture2D>("paddleRot");

            var offset = -60;
            var goal = Content.Load<Song>("goal");
            var blip = Content.Load<SoundEffect>("blip");
            foreach (var pongPlayer in players)
            {
                if (pongPlayer.Side)
                    pongPlayer.Load(font, paddle, offset,goal);
                else
                    pongPlayer.Load(font, paddleRot, offset,goal);
                offset += 30;
            }

            Ball.BounceSong = blip;
            Ball.Texture2D = texture;
            Ball.Reset(Width, Height);
            var boundary = Content.Load<Texture2D>("Boundary");
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

            foreach (var pongPlayer in players)
            {
                pongPlayer.SetPosition(Width, Height);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var b = boundaries[0];

            var viewPort = GraphicsDevice.Viewport.Bounds.Size.ToVector2();

            foreach (var pongPlayer in players)
            {
                pongPlayer.Update(gameTime, viewPort, Ball, Width, Height, b.Texture2D.Width);
            }
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
            foreach (var pongPlayer in players)
            {
                pongPlayer.Draw(_spriteBatch, Width);
            }
            
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
