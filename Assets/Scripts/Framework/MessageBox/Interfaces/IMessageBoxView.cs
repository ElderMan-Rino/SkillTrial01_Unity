using Elder.Framework.MessageBox.Definitions;

namespace Elder.Framework.MessageBox.Interfaces
{
    public interface IMessageBoxView
    {
        public void SetContent(string message, MessageBoxButtonType buttonType);
    }
}
