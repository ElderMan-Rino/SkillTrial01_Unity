using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Composition.Internal
{
    internal sealed class RegistrationBuilder : IRegistrationBuilder
    {
        private readonly Registration _registration;

        public RegistrationBuilder(Registration registration)
        {
            _registration = registration;
        }

        public IRegistrationBuilder As<T>() where T : ISystemComponent
        {
            _registration.AddServiceType(typeof(T));
            return this;
        }
    }
}