namespace MyGame
{
    public interface IRandomizer
    {
        float NextFloat();
        int Next(int max);
        int Next(int min,int max);
    }
}