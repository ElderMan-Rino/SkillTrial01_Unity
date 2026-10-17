namespace Elder.Framework.Core.Interfaces
{
    public interface ISignalSubscribable
    {
        public bool TrySignalSubscribe(IComponentRegistry provider);
        public void SignalUnsubscribe();
    }
}
