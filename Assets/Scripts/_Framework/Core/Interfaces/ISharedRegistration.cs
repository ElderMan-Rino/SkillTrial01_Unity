namespace Elder.Framework.Core.Interfaces
{
    public interface ISharedRegistration
    {
        public ISharedRegistration As<T>() where T : IFrameworkComponent;
    }
}