namespace Elder.Framework.Core.Interfaces
{
    public interface IRegistrationBuilder 
    {
        public IRegistrationBuilder As<T>() where T : ISystemComponent;
    }
}