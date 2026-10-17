using Elder.Framework.Signal.Interfaces;
using System;

namespace Elder.Framework.Signal.Messages
{
    public readonly struct FxModuleStartSignal : ISignal
    {
        public FxModuleStartSignal(Type moduleType)
        {
            ModuleType = moduleType;
        }

        public Type ModuleType { get; }
    }
}
