using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using PongGame;

namespace MyGame.Levels
{
    public class RegularPongLevel : Level
    {
        private ParticleEngine? engine;

        private List<Ball> balls = new List<Ball>();
        private List<PongPlayer> players = new List<PongPlayer>();
        private GameResult gameResult = new GameResult();
        private IRandomizer? randomizer;
        private bool _hasTeams;
        public RegularPongLevel(bool hasTeams = false)
        {
            _hasTeams = hasTeams;
        }

        public override void Dispose()
        {
            foreach (var ball in balls)
            {
                ball?.Dispose();
            }
            foreach (var pongPlayer in players)
            {
                pongPlayer?.Dispose();
            }

            base.Dispose();
        }

        public override void Initialize()
        {
            
            randomizer = new Randomizer();
            for (int i = 0; i < 1; i++)
            {
                var gameTimer = new GameTimer();
                gameTimer.EveryNumOfSeconds = 3f;
                balls.Add(new Ball(randomizer, gameTimer));
            }
            base.Initialize();
        }

        public override void LoadContent(ContentManager Content, Point windowSize)
        {
            var ballTexture = Content.Load<Texture2D>("ball2");
            var paddle = Content.Load<Texture2D>("paddle");
            var goal = Content.Load<Song>("goal");
            var paddleRot = Content.Load<Texture2D>("paddleRot");
            var deathSound = Content.Load<SoundEffect>("death");

            engine = new ParticleEngine(new List<Texture2D>() { ballTexture }, randomizer);

            if (_hasTeams)
            {
                var goalLeft = new Goal();
                var goalRight = new Goal();
                players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()), Paddles.Right, engine, goalRight));
                players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)), Paddles.Left, engine, goalLeft));
                players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Three)), Paddles.Right, engine, goalRight));
                players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Four)), Paddles.Left, engine, goalLeft));

            }
            else
            {
                players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.One), new KeyBoardPlayer()), Paddles.Right, engine));
                players.Add(new PongPlayer(new PlayerOrAi(true, new ControllerPlayer(PlayerIndex.Two)), Paddles.Left, engine));
            }


            var pew = Content.Load<SoundEffect>("pew");
            var blip = Content.Load<SoundEffect>("blip");
            var font = Content.Load<SpriteFont>("arial");
            LoadPlayers(font, paddle, goal, paddleRot, deathSound);

            LoadBalls(pew, blip, ballTexture, font);
            SetPositions(windowSize.X, windowSize.Y);
            ResetGame(windowSize.X, windowSize.Y);
        }
        private void LoadPlayers(SpriteFont font, Texture2D paddle, Song goal, Texture2D paddleRot, SoundEffect death)
        {
            var offset = -60;

            foreach (var pongPlayer in players)
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
            foreach (var ball in balls)
            {
                ball.Reset(Width, Height);
            }
            foreach (var pongPlayer in players)
            {
                pongPlayer.Reset(Width, Height);
            }
            gameResult.Reset();
        }
        public void SetPositions(int Width, int Height)
        {
            foreach (var pongPlayer in players)
            {
                pongPlayer.SetPosition(Width, Height);
            }
        }
        private void LoadBalls(SoundEffect pew, SoundEffect blip, Texture2D ballTexture, SpriteFont font)
        {
            foreach (var ball in balls)
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
            gameResult.Update(players);

            var viewPort = gameState.ViewPort.Size.ToVector2();
            foreach (var pongPlayer in players)
            {
                pongPlayer.Goal.SoundOn = gameState.IsSoundOn;
                pongPlayer.Update(gameTime, viewPort, balls, Width, Height, 0);
            }

            foreach (var ball in balls)
            {
                if (ball.IsColliding)
                    engine.AddParticles(ball.Position);
                ball.Debug = gameState.IsDebug;
                ball.HasSound = gameState.IsSoundOn;
                ball.Update(gameTime, viewPort);
                ball.Timer.Update(gameTime);
            }
            engine.Update();


            if (gameResult.Winner != null)
            {
                //mainMenu.Winner = $"{gameResult.Winner.Position} won!";
                ResetGame(Width, Height);
                BackToMenu($"{gameResult.Winner.Position} won!");
                //BackToMainMenu();
            }
        }

        public override void Draw(SpriteBatch _spriteBatch, GameTime gameTime, Point window)
        {
            var Width = window.X;
            var Height = window.Y;

            foreach (var ball in balls)
            {
                ball.Draw(_spriteBatch);
                ball.Timer.Draw(_spriteBatch, Width, Height);

            }
            foreach (var pongPlayer in players)
            {
                pongPlayer.Draw(_spriteBatch, Width);
            }


            engine.Draw(_spriteBatch);
        }
    }
}