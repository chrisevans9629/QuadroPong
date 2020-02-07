using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame
{
    public class ParticleEngine
    {
        private IRandomizer random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private List<Texture2D> textures;

        public ParticleEngine(List<Texture2D> textures, Vector2 location, IRandomizer random)
        {
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            this.random = random;
        }

        public void Update()
        {
            int total = 10;

            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle());
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        private Particle GenerateNewParticle()
        {
            Texture2D texture = textures[random.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                                    1f * (float)(random.NextFloat() * 2 - 1),
                                    1f * (float)(random.NextFloat() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextFloat() * 2 - 1);
            Color color = new Color(
                        (float)random.NextFloat(),
                        (float)random.NextFloat(),
                        (float)random.NextFloat());
            float size = (float)random.NextFloat();
            int ttl = 20 + random.Next(40);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
        }
    }
    public class Particle
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public int TTL { get; set; }

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size, int ttl)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            TTL = ttl;
        }

        public void Update()
        {
            TTL--;
            Position += Velocity;
            Angle += AngularVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            spriteBatch.Draw(Texture, Position, sourceRectangle, Color,
                Angle, origin, Size, SpriteEffects.None, 0f);
        }
    }



    public abstract class Collider
    {
        public abstract Rectangle Bounds();

        private  List<Sprite> lastCollisions = new List<Sprite>();


        public bool BetweenX(Sprite sprite)
        {
            var bounds = Bounds();
            return sprite.Position.X >= bounds.X && sprite.Position.X + sprite.EndPoint.X <= bounds.X + bounds.Width;
        }

        public bool BetweenY(Sprite sprite)
        {
            var bounds = Bounds();
            return sprite.Position.Y >= bounds.Y && sprite.Position.Y + sprite.EndPoint.Y <= bounds.Y + bounds.Height;
        }

        public Direction OnRelativeSide(Sprite sprite)
        {
            var bounds = Bounds();
            if (BetweenX(sprite))
            {
                if (sprite.Position.Y >= bounds.Y + bounds.Height)
                {
                    return Direction.Bottom;
                }
                else
                {
                    return Direction.Top;
                }
            }
            else if (BetweenY(sprite))
            {
                if (sprite.Position.X + sprite.Texture2D.Width >= bounds.X + bounds.Width)
                {
                    return Direction.Right;
                }
                else
                {
                    return Direction.Left;
                }
            }

            return Direction.Bottom;
        }

        public bool Collision(Sprite sprite)
        {
            // Texture2D.Bounds.Intersects(sprite.Texture2D.Bounds);
            var isColliding = Bounds().Intersects(sprite.Bounds());
            if (isColliding)
            {
                if (lastCollisions.Contains(sprite))
                {
                    return false;
                }

                lastCollisions.Add(sprite);
            }
            else
            {
                lastCollisions.Remove(sprite);
            }

            return isColliding;
        }
    }
}