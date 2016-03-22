using System;
using System.Windows;
using System.Windows.Threading;

namespace HotStats.Wrappers
{
    public class DispatcherWrapper : IDispatcherWrapper
    {
        public void BeginInvoke(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
    }
}