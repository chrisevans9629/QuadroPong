using System;
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
        private Goal goal;
        private SpriteFont font;
        Random random;
        public PongGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            random = new Random();
            Ball = new Ball(random);
            Paddle = new Paddle();
            Paddle.Speed = 300;
            goal = new Goal();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var texture = Content.Load<Texture2D>("ball2");
            font = Content.Load<SpriteFont>("arial");
            goal.Rectangle = new Rectangle(GraphicsDevice.Viewport.Width-5,0,1,GraphicsDevice.Viewport.Height);
            Ball.Texture2D = texture;
            Ball.Reset(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            Paddle.Texture2D = Content.Load<Texture2D>("paddle");
            Paddle.Position = new Vector2(GraphicsDevice.Viewport.Width - 30, GraphicsDevice.Viewport.Height / 2f) - new Vector2(0, Paddle.Texture2D.Height / 2f);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var viewPort = GraphicsDevice.Viewport.Bounds.Size.ToVector2();
            Ball.Update(gameTime, viewPort);
            Paddle.Update(gameTime, viewPort, Ball);
            goal.Update(Ball, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            Ball.Draw(_spriteBatch);
            Paddle.Draw(_spriteBatch);

            _spriteBatch.DrawString(font,goal.Score.ToString(), new Vector2(GraphicsDevice.Viewport.Width/2f, GraphicsDevice.Viewport.Y/2f), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public class Goal : Collider
    {
        public Rectangle Rectangle { get; set; }
        public override Rectangle Bounds()
        {
            return Rectangle;
        }

        public void Update(Ball ball, int width, int height)
        {
            if (Collision(ball))
            {
                Score++;
                ball.Reset(width, height);
            }
        }

        public int Score { get; set; }
    }
    
}
