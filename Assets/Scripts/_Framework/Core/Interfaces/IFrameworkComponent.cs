using System;

namespace Elder.Framework.Core.Interfaces
{
    public interface IFrameworkComponent 
    {
        public Type ClassType { get; }
        public int Scope { get; }
    }
}