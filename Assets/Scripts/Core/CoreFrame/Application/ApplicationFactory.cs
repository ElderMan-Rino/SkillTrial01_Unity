using Elder.Core.Common.Interfaces;
using Elder.Core.CoreFrame.Interfaces;
using System;
using System.Collections;

namespace Elder.Core.CoreFrame.Application
{
    public class ApplicationFactory : DictionaryBase, IApplicationFactory
    {
        public bool TryCreateApplication(Type type, out IApplication application)
        {
            throw new NotImplementedException();
        }
    }
}