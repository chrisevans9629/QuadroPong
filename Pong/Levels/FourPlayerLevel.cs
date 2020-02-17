using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using PongGame;
using PongGame.Sprites;

namespace MyGame.Levels
{
    public class FourPlayerLevel : Level
    {
        private readonly ISettings _settings;
        private AstroidManager astroidManager;
        private ParticleEngine engine;
        private List<Ball> balls = new List<Ball>();
        private List<PongPlayer> players = new List<PongPlayer>();
        private List<Boundary> boundaries = new List<Boundary>();
        private List<PowerUp> powerups = new List<PowerUp>();
        private Ship ship;
        private GameResult gameResult = new GameResult();
        private PowerupManager manager;
        private IRandomizer randomizer;
        public Rectangle PowerUpArea(int Width, int Height) => new Rectangle(250, 250, Width - 250, Height - 250);


        public override void Dispose()
        {
            astroidManager?.Dispose();
            engine?.Dispose();
            foreach (var ball in balls)
            {
                ball?.Dispose();
            }
            foreach (var pongPlayer in this.players)
            {
                pongPlayer?.Dispose();
            }
            foreach (var boundary in boundaries)
            {
                boundary?.Dispose();
            }
            foreach (var powerUp in powerups)
            {
                powerUp.Dispose();
            }
            ship.Dispose();
            base.Dispose();
        }

        public override void Initialize()
        {
            PongGame.Width = 1000;
            PongGame.Height = 1000;
            for (int i = 0; i < 4; i++)
            {
                boundaries.Add(new Boundary());
            }
            // TODO: Add your initialization logic here
            manager = new PowerupManager();
            randomizer = new Randomizer();
            astroidManager = new AstroidManager(randomizer);

            for (int i = 0; i < 10; i++)
            {
                powerups.Add(new PowerUp(randomizer, manager));
            }
            for (int i = 0; i < 1; i++)
            {
                var gameTimer = new GameTimer();
                gameTimer.EveryNumOfSeconds = 3f;
                balls.Add(new Ball(randomizer, gameTimer));
            }
        }

        public override void LoadContent(IContentManager Content, Point windowSize)
        {
            var font = Content.Load<SpriteFont>("arial");

            var ballTexture = Content.Load<Texture2D>("ball2");
            var paddle = Content.Load<Texture2D>("paddle");
            var paddleRot = Content.Load<Texture2D>("paddleRot");
            var goal = Content.Load<Song>("goal");
            var blip = Content.Load<SoundEffect>("blip");
            var explosions = Content.Load<SoundEffect>("explosion");
            var MEAT = Content.Load<Texture2D>("meatball");
            var engineSound = Content.Load<SoundEffect>("shipengines");
            var powerUpSound = Content.Load<SoundEffect>("powerup");
            var pew = Content.Load<SoundEffect>("pew");
            var boundary = Content.Load<Texture2D>("Boundary");
            var deathSound = Content.Load<SoundEffect>("death");
            astroidManager.Load(Content.Load<Texture2D>("astroids"));
            engine = new ParticleEngine(new List<Texture2D>() { ballTexture }, randomizer);
            players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()), Paddles.Right, engine, PlayerName.PlayerOne));
            players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)), Paddles.Left, engine, PlayerName.PlayerTwo));
            players.Add(new PongPlayer(new PlayerOrAi(false, new ControllerPlayer(PlayerIndex.Three)), Paddles.Top, engine, PlayerName.PlayerThree));
            players.Add(new PongPlayer(new PlayerOrAi(false, new ControllerPlayer(PlayerIndex.Four)), Paddles.Bottom, engine, PlayerName.PlayerFour));

            var bullets = LoadShip(MEAT, explosions, engineSound, windowSize);

            LoadPlayers(font, paddle, goal, paddleRot, deathSound);

            LoadPowerUps(powerUpSound, ballTexture);

            LoadBalls(bullets, pew, blip, ballTexture, font);

            LoadBoundaries(boundary);

            SetPositions(windowSize.X, windowSize.Y);
            ResetGame(windowSize.X, windowSize.Y);
        }

        public override void Update(GameTime gameTime, GameState gameState)
        {
            var Width = gameState.Width;
            var Height = gameState.Height;
            if (_settings.HasAstroids)
                astroidManager.Update(gameTime, balls.Union(ship.Bullets).ToList(), Width, Height);

            manager.UpdateTimedPowerup(gameTime);
            gameResult.Update(players);

            var b = boundaries[0];
            var viewPort = gameState.ViewPort.Size.ToVector2();

            ship.Update(gameTime, balls, Width, Height, gameState.IsSoundOn);
            foreach (var pongPlayer in players)
            {
                pongPlayer.Goal.SoundOn = gameState.IsSoundOn;
                pongPlayer.Update(gameTime, viewPort, balls.Union(ship.Bullets).ToList(), Width, Height, b.Texture2D.Width);

                var score = pongPlayer.Paddle.Score;
                if (score > 0 && pongPlayer.Paddle.Score % 3 == 0 && ship.ShipState == ShipState.Dead && score > ship.Score)
                {
                    ship.Score = score;
                    this.ship.Start();
                }
            }

            foreach (var ball in balls.Union(ship.Bullets))
            {
                if (ball.IsBallColliding)
                    engine.AddParticles(ball.Position);
                ball.Debug = gameState.IsDebug;
                ball.HasSound = gameState.IsSoundOn;
                ball.Update(gameTime, viewPort);
                ball.Timer.Update(gameTime);
                foreach (var powerUp in powerups)
                {
                    powerUp.Update(ball, PowerUpArea(Width, Height), gameState.IsSoundOn);
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

            if (gameResult.Winner != null)
            {
                //mainMenu.Winner = $"{gameResult.Winner.Position} won!";
                ResetGame(Width, Height);
                //todo: make gui to show winner
                PongGame.ShowMainMenu();
                //BackToMenu($"{gameResult.Winner.Position} won!");
                //BackToMainMenu();
            }
            base.Update(gameTime, gameState);
        }



        private void LoadBoundaries(Texture2D boundary)
        {
            foreach (var boundary1 in boundaries)
            {
                boundary1.Texture2D = boundary;
            }
        }
        private List<Ball> LoadShip(Texture2D MEAT, SoundEffect explosions, SoundEffect engineSound, Point window)
        {
            ship = new Ship(engine, randomizer);
            ship.Texture2D = MEAT;
            ship.Position = new Vector2(window.X / 2f, window.Y / 2f);
            ship.Explosions = explosions;
            ship.Engines = engineSound;

            var bullets = new List<Ball>();
            for (int i = 0; i < 4; i++)
            {
                bullets.Add(new Ball(randomizer, new GameTimer()));
            }

            ship.Bullets = bullets;
            return bullets;
        }
        private void LoadPlayers(SpriteFont font, Texture2D paddle, Song goal, Texture2D paddleRot, SoundEffect death)
        {
            var offset = -240;

            foreach (var pongPlayer in players)
            {
                if (pongPlayer.Side)
                    pongPlayer.Load(font, paddle, offset, goal, death);
                else
                    pongPlayer.Load(font, paddleRot, offset, goal, death);
                offset += 120;
            }
        }
        private void LoadPowerUps(SoundEffect powerUpSound, Texture2D ballTexture)
        {
            foreach (var powerUp in powerups)
            {
                powerUp.SoundEffect = powerUpSound;
                powerUp.Texture2D = ballTexture;
            }
        }
        private void LoadBalls(List<Ball> bullets, SoundEffect pew, SoundEffect blip, Texture2D ballTexture, SpriteFont font)
        {
            foreach (var ball in bullets)
            {
                ball.PewSound = pew;
                ball.BounceSong = blip;
                ball.Texture2D = ballTexture;
                ball.Timer.Font = font;
                ball.SpriteFont = font;
            }

            foreach (var ball in balls)
            {
                ball.PewSound = pew;
                ball.BounceSong = blip;
                ball.Texture2D = ballTexture;
                ball.Timer.Font = font;
                ball.SpriteFont = font;
            }
        }
        private void ResetGame(int Width, int Height)
        {
            foreach (var ball in balls)
            {
                ball.Reset(Width, Height);
            }

            foreach (var shipBullet in ship.Bullets ?? new List<Ball>())
            {
                shipBullet.Reset(Width, Height);
            }

            foreach (var powerUp in powerups)
            {
                powerUp.Reset(PowerUpArea(Width, Height));
            }

            foreach (var pongPlayer in players)
            {
                pongPlayer.Reset(Width, Height);
            }

            ship.Reset();
            gameResult.Reset();
        }

        public override void WindowResized()
        {
            SetPositions(PongGame.Width, PongGame.Height);
            base.WindowResized();
        }

        public void SetPositions(int Width, int Height)
        {
            boundaries[0].Position = new Vector2(0);
            boundaries[1].Position = new Vector2(Width - boundaries[1].Texture2D.Width, Height - boundaries[1].Texture2D.Height);
            boundaries[2].Position = new Vector2(0, Height - boundaries[2].Texture2D.Height);
            boundaries[3].Position = new Vector2(Width - boundaries[3].Texture2D.Width, 0);

            foreach (var pongPlayer in players)
            {
                pongPlayer.SetPosition(Width, Height, 0);
            }
        }

        public override void Draw(SpriteBatch _spriteBatch, GameTime gameTime, Point window)
        {
            var Width = window.X;
            var Height = window.Y;
            ship.Draw(_spriteBatch);


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
            if (_settings.HasAstroids)
                astroidManager.Draw(_spriteBatch);
            engine.Draw(_spriteBatch);
        }

        public FourPlayerLevel(IPongGame pongGame, ISettings settings) : base(pongGame)
        {
            _settings = settings;
        }
    }
}