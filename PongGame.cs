using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using MonoGame.Extended.ViewportAdapters;
using PongGame;

namespace MyGame
{
    public class PongGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //private Ball Ball;
        // private GameTimer gameTimer;
        private ParticleEngine engine;
        private List<Ball> balls = new List<Ball>();
        private List<PongPlayer> players = new List<PongPlayer>();
        private List<Boundary> boundaries = new List<Boundary>();
        private List<PowerUp> powerups = new List<PowerUp>();
        private Ship ship;
        private GuiSystem _guiSystem;
        private PongGui gui;
        private GameResult gameResult;
        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);

            gameResult = new GameResult();

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

            foreach (var name in Enum.GetNames(typeof(Paddles)))
            {
                var pos = (Paddles)Enum.Parse(typeof(Paddles), name);
                if(pos == Paddles.Right)
                {
                    players.Add(new PongPlayer(new Player(), pos));
                }
                else
                {
                    players.Add(new PongPlayer(new AiPlayer(pos == Paddles.Left || pos == Paddles.Right), pos));
                }
            }

            
        }

        private void WindowOnClientSizeChanged(object sender, EventArgs e)
        {
            _guiSystem.ClientSizeChanged();
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            _graphics.ApplyChanges();
            SetPositions();
        }

        private IRandomizer randomizer;
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            randomizer = new Randomizer();
            for (int i = 0; i < 4; i++)
            {
                powerups.Add(new PowerUp(randomizer));
            }
            for (int i = 0; i < 1; i++)
            {
                var gameTimer = new GameTimer();
                gameTimer.EveryNumOfSeconds = 3f;
                balls.Add(new Ball(randomizer, gameTimer));
            }

            base.Initialize();
        }

        private SoundEffect music;
        public Rectangle PowerUpArea => new Rectangle(250, 250, Width - 250, Height - 250);
        public int Width => GraphicsDevice.Viewport.Width;
        public int Height => GraphicsDevice.Viewport.Height;
        protected override void LoadContent()
        {



            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var ballTexture = Content.Load<Texture2D>("ball2");
            var font = Content.Load<SpriteFont>("arial");
            var paddle = Content.Load<Texture2D>("paddle");
            var paddleRot = Content.Load<Texture2D>("paddleRot");

            var offset = -60;
            var goal = Content.Load<Song>("goal");
            var blip = Content.Load<SoundEffect>("blip");

            music = Content.Load<SoundEffect>("retromusic");

            var explosions = Content.Load<SoundEffect>("explosion");
            gameResult.SpriteFont = font;

            engine = new ParticleEngine(new List<Texture2D>() { ballTexture },  randomizer);
            ship = new Ship(engine, randomizer);
            ship.Texture2D = Content.Load<Texture2D>("meatball");
            ship.Position = new Vector2(Width/2f, Height/2f);
            ship.Explosions = explosions;
            ship.Engines = Content.Load<SoundEffect>("shipengines");

            var bullets = new List<Ball>();
            for (int i = 0; i < 4; i++)
            {
                bullets.Add(new Ball(randomizer, new GameTimer()));
            }
            ship.Bullets = bullets;
            foreach (var pongPlayer in players)
            {
                if (pongPlayer.Side)
                    pongPlayer.Load(font, paddle, offset, goal);
                else
                    pongPlayer.Load(font, paddleRot, offset, goal);
                offset += 30;

            }

            var powerUpSound = Content.Load<SoundEffect>("powerup");

            foreach (var powerUp in powerups)
            {
                powerUp.SoundEffect = powerUpSound;
                powerUp.Texture2D = ballTexture;
                powerUp.Reset(PowerUpArea);
            }

            var pew = Content.Load<SoundEffect>("pew");
            foreach (var ball in bullets)
            {
                ball.PewSound = pew;
                ball.BounceSong = blip;
                ball.Texture2D = ballTexture;
                ball.Reset(Width, Height);
                ball.Timer.Font = font;
                ball.SpriteFont = font;
            }
            foreach (var ball in balls)
            {
                ball.PewSound = pew;
                ball.Debug = true;
                ball.BounceSong = blip;
                ball.Texture2D = ballTexture;
                ball.Reset(Width, Height);
                ball.Timer.Font = font;
                ball.SpriteFont = font;
            }

            var boundary = Content.Load<Texture2D>("Boundary");

            foreach (var boundary1 in boundaries)
            {
                boundary1.Texture2D = boundary;
            }

            SetPositions();

            var viewportAdapter = new DefaultViewportAdapter(GraphicsDevice);
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, () => Matrix.Identity);
            var font1 = Content.Load<BitmapFont>("Sensation");
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font1);
            gui = new PongGui();

            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer)
            {
                ActiveScreen = gui.Screen,
            };

            var backSong = music.CreateInstance();
            backSong.IsLooped = true;
            backSong.Volume = 0.5f;
            backSong.Play();

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
            _guiSystem.Update(gameTime);

            if (!gui.IsRunning)
                return;


            gameResult.Update(players);
            

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var b = boundaries[0];
            var viewPort = GraphicsDevice.Viewport.Bounds.Size.ToVector2();

            
            ship.Update(gameTime, balls, Width, Height);
            foreach (var pongPlayer in players)
            {
                pongPlayer.Goal.SoundOn = gui.SoundOn;
                pongPlayer.Update(gameTime, viewPort, balls.Union(ship.Bullets).ToList(), Width, Height, b.Texture2D.Width);

                var score = pongPlayer.Paddle.Score;
                if (score > 0 && pongPlayer.Paddle.Score % 5 == 0 && ship.ShipState == ShipState.Dead && score > ship.Score)
                {
                    ship.Score = score;
                    this.ship.Start();
                }
            }

            foreach (var ball in balls.Union(ship.Bullets))
            {
                if (ball.IsColliding)
                    engine.AddParticles(ball.Position);
                ball.Debug = gui.IsDebugging;
                ball.HasSound = gui.SoundOn;
                ball.Update(gameTime, viewPort);
                ball.Timer.Update(gameTime);
                foreach (var powerUp in powerups)
                {
                    powerUp.Update(ball, PowerUpArea);
                }
            }
            engine.Update();

            foreach (var boundary in boundaries)
            {
                foreach (var ball in balls.Union(ship.Bullets))
                {
                    boundary.Update(ball);
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            ship.Draw(_spriteBatch);

            _guiSystem.Draw(gameTime);

            foreach (var ball in balls)
            {
                ball.Draw(_spriteBatch);
                ball.Timer.Draw(_spriteBatch, Width, Height);

            }
            foreach (var pongPlayer in players)
            {
                pongPlayer.Draw(_spriteBatch, Width);
            }


            foreach (var boundary in boundaries)
            {
                boundary.Draw(_spriteBatch);
            }

            foreach (var powerUp in powerups)
            {
                powerUp.Draw(_spriteBatch);
            }
            engine.Draw(_spriteBatch);
            gameResult.Draw(_spriteBatch, new Vector2(Width/2f, Height/2f));
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
