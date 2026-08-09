using Cysharp.Threading.Tasks;
using Elder.Framework.Signal.Interfaces;

namespace Elder.Framework.Signal.Definitions
{
    public delegate void SignalHandler<T>(in T signal) where T : struct, ISignal;
    public delegate UniTask AsyncSignalHandler<T>(T signal) where T : struct, ISignal;
}