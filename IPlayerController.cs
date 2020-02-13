using Microsoft.Xna.Framework;

namespace MyGame
{
    //public interface IUserPlayerController : IPlayerController
    //{
    //    bool IsConnected { get; }
    //}
    public struct InputResult
    {
        public bool HasMoved { get; set; }
        public bool IsHandled { get; set; }
    }

    public struct InputResult<T>
    {
        public T Value { get; set; }
        public bool IsHandled { get; set; }
    }
    public interface IPlayerController
    {
        InputResult UpdateAcceleration(Sprite sprite, Ball ball);
        InputResult<bool> TriggerPressed();
        InputResult<Vector2> GetDirectional(Vector2 defaultVector2);
    }
}