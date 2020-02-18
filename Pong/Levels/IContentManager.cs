namespace MyGame.Levels
{
    public interface IContentManager
    {
        T Load<T>(string assetName);
    }
}