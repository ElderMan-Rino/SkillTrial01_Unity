namespace Elder.Framework.Boot.Interfaces
{
    public interface IBootConfig 
    {
        public string BaseDataKey { get; }
        public string EncryptionKeyPartB { get; }
    }
}