using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Boot.Interfaces
{
    public interface IBootConfig : ISystemComponent
    {
        public string BaseDataKey { get; }
        public string EncryptionKeyPartB { get; }
    }
}
