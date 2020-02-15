namespace MyGame
{
    public interface ISettings
    {
        bool IsPaused { get; set; }
        bool IsDebugging { get; set; }
        bool IsSoundOn { get; set; }
    }
}