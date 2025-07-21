namespace Elder.Core.FluxMessage.Delegates
{
    public delegate void MessageHandler<T>(in T message);
}

