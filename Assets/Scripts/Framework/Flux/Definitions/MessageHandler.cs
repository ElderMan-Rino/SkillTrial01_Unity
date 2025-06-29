using Elder.Framework.Flux.Interfaces;

namespace Elder.Framework.Flux.Definitions
{
    public delegate void MessageHandler<T>(in T message) where T : struct, IFluxMessage;
}