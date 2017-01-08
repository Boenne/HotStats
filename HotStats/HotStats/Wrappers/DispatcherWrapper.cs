using System;
using System.Windows;
using System.Windows.Threading;

namespace HotStats.Wrappers
{
    public interface IDispatcherWrapper
    {
        void BeginInvoke(Action action);
    }

    public class DispatcherWrapper : IDispatcherWrapper
    {
        public void BeginInvoke(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
    }
}