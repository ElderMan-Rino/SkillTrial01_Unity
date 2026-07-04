namespace Elder.Framework.Data.App
{
    internal interface IDataHandleList
    {
        public int Scope { get; }

        public void DisposeAll();
    }
}
