using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public interface IParticleEngine
    {
        void AddParticles(Vector2 location);
        void AddParticles(Vector2 location, List<Color> colors);
    }
}