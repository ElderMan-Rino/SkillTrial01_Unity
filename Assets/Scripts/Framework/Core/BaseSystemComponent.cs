using Elder.Framework.Core.Interfaces;
using Elder.Framework.Core.Interfaces.Base;

namespace Elder.Framework.Core
{
    public abstract class BaseSystemComponent : DisposableBase, ISystemComponent
    {
        public override void PreDispose() { }
    }
}