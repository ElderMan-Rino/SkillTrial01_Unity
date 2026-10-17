namespace Elder.Framework.Core.Interfaces
{
    public interface IDependencyInjectable
    {
        public void InjectDependency(IComponentProvider provider);
    }
}