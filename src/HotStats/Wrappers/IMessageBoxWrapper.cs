using System.Windows;

namespace HotStats.Wrappers
{
    public interface IMessageBoxWrapper
    {
        void Show(string message);
    }

    public class MessageBoxWrapper : IMessageBoxWrapper
    {
        public void Show(string message)
        {
            MessageBox.Show(message);
        }
    }
}