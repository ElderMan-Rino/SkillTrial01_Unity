namespace Elder.Framework.Blob.App
{
    internal interface IDataHandleList
    {
        public int Scope { get; }

        public void DisposeAll();
    }
}
