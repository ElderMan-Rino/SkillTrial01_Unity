namespace Elder.Framework.Core.Interfaces
{
    public interface IFrameworkComponentActivator
    {
        public bool TryActivate(IFrameworkComponent component);
    }
}
