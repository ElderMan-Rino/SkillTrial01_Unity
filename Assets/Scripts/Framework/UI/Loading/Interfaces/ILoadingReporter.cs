namespace Elder.Framework.UI.Loading.Interfaces
{
    public interface ILoadingReporter
    {
        public void Report(float progress, string statusText);
        public void Complete();
    }
}
