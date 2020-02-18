using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using DryIoc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    public enum PlayerName
    {
        PlayerOne = 1,
        PlayerTwo,
        PlayerThree,
        PlayerFour
    }

    public static class PlayerNameExt
    {
        public static Color ToColor(this PlayerName playerName)
        {
            return playerName switch 
                {
                    PlayerName.PlayerOne => Color.Red,
                    PlayerName.PlayerTwo => Color.Blue,
                    PlayerName.PlayerThree => Color.Green,
                    PlayerName.PlayerFour => Color.Purple,
                    _ => throw new NotImplementedException()
                };
        }
    }

    public class PlayerStatsState
    {
        public Vector2 Position { get; set; }
        public int Score { get; set; }
        public int Health { get; set; }
        public PlayerName PlayerName { get; set; }
    }
    public class PlayerStats
    {
        public SpriteFont? SpriteFont { get; set; }
        public PlayerStatsState State { get; set; } = new PlayerStatsState();


        public void Draw(SpriteBatch spriteBatch)
        {
            var one = new Vector2(0,1);
            spriteBatch.DrawString(SpriteFont, State.PlayerName.ToString(), State.Position, State.PlayerName.ToColor());
            spriteBatch.DrawString(SpriteFont,$"Score: {State.Score}", State.Position + one * 30f,Color.White);
            spriteBatch.DrawString(SpriteFont,$"Health: {State.Health}", State.Position + one * 60f,Color.White);
        }
    }

    public class PongPlayerState
    {
        public PlayerStatsState StatsState { get; set; } = new PlayerStatsState();
        public Paddles Position { get; set; }
        public PaddleState PaddleState { get; set; } = new PaddleState();
        public GoalState GoalState { get; set; } = new GoalState();
    }


    public class PongPlayer : IDisposable
    {
        private Paddles? _statsPosition;
        private float _positionOffset = 0;
        private SoundEffect? death;
        private PongPlayerState _state = new PongPlayerState();

        public PongPlayerState State
        {
            get => _state;
            set
            {
                _state = value;
                this.Paddle.State = value.PaddleState;
                this.Goal.State = value.GoalState;
                this.PlayerStats.State = value.StatsState;
            }
        }
        public PlayerStats PlayerStats { get; set; }

        public Paddle Paddle { get; set; }
        public Goal Goal { get; set; }
        public bool Side => State.Position == Paddles.Left || State.Position == Paddles.Right;
        public PongPlayer(
            IPlayerController player, 
            Paddles position, 
            IParticleEngine particleEngine,
            PlayerName playerName,
            Goal? goal = null,
            Paddles? statsPosition = null)
        {
            _statsPosition = statsPosition;
            State.Position = position;
            Paddle = new Paddle(player, particleEngine) {PlayerName = playerName, Speed = 300};
            this.Goal ??= new Goal();
            PlayerStats = new PlayerStats {State = {PlayerName = playerName}};
        }


        public void Load(SpriteFont font, Texture2D paddle, int goalOffset, Song goalSong, SoundEffect deathSound)
        {
            death = deathSound;
            Paddle.Texture2D = paddle;
            Paddle.SpriteFont = font;
            Goal.SpriteFont = font;
            Goal.Offset = goalOffset;
            Goal.Song = goalSong;
            PlayerStats.SpriteFont = font;
        }

        public void Reset(int width, int height)
        {
            Goal.Died = false;  
            Paddle.Power = 0;
            Paddle.Score = 0;
            Goal.Health = GoalState.DefaultHealth;
            SetPosition(width, height, _positionOffset);
        }

        public void SetPosition(int Width, int Height, float positionOffset)
        {
            var offset = 30;
            var paddleWidth = Paddle.Texture2D?.Width ?? 1f;
            var halfPaddleWidth = new Vector2(paddleWidth / 2f, 0);
            var halfWinWidth = Width / 2f;

            var goalOffset = 5;
            var goalWidth = 1;
            Paddle.Paddles = State. Position;
            Goal.Paddles = State.Position;

            var topPos = new Vector2(0,positionOffset);
            var ballOffset = 30;

            if (State.Position == Paddles.Bottom)
            {
                Paddle.BallLaunchOffset = new Vector2(0,ballOffset);
                Paddle.Position = new Vector2(halfWinWidth, Height - offset) - halfPaddleWidth - topPos;
                Goal.Rectangle = new Rectangle(0, Height - goalOffset, Width, goalWidth);
            }
            else if (State.Position == Paddles.Top)
            {
                Paddle.BallLaunchOffset = new Vector2(0, -ballOffset);
                Paddle.Position = new Vector2(halfWinWidth, offset) - halfPaddleWidth + topPos;
                Goal.Rectangle = new Rectangle(0, goalOffset, Width, goalWidth);
            }
            var sidePos = new Vector2(positionOffset,0);
            var halfPaddleHeight = new Vector2(0, Paddle.Texture2D?.Height ?? 1f / 2f);
            var halfWinHeight = Height / 2f;

            if (State.Position == Paddles.Left)
            {
                Paddle.BallLaunchOffset = new Vector2(-ballOffset, 0);
                Paddle.Position = new Vector2(offset, halfWinHeight) - halfPaddleHeight + sidePos;
                Goal.Rectangle = new Rectangle(goalOffset, 0, goalWidth, Height);
            }
            else if (State.Position == Paddles.Right)
            {
                Paddle.BallLaunchOffset = new Vector2(ballOffset, 0);
                Paddle.Position = new Vector2(Width - offset, halfWinHeight) - halfPaddleHeight - sidePos;
                Goal.Rectangle = new Rectangle(Width - goalOffset, 0, goalWidth, Height);
            }

            SetStatsPosition(Width, Height);
        }

        private void SetStatsPosition(int width, int height)
        {
            var offset = 100;
            _statsPosition ??= State.Position;
            var pos = _statsPosition switch
                {
                    Paddles.Left => Vector2.One * 20,
                    Paddles.Top => new Vector2(width - offset,20),
                    Paddles.Right => new Vector2(width-offset, height-offset),
                    Paddles.Bottom => new Vector2(20, height-offset)
                };
            PlayerStats.State.Position = pos;
        }

      

        public void Update(GameTime gameTime, Vector2 viewPort, List<Ball> balls, int width, int height, int boundarySize)
        {
            PlayerStats.State.Health = Goal.Health;
            PlayerStats.State.Score = Paddle.Score;
            if (Goal.Died)
                return;

            Goal.Update(balls, width, height);

            if (Goal.Health <= 0 && !Goal.Died)
            {
                death?.Play();
                Goal.Died = true;
            }

            

            var ball = balls.OrderBy(p => Vector2.Distance(p.Position, Goal.Rectangle.Center)).First();

            var width4 = width / 4f;
            var height4 = height / 4f;


            if (this.State.Position == Paddles.Left)
            {
                Paddle.Update(gameTime, new Vector2(0, boundarySize), viewPort - new Vector2(width4 * 3, boundarySize), ball, width, height);
            }
            else if (State.Position == Paddles.Right)
            {
                Paddle.Update(gameTime, new Vector2(width4 * 3, boundarySize), viewPort - new Vector2(0, boundarySize), ball, width, height);
            }
            else if (State.Position == Paddles.Top)
            {
                Paddle.Update(gameTime, new Vector2(boundarySize, 0), viewPort - new Vector2(boundarySize, height4 * 3), ball, width, height);
            }
            else
            {
                Paddle.Update(gameTime, new Vector2(boundarySize, height4 * 3), viewPort - new Vector2(boundarySize, 0), ball, width, height);
            }
        }

        public void Draw(SpriteBatch spriteBatch, int width)
        {
            Paddle.Draw(spriteBatch);
            Goal.Draw(spriteBatch, width);
            PlayerStats.Draw(spriteBatch);
        }

        public void Dispose()
        {
            death?.Dispose();
            Paddle?.Dispose();
            Goal?.Dispose();
        }
    }
}