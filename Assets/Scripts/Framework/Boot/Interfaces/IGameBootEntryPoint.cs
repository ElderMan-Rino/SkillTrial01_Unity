using Elder.Framework.Core.Interfaces;

namespace Elder.Framework.Boot.Interfaces
{
    public interface IGameBootEntryPoint : IGameSystem
    {
        public void Run();
    }
}