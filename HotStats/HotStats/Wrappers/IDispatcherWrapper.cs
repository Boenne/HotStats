using System;

namespace HotStats.Wrappers
{
    public interface IDispatcherWrapper
    {
        void BeginInvoke(Action action);
    }
}