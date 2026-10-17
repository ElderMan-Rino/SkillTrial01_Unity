using System;

namespace Elder.Framework.Core.Definitions
{
    public static class GlobalScope
    {
        public static readonly int Id = StringComparer.Ordinal.GetHashCode("GlobalScope");
    }
}