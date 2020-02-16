using MonoGame.Extended.Gui;

namespace MyGame
{
    public interface IGui
    {
        Screen Screen { get; }
        void Update();
    }
}