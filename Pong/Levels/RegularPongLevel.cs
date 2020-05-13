using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using PongGame;
using PongGame.States;

namespace MyGame.Levels
{
    public class RegularPongLevel : Level
    {
        private readonly IGameStateManager _gameStateManager;

        protected virtual LevelState GetState()
        {
            return new LevelState()
            {
                Balls = Balls.Select(p => p.State).ToList(),
                GameMode = GameMode,
                PongPlayerStates = Players.Select(p => p.State).ToList()
            };
        }
        public ParticleEngine Engine { get; private set; }
        public List<Ball> Balls { get; } = new List<Ball>();
        public List<PongPlayer> Players { get; } = new List<PongPlayer>();
        public GameResult Result { get; } = new GameResult();
        public IRandomizer Randomizer1 { get; private set; }

        //public bool HasTeams { get; set; }
        public RegularPongLevel(
            IPongGame pongGame,
            IGameStateManager gameStateManager) : base(pongGame)
        {
            _gameStateManager = gameStateManager;
        }

        public override void Dispose()
        {
            foreach (var ball in Balls)
            {
                ball?.Dispose();
            }
            foreach (var pongPlayer in Players)
            {
                pongPlayer?.Dispose();
            }

            base.Dispose();
        }

        public override void WindowResized()
        {
            SetPositions(PongGame.Width, PongGame.Height);
            base.WindowResized();
        }

        public virtual void InitializeWindowSize()
        {
            PongGame.Width = 1000;
            PongGame.Height = 700;
        }
        public override void Initialize()
        {
            InitializeWindowSize();
            Randomizer1 = new Randomizer();
            Engine = new ParticleEngine(Randomizer1);
            base.Initialize();
        }


        public override void SaveGame()
        {
            _gameStateManager.SaveGame(GetState());
        }

        public override void LoadSavedGame(IContentManager Content, LevelState state)
        {
            var ballTexture = Content.Load<Texture2D>("ball2");
            var paddle = Content.Load<Texture2D>("paddle");
            var goal = Content.Load<Song>("goal");
            var paddleRot = Content.Load<Texture2D>("paddleRot");
            var deathSound = Content.Load<SoundEffect>("death");
            var pew = Content.Load<SoundEffect>("pew");
            var blip = Content.Load<SoundEffect>("blip");
            var font = Content.Load<SpriteFont>("arial");
            Engine.Load(new List<Texture2D>() { ballTexture });

            Balls.Clear();
            foreach (var spriteState in state.Balls)
            {
                //resets powerup size
                spriteState.SpriteState.Size = Vector2.One;
                
                Balls.Add(new Ball(Randomizer1, new GameTimer())
                {
                    State = spriteState
                });
            }
            Players.Clear();
            for (var index = 0; index < state.PongPlayerStates.Count; index++)
            {
                var pongPlayer = state.PongPlayerStates[index];
//this should fix powerups being stuck
                pongPlayer.PaddleState.HasHoldPaddle = false;
                pongPlayer.PaddleState.IsStunned = false;
                pongPlayer.PaddleState.SpriteState.Size = Vector2.One;

                var controllers = new List<IPlayerController>();

                controllers.Add(new ControllerPlayer((PlayerIndex)index));
                if (index == 0)
                {
                    controllers.Add(new KeyBoardPlayer());
                }

                Players.Add(new PongPlayer(
                    new PlayerOrAi(pongPlayer.Side, controllers.ToArray()),
                    pongPlayer.Position,
                    Engine,
                    pongPlayer.PaddleState.PlayerName,
                    state: pongPlayer));
            }

            LoadPlayers(font, paddle, goal, paddleRot, deathSound);
            LoadBalls(Balls, pew, blip, ballTexture, font);
        }

        public override void LoadContent(IContentManager Content, Point windowSize)
        {
            var ballTexture = Content.Load<Texture2D>("ball2");
            var paddle = Content.Load<Texture2D>("paddle");
            var goal = Content.Load<Song>("goal");
            var paddleRot = Content.Load<Texture2D>("paddleRot");
            var deathSound = Content.Load<SoundEffect>("death");
            var pew = Content.Load<SoundEffect>("pew");
            var blip = Content.Load<SoundEffect>("blip");
            var font = Content.Load<SpriteFont>("arial");
            Engine.Load(new List<Texture2D>() { ballTexture });
            for (int i = 0; i < 1; i++)
            {
                var gameTimer = new GameTimer
                {
                    EveryNumOfSeconds = 3f
                };
                var ball = new Ball(Randomizer1, gameTimer);
                Balls.Add(ball);
            }
            LoadGameMode();
            LoadPlayers(font, paddle, goal, paddleRot, deathSound);

            LoadBalls(Balls, pew, blip, ballTexture, font);



            ResetGame(windowSize.X, windowSize.Y);
            SetPositions(windowSize.X, windowSize.Y);

        }

        protected virtual void LoadGameMode()
        {
            if (GameMode == GameMode.Teams)
            {
                var goalLeft = new Goal(new GoalState());
                var goalRight = new Goal(new GoalState());
                Players.Add(new PongPlayer(
                    new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()),
                    Paddles.Right,
                    Engine,
                    PlayerName.PlayerOne,
                    goalRight, state: new PongPlayerState() { GoalState = goalRight.State }));
                Players.Add(new PongPlayer(
                    new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)),
                    Paddles.Left,
                    Engine,
                    PlayerName.PlayerTwo,
                    goalLeft, state: new PongPlayerState() { GoalState = goalLeft.State }));
                Players.Add(new PongPlayer(
                    new PlayerOrAi(true,
                        new ControllerPlayer(PlayerIndex.Three)),
                    Paddles.Right,
                    Engine,
                    PlayerName.PlayerThree,
                    goalRight, state: new PongPlayerState() { GoalState = goalRight.State }));
                Players.Add(new PongPlayer(
                    new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Four)),
                    Paddles.Left,
                    Engine,
                    PlayerName.PlayerFour,
                    goalLeft, state: new PongPlayerState() { GoalState = goalLeft.State }));
            }
            else if (GameMode == GameMode.Classic)
            {
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()),
                    Paddles.Right, Engine, PlayerName.PlayerOne));
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)), Paddles.Left, Engine,
                    PlayerName.PlayerTwo));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void LoadPlayers(
            SpriteFont font,
            Texture2D paddle,
            Song goal,
            Texture2D paddleRot,
            SoundEffect death)
        {
            var offset = -60;

            foreach (var pongPlayer in Players)
            {
                if (pongPlayer.State.Side)
                    pongPlayer.Load(font, paddle, offset, goal, death);
                else
                    pongPlayer.Load(font, paddleRot, offset, goal, death);
                offset += 120;
            }
        }
        protected virtual void ResetGame(int Width, int Height)
        {
            foreach (var ball in Balls)
            {
                ball.Reset(Width, Height);
            }

            foreach (var pongPlayer in Players)
            {
                pongPlayer.Reset(Width, Height);
            }
            Result.Reset();
        }
        public virtual void SetPositions(int Width, int Height)
        {
            var offset = 40;
            var lefts = Players.Where(p => p.State.Position == Paddles.Left);
            var i = 0;
            foreach (var pongPlayer in lefts)
            {
                pongPlayer.SetPosition(Width, Height, i);
                i += offset;
            }

            var rights = Players.Where(p => p.State.Position == Paddles.Right);
            i = 0;
            foreach (var pongPlayer in rights)
            {
                pongPlayer.SetPosition(Width, Height, i);
                i += offset;
            }
        }
        protected virtual void LoadBalls(
            IEnumerable<Ball> balls, 
            SoundEffect pew, 
            SoundEffect blip, 
            Texture2D ballTexture, 
            SpriteFont font)
        {
            foreach (var ball in balls)
            {
                ball.Load(pew, blip, ballTexture, font);
            }
        }
        public override void Update(GameTime gameTime, GameState gameState)
        {
            var Width = gameState.Width;
            var Height = gameState.Height;
            Result.Update(Players);

            var viewPort = gameState.ViewPort.Size.ToVector2();
            UpdatePlayers(gameTime, gameState, viewPort, Players, Balls);

            UpdateBalls(gameTime, gameState, viewPort, Balls);
            Engine.Update();


            if (Result.Winner != null)
            {
                ResetGame(Width, Height);
                PongGame.ShowMainMenu();
            }
            base.Update(gameTime, gameState);
        }

        protected virtual void UpdateBalls(
            GameTime gameTime,
            GameState gameState,
            Vector2 viewPort, IEnumerable<Ball> balls)
        {
            foreach (var ball in balls)
            {
                if (ball.IsBallColliding)
                    Engine.AddParticles(ball.Position);
                ball.Debug = gameState.IsDebug;
                ball.HasSound = gameState.IsSoundOn;
                ball.Update(gameTime, viewPort);
                ball.Timer.Update(gameTime);
            }
        }

        protected virtual void UpdatePlayers(
            GameTime gameTime,
            GameState gameState,
            Vector2 viewPort,
            IEnumerable<PongPlayer> players,
            List<Ball> balls)
        {
            foreach (var pongPlayer in players)
            {
                pongPlayer.Goal.SoundOn = gameState.IsSoundOn;
                pongPlayer.Update(gameTime, viewPort, balls, gameState.Width, gameState.Height);
            }
        }

        public override void Draw(SpriteBatch _spriteBatch, GameTime gameTime, Point window)
        {
            var Width = window.X;
            var Height = window.Y;

            foreach (var ball in Balls)
            {
                ball.Draw(_spriteBatch);
                ball.Timer.Draw(_spriteBatch, Width, Height);

            }
            foreach (var pongPlayer in Players)
            {
                pongPlayer.Draw(_spriteBatch, Width);
            }


            Engine.Draw(_spriteBatch);
        }



    }
}