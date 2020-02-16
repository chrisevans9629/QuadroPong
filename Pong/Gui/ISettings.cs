namespace MyGame
{
    public interface ISettings
    {
        bool IsPaused { get; set; }
        bool IsDebugging { get; set; }
        bool IsSoundOn { get; set; }
        bool HasAstroids { get; set; }
        float MasterVolume { get; set; }
        bool IsFullScreen { get; set; }
    }
}