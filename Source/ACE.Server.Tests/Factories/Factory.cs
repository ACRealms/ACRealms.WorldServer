using ACE.Server.Network.GameMessages.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACRealms.Tests.Factories
{
    internal abstract class Factory { }

    internal abstract class Factory<T, TFactory> : Factory
        where TFactory : Factory<T, TFactory>, new()
    {
        private static TFactory DefaultFactory { get; } = Activator.CreateInstance<TFactory>();
        public static T Make() => DefaultFactory.Create();
        public static uint CurrentIndex { get; private set; } = 0;

        public T Create()
        {
            lock (typeof(TFactory))
            {
                CurrentIndex++;
                return Builder().Invoke();
            }
        }

        public abstract Func<T> Builder();
    }
}
