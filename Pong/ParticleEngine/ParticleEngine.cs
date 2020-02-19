using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame
{
    public class ParticleEngine : IParticleEngine, IDisposable
    {
        private readonly IRandomizer _random;
        public Vector2 EmitterLocation { get; set; }
        private readonly List<Particle> _particles;
        private List<Texture2D> _textures;

        public ParticleEngine(IRandomizer random)
        {
            EmitterLocation = Vector2.One;
            this._particles = new List<Particle>();
            this._random = random;
        }
        public void Load(List<Texture2D> textures)
        {
            this._textures = textures;
        }
        public void AddParticles(Vector2 location)
        {
            EmitterLocation = location;
            int total = 20;
            for (int i = 0; i < total; i++)
            {
                _particles.Add(GenerateNewParticle(new List<Color>(){Color.White}));
            }
        }

        public void AddParticles(Vector2 location, List<Color> colors)
        {
            EmitterLocation = location;
            int total = 20;
            for (int i = 0; i < total; i++)
            {
                _particles.Add(GenerateNewParticle(colors));
            }
        }

        public void Update()
        {
            for (int particle = 0; particle < _particles.Count; particle++)
            {
                _particles[particle].Update();
                if (_particles[particle].TTL <= 0)
                {
                    _particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        private Particle GenerateNewParticle(List<Color> colors)
        {
            Texture2D texture = _textures[_random.Next(_textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                1f * (float)(_random.NextFloat() * 2 - 1),
                1f * (float)(_random.NextFloat() * 2 - 1));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(_random.NextFloat() * 2 - 1);

            var color = colors[_random.Next(colors.Count)];
            //Color color = new Color(
            //    (float)random.NextFloat(),
            //    (float)random.NextFloat(),
            //    (float)random.NextFloat());
            //var color = Color.LightGray;

            float size = (float)_random.NextFloat();
            int ttl = 20 + _random.Next(40);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < _particles.Count; index++)
            {
                _particles[index].Draw(spriteBatch);
            }
        }

        public void Dispose()
        {
            foreach (var particle in this._particles)
            {
                particle?.Dispose();
            }

            foreach (var texture2D in this._textures)
            {
                texture2D?.Dispose();
            }
        }
    }
}