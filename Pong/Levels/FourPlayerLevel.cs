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
        private ServerClient? _serverClient;
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
            if (GameHosting != GameHosting.Offline)
            {
                _serverClient = new ServerClient();
                _serverClient.Start().GetAwaiter().GetResult();
            }
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
                if (GameHosting != GameHosting.Offline)
                {
                    Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()), Paddles.Right, Engine, PlayerName.PlayerOne));
                    Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)), Paddles.Left, Engine, PlayerName.PlayerTwo));
                    Players.Add(new PongPlayer(new PlayerOrAi(false, new ControllerPlayer(PlayerIndex.Three)), Paddles.Top, Engine, PlayerName.PlayerThree));
                    Players.Add(new PongPlayer(new PlayerOrAi(false, new ControllerPlayer(PlayerIndex.Four)), Paddles.Bottom, Engine, PlayerName.PlayerFour));
                }
                else
                {
                    Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()), Paddles.Right, Engine, PlayerName.PlayerOne));
                    Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)), Paddles.Left, Engine, PlayerName.PlayerTwo));
                    Players.Add(new PongPlayer(new PlayerOrAi(false, new ControllerPlayer(PlayerIndex.Three)), Paddles.Top, Engine, PlayerName.PlayerThree));
                    Players.Add(new PongPlayer(new PlayerOrAi(false, new ControllerPlayer(PlayerIndex.Four)), Paddles.Bottom, Engine, PlayerName.PlayerFour));

                }
            }
            else
            {
                base.LoadGameMode();
            }
        }

        protected override LevelState GetState()
        {
            var result = base.GetState();
            result.ShipState = _ship?.State;
            result.PowerUps = powerups.Select(p => p.State).ToList();
            result.Astroids = _astroidManager.Sprites.Select(p => p.SpriteState).ToList();
            result.Boundaries = boundaries.Select(p => p.SpriteState).ToList();
            return result;
        }

        public void UpdateLevelState(LevelState state)
        {
            GameMode = state.GameMode;
            for (var index = 0; index < state.Balls.Count; index++)
            {
                var spriteState = state.Balls[index];
                Balls[index].SpriteState = spriteState;
            }
            for (var index = 0; index < state.PongPlayerStates.Count; index++)
            {
                var pongPlayer = state.PongPlayerStates[index];
                Players[index].State = pongPlayer;
                Players[index].PlayerStats.State = pongPlayer.StatsState;
                Players[index].Goal.State = pongPlayer.GoalState;
                Players[index].Paddle.State = pongPlayer.PaddleState;
                Players[index].Paddle.SpriteState = pongPlayer.PaddleState.SpriteState;
            }

            for (var index = 0; index < state.Astroids.Count; index++)
            {
                var stateAstroid = state.Astroids[index];
                _astroidManager.Sprites[index].SpriteState = stateAstroid;
            }

            for (var index = 0; index < state.Boundaries.Count; index++)
            {
                var stateBoundary = state.Boundaries[index];
                boundaries[index].SpriteState = stateBoundary;
            }

            for (var index = 0; index < state.PowerUps.Count; index++)
            {
                var statePowerUp = state.PowerUps[index];
                powerups[index].State = statePowerUp;
                powerups[index].SpriteState = statePowerUp.SpriteState;
            }

            if (state.ShipState != null)
            {
                _ship.State = state.ShipState;
                _ship.SpriteState = state.ShipState.SpriteState;
            }
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
            GameMode = state.GameMode;

            _astroidManager.Sprites.Clear();
            foreach (var stateAstroid in state.Astroids)
            {
                _astroidManager.Sprites.Add(new Astroid(randomizer) { SpriteState = stateAstroid, Texture2D = astroid });
            }
            boundaries.Clear();
            foreach (var stateBoundary in state.Boundaries)
            {
                boundaries.Add(new Boundary() { SpriteState = stateBoundary });
            }
            powerups.Clear();
            foreach (var statePowerUp in state.PowerUps)
            {
                powerups.Add(new PowerUp(randomizer, _powerupManager, statePowerUp));
            }
            if (state.ShipState != null)
            {
                //this should reset the size
                state.ShipState.Balls.ForEach(p => p.Size = Vector2.One);
                _ship = new Ship(Engine, randomizer, state.ShipState);
                _ship?.Load(MEAT, new Point(PongGame.Width, PongGame.Height), explosions, engineSound, pew, blip, ballTexture, font);
            }
            LoadPowerUps(powerUpSound, ballTexture);
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

            if (GameHosting != GameHosting.Offline)
            {
                if (GameHosting == GameHosting.Client)
                {
                    base.UpdatePlayers(gameTime, gameState, viewPort, players.Where(p=> _serverClient.PlayerName == p.State.StatsState.PlayerName), balls.Union(_ship.Bullets).ToList());
                }

                if (GameHosting == GameHosting.Host)
                {
                    base.UpdatePlayers(gameTime, gameState, viewPort, players.Where(p => _serverClient.Players.Contains(p.State.StatsState.PlayerName) != true), balls.Union(_ship.Bullets).ToList());
                }
            }
            else
            {
                base.UpdatePlayers(gameTime, gameState, viewPort, players, balls.Union(_ship.Bullets).ToList());
            }

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

            if (GameHosting == GameHosting.Client)
            {
                if (_serverClient.LevelStates != null)
                {
                    UpdateLevelState(_serverClient.LevelStates);
                    _serverClient.LevelStates = null;
                }
                //return;
            }
            else if (GameHosting == GameHosting.Host)
            {
                _serverClient?.SendMove(GetState()).GetAwaiter().GetResult();
            }

            if (_settings.HasAstroids)
                _astroidManager?.Update(gameTime, Balls.Union(_ship.Bullets).ToList(), width, height);

            _powerupManager?.UpdateTimedPowerup(gameTime);

            _ship?.Update(gameTime, Balls, width, height, gameState.IsSoundOn);

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