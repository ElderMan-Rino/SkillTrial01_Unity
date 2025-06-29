using Elder.Framework.Input.Domain.Values;

namespace Elder.Framework.Input.Interfaces
{
    public interface IInputService 
    {
        public AxisInputData GetAxisData();
        public bool GetButtonDown(string action);
    }
}