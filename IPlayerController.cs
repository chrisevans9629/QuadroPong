namespace MyGame
{
    //public interface IUserPlayerController : IPlayerController
    //{
    //    bool IsConnected { get; }
    //}

    public interface IPlayerController
    {
        bool UpdateAcceleration(Sprite sprite, Ball ball);
    }
}