using Elder.Framework.Common.Base;
using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Core
{
    public abstract class BaseSystemComponent : DisposableBase, ISystemComponent
    {
        public override void PreDispose() { }
    }
}