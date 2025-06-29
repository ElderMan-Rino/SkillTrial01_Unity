using System;

namespace Elder.Framework.UI.Loading.Interfaces
{
    public interface ILoadingModel
    {
        public float Progress { get; }
        public string StatusText { get; }
        public bool IsComplete { get; }

        public event Action OnChanged;
    }
}
