using System;

namespace HotStats.Messaging
{
    public interface IMessenger
    {
        void Register<T>(object recipient, Action<T> action);
        void Send<T>(T message);
    }
}