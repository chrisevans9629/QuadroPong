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

namespace MyGame.Levels
{
    public class RegularPongLevel : Level
    {
        private ParticleEngine? engine;

        LevelState GetState()
        {
            return new LevelState()
            {
                Balls = Balls.Select(p => p.SpriteState).ToList(),
                PongPlayerStates = Players.Select(p => p.State).ToList()
            };
        }

        public List<Ball> Balls { get; } = new List<Ball>();
        public List<PongPlayer> Players { get; } = new List<PongPlayer>();
        private readonly GameResult _gameResult = new GameResult();
        private IRandomizer? _randomizer;

        public bool HasTeams { get; set; }


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

        public override void Initialize()
        {
            PongGame.Width = 1000;
            PongGame.Height = 700;

            _randomizer = new Randomizer();


            for (int i = 0; i < 1; i++)
            {
                var gameTimer = new GameTimer
                {
                    EveryNumOfSeconds = 3f
                };
                var ball = new Ball(_randomizer, gameTimer);
                Balls.Add(ball);
            }
            base.Initialize();
        }


        public override void SaveGame()
        {
            var json = JsonConvert.SerializeObject(GetState());
            File.WriteAllText("game.json", json);
        }

        public LevelState? LoadSave()
        {
            if (File.Exists("game.json"))
            {
                var str = File.ReadAllText("game.json");
                var state = JsonConvert.DeserializeObject<LevelState>(str);
                return state;
            }
            return null;
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
            engine = new ParticleEngine(new List<Texture2D>() { ballTexture }, _randomizer);

            //var state = LoadSave();
            //if(state != null)
            //{
            //    Balls.Clear();
            //    foreach (var spriteState in state.Balls)
            //    {
            //        Balls.Add(new Ball(_randomizer, new GameTimer()){SpriteState = spriteState});
            //    }
            //    Players.Clear();
            //    foreach (var pongPlayer in state.PongPlayerStates)
            //    {
            //        var goal2 = new Goal();
            //        Players.Add(new PongPlayer(
            //            new AiPlayer(true),
            //            pongPlayer.Position,
            //            engine,
            //            pongPlayer.PaddleState.PlayerName,
            //            goal2
            //            ){State = pongPlayer});
            //    }

            //    LoadPlayers(font, paddle, goal, paddleRot, deathSound);
            //    LoadBalls(pew, blip, ballTexture, font);
            //    return;
            //}

            if (HasTeams)
            {
                var goalLeft = new Goal();
                var goalRight = new Goal();
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()), Paddles.Right, engine, PlayerName.PlayerOne, goalRight));
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)), Paddles.Left, engine, PlayerName.PlayerTwo, goalLeft));
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Three)), Paddles.Right, engine, PlayerName.PlayerThree, goalRight));
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Four)), Paddles.Left, engine, PlayerName.PlayerFour, goalLeft));

            }
            else
            {
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()), Paddles.Right, engine, PlayerName.PlayerOne));
                Players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)), Paddles.Left, engine, PlayerName.PlayerTwo));
            }
            LoadPlayers(font, paddle, goal, paddleRot, deathSound);

            LoadBalls(pew, blip, ballTexture, font);



            ResetGame(windowSize.X, windowSize.Y);
            SetPositions(windowSize.X, windowSize.Y);

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
                if (pongPlayer.Side)
                    pongPlayer.Load(font, paddle, offset, goal, death);
                else
                    pongPlayer.Load(font, paddleRot, offset, goal, death);
                offset += 120;
            }
        }
        private void ResetGame(int Width, int Height)
        {
            foreach (var ball in Balls)
            {
                ball.Reset(Width, Height);
            }

            foreach (var pongPlayer in Players)
            {
                pongPlayer.Reset(Width, Height);
            }
            _gameResult.Reset();
        }
        public void SetPositions(int Width, int Height)
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
        private void LoadBalls(SoundEffect pew, SoundEffect blip, Texture2D ballTexture, SpriteFont font)
        {
            foreach (var ball in Balls)
            {
                ball.PewSound = pew;
                ball.BounceSong = blip;
                ball.Texture2D = ballTexture;
                ball.Timer.Font = font;
                ball.SpriteFont = font;
            }
        }
        public override void Update(GameTime gameTime, GameState gameState)
        {


            var Width = gameState.Width;
            var Height = gameState.Height;
            _gameResult.Update(Players);

            var viewPort = gameState.ViewPort.Size.ToVector2();
            foreach (var pongPlayer in Players)
            {
                pongPlayer.Goal.SoundOn = gameState.IsSoundOn;
                pongPlayer.Update(gameTime, viewPort, Balls, Width, Height, 0);
            }

            foreach (var ball in Balls)
            {
                if (ball.IsBallColliding)
                    engine.AddParticles(ball.Position);
                ball.Debug = gameState.IsDebug;
                ball.HasSound = gameState.IsSoundOn;
                ball.Update(gameTime, viewPort);
                ball.Timer.Update(gameTime);
            }
            engine.Update();


            if (_gameResult.Winner != null)
            {
                //mainMenu.Winner = $"{gameResult.Winner.Position} won!";
                ResetGame(Width, Height);

                PongGame.ShowMainMenu();

                //BackToMenu($"{gameResult.Winner.Position} won!");
                //BackToMainMenu();
            }
            base.Update(gameTime, gameState);
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


            engine.Draw(_spriteBatch);
        }


        public RegularPongLevel(IPongGame pongGame) : base(pongGame)
        {
        }
    }
}