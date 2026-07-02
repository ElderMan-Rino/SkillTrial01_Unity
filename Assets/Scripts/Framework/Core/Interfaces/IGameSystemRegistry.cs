namespace Elder.Framework.Core.Interfaces
{
    public interface IGameSystemRegistry
    {
        public IRegistrationBuilder Register<T>() where T : class, ISystemComponent, new();
        public IRegistrationBuilder RegisterInstance<T>(T instance) where T : class, ISystemComponent;
        public IGameSystemProvider Build();
    }

}
