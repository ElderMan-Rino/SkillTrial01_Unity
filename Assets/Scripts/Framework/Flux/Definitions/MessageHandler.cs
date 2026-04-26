using Cysharp.Threading.Tasks;
using Elder.Framework.Flux.Interfaces;

namespace Elder.Framework.Flux.Definitions
{
    public delegate void MessageHandler<T>(in T message) where T : struct, IFluxMessage;
    public delegate UniTask AsyncMessageHandler<T>(T message) where T : struct, IFluxMessage;
}