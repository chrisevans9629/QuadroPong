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
    public interface IPlayerController
    {
        InputResult UpdateAcceleration(Sprite sprite, Ball ball);
    }
}