using System;
using Chat.Core;
using NATS.Client;

namespace Chat.Application.Services
{
    public interface INatsBus
    {
        IAsyncSubscription Subscribe<T>(Action<T> handler, string subjectName) where T : class;
        void Publish<T>(T message, string subjectName) where T : class;
    }
}