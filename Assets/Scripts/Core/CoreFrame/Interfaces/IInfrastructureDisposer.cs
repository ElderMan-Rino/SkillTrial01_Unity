using System;

namespace Elder.Core.CoreFrame.Interfaces
{
    public interface IInfrastructureDisposer : IDisposable
    {
        public void DiposeSceneInfras();
        public void DisposePersistentInfras();
        public void DisposeLogInfras();
        public void PreDiposeSceneInfras();
        public void PreDisposePersistentInfras();
    }
}
