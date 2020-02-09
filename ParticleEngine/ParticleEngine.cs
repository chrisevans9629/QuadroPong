using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame
{
    public interface IParticleEngine
    {
        void AddParticles(Vector2 location);
    }
    public class ParticleEngine : IParticleEngine
    {
        private readonly IRandomizer random;
        public Vector2 EmitterLocation { get; set; }
        private readonly List<Particle> particles;
        private readonly List<Texture2D> textures;

        public ParticleEngine(List<Texture2D> textures, IRandomizer random)
        {
            EmitterLocation = Vector2.One;
            this.textures = textures;
            this.particles = new List<Particle>();
            this.random = random;
        }

        public void AddParticles(Vector2 location)
        {
            EmitterLocation = location;
            int total = 20;
            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle());
            }
        }

        public void Update()
        {
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
            //Color color = new Color(
            //    (float)random.NextFloat(),
            //    (float)random.NextFloat(),
            //    (float)random.NextFloat());
            var color = Color.LightGray;

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
}