namespace Elder.Core.Common.BaseClasses
{
    /// <summary>
    /// ·Î±× °ü·Ã¸¸ ½Ì±ÛÅæ »ç¿ë
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonBase<T> : DisposableBase where T : class, new()
    {
        private static T _instance;
        private static bool _isDisposed = false;

        public static T In
        {
            get
            {
                if (_isDisposed)
                    return null;

                if (_instance == null)
                    _instance = new T();
                return _instance;
            }
        }
        protected override void CompleteDispose()
        {
            MarkAsDisposed();
            DisposeInstance();
        }
        private void MarkAsDisposed()
        {
            _isDisposed = true;
        }
        private void DisposeInstance()
        {
            _instance = null;
        }
    }
}
