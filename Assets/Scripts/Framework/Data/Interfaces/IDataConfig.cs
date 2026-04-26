namespace Elder.Framework.Data.Interfaces
{
    public interface IDataConfig
    {
        public string BaseDataKey { get; }
        // 암호화 키 조각B — Inspector 입력, 바이너리에 평문으로 존재하지 않는 조각
        public string EncryptionKeyPartB { get; }
    }
}