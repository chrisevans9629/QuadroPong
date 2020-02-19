﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using PongGame;
using PongGame.Sprites;
using PongGame.States;

namespace MyGame.Levels
{
    public class FourPlayerLevel : RegularPongLevel
    {
        private readonly ISettings _settings;
        private AstroidManager? _astroidManager;
        private List<Boundary> boundaries = new List<Boundary>();
        private List<PowerUp> powerups = new List<PowerUp>();
        private Ship? _ship;
        private PowerupManager? _powerupManager;
        private IRandomizer randomizer => Randomizer1;
        public Rectangle PowerUpArea(int Width, int Height) => new Rectangle(250, 250, Width - 250, Height - 250);
        public FourPlayerLevel(
            IPongGame pongGame,
            ISettings settings,
            IGameStateManager gameStateManager)
            : base(pongGame, gameStateManager)
        {
            _settings = settings;
        }

        public override void Dispose()
        {
            _astroidManager?.Dispose();
            
            foreach (var boundary in boundaries)
            {
                boundary?.Dispose();
            }
            foreach (var powerUp in powerups)
            {
                powerUp.Dispose();
            }
            _ship?.Dispose();
            base.Dispose();
        }

      

        public override void InitializeWindowSize()
        {
            PongGame.Width = 1000;
            PongGame.Height = 1000;
        }

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < 4; i++)
            {
                boundaries.Add(new Boundary());
            }
            _powerupManager = new PowerupManager();
            _astroidManager = new AstroidManager(randomizer);
            _ship = new Ship(Engine, randomizer);

            for (int i = 0; i < 10; i++)
            {
                powerups.Add(new PowerUp(randomizer, _powerupManager));
            }
        }

        protected override void LoadGameMode()
        {
            if (GameMode == GameMode.PlayerVs)
            {
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()), Paddles.Right, Engine, PlayerName.PlayerOne));
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)), Paddles.Left, Engine, PlayerName.PlayerTwo));
                Players.Add(new PongPlayer(new PlayerOrAi(false, new ControllerPlayer(PlayerIndex.Three)), Paddles.Top, Engine, PlayerName.PlayerThree));
                Players.Add(new PongPlayer(new PlayerOrAi(false, new ControllerPlayer(PlayerIndex.Four)), Paddles.Bottom, Engine, PlayerName.PlayerFour));
            }
            else
            {
                base.LoadGameMode();
            }
        }

        protected override LevelState GetState()
        {
            var result = base.GetState();
            result.ShipState = _ship.State;
            result.PowerUps = powerups.Select(p => p.State).ToList();
            result.Astroids = _astroidManager.Sprites.Select(p => p.SpriteState).ToList();
            result.Boundaries = boundaries.Select(p => p.SpriteState).ToList();
            return result;
        }

        public override void LoadSavedGame(IContentManager Content, LevelState state)
        {
            var font = Content.Load<SpriteFont>("arial");
            var ballTexture = Content.Load<Texture2D>("ball2");
            var blip = Content.Load<SoundEffect>("blip");
            var explosions = Content.Load<SoundEffect>("explosion");
            var MEAT = Content.Load<Texture2D>("meatball");
            var engineSound = Content.Load<SoundEffect>("shipengines");
            var powerUpSound = Content.Load<SoundEffect>("powerup");
            var pew = Content.Load<SoundEffect>("pew");
            var boundary = Content.Load<Texture2D>("Boundary");
            var astroid = Content.Load<Texture2D>("astroids");
            _astroidManager.Sprites.Clear();
            GameMode = state.GameMode;
            foreach (var stateAstroid in state.Astroids)
            {
                _astroidManager.Sprites.Add(new Astroid(randomizer){SpriteState = stateAstroid, Texture2D = astroid});
            }
            boundaries.Clear();
            foreach (var stateBoundary in state.Boundaries)
            {
                boundaries.Add(new Boundary(){SpriteState = stateBoundary});
            }
            powerups.Clear();
            foreach (var statePowerUp in state.PowerUps)
            {
                powerups.Add(new PowerUp(randomizer, _powerupManager, statePowerUp));
            }
            if (state.ShipState != null)
            {
                _ship = new Ship(Engine, randomizer, state.ShipState);
                _ship?.Load(MEAT, new Point(PongGame.Width,PongGame.Height), explosions, engineSound, pew, blip, ballTexture, font);
            }
            LoadPowerUps(powerUpSound,ballTexture);
            LoadBoundaries(boundary);
            base.LoadSavedGame(Content, state);
        }
        public override void LoadContent(IContentManager Content, Point windowSize)
        {

            var font = Content.Load<SpriteFont>("arial");
            var ballTexture = Content.Load<Texture2D>("ball2");
            var blip = Content.Load<SoundEffect>("blip");
            var explosions = Content.Load<SoundEffect>("explosion");
            var MEAT = Content.Load<Texture2D>("meatball");
            var engineSound = Content.Load<SoundEffect>("shipengines");
            var powerUpSound = Content.Load<SoundEffect>("powerup");
            var pew = Content.Load<SoundEffect>("pew");
            var boundary = Content.Load<Texture2D>("Boundary");
            var astroid = Content.Load<Texture2D>("astroids");

            _astroidManager?.Load(astroid);

            _ship?.Load(MEAT, windowSize, explosions, engineSound, pew, blip, ballTexture, font);

            //var bullets = LoadShip(MEAT, explosions, engineSound, windowSize);
            //LoadBalls(bullets, pew, blip, ballTexture, font);
            LoadPowerUps(powerUpSound, ballTexture);
            LoadBoundaries(boundary);
            base.LoadContent(Content, windowSize);

        }

        protected override void UpdateBalls(GameTime gameTime, GameState gameState, Vector2 viewPort, IEnumerable<Ball> balls)
        {
            base.UpdateBalls(gameTime, gameState, viewPort, balls.Union(_ship.Bullets));
        }

        protected override void UpdatePlayers(GameTime gameTime, GameState gameState, Vector2 viewPort, IEnumerable<PongPlayer> players, List<Ball> balls)
        {
            var b = boundaries[0];
            foreach (var pongPlayer in players)
            {
                pongPlayer.BoundarySize = b.Texture2D.Width;
            }
            base.UpdatePlayers(gameTime, gameState, viewPort, players, balls.Union(_ship.Bullets).ToList());

            foreach (var pongPlayer in players)
            {
                var score = pongPlayer.Paddle.Score;

                if (score > 0 && pongPlayer.Paddle.Score % 3 == 0 && _ship.ShipStatus == ShipStatus.Dead && score > _ship.Score)
                {
                    _ship.Score = score;
                    this._ship.Start();
                }
            }
        }

        public override void Update(GameTime gameTime, GameState gameState)
        {
            var width = gameState.Width;
            var height = gameState.Height;
            if (_settings.HasAstroids)
                _astroidManager.Update(gameTime, Balls.Union(_ship.Bullets).ToList(), width, height);

            _powerupManager.UpdateTimedPowerup(gameTime);
           
            _ship.Update(gameTime, Balls, width, height, gameState.IsSoundOn);
           
            foreach (var powerUp in powerups)
            {
                powerUp.Update(Balls.Union(_ship.Bullets), PowerUpArea(width, height), gameState.IsSoundOn);
            }

            foreach (var boundary in boundaries)
            {
                foreach (var ball in Balls.Union(_ship.Bullets))
                {
                    boundary.Update(ball);
                }
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
        //private List<Ball> LoadShip(Texture2D MEAT, SoundEffect explosions, SoundEffect engineSound, Point window)
        //{
        //}
       
        private void LoadPowerUps(SoundEffect powerUpSound, Texture2D ballTexture)
        {
            foreach (var powerUp in powerups)
            {
                powerUp.SoundEffect = powerUpSound;
                powerUp.Texture2D = ballTexture;
            }
        }
        
        protected override void ResetGame(int Width, int Height)
        {
            foreach (var shipBullet in _ship.Bullets ?? new List<Ball>())
            {
                shipBullet.Reset(Width, Height);
            }

            foreach (var powerUp in powerups)
            {
                powerUp.Reset(PowerUpArea(Width, Height));
            }

            _ship.Reset();
            base.ResetGame(Width, Height);
        }

        public override void WindowResized()
        {
            SetPositions(PongGame.Width, PongGame.Height);
            base.WindowResized();
        }

        public override void SetPositions(int Width, int Height)
        {
            boundaries[0].Position = new Vector2(0);
            boundaries[1].Position = new Vector2(Width - boundaries[1].Texture2D.Width, Height - boundaries[1].Texture2D.Height);
            boundaries[2].Position = new Vector2(0, Height - boundaries[2].Texture2D.Height);
            boundaries[3].Position = new Vector2(Width - boundaries[3].Texture2D.Width, 0);

            foreach (var pongPlayer in Players)
            {
                pongPlayer.SetPosition(Width, Height, 0);
            }
        }

        public override void Draw(SpriteBatch _spriteBatch, GameTime gameTime, Point window)
        {
            _ship.Draw(_spriteBatch);

            foreach (var boundary in boundaries)
            {
                boundary.Draw(_spriteBatch);
            }

            foreach (var powerUp in powerups)
            {
                powerUp.Draw(_spriteBatch);
            }
            if (_settings.HasAstroids)
                _astroidManager.Draw(_spriteBatch);
            base.Draw(_spriteBatch, gameTime, window);
        }

        
    }
}