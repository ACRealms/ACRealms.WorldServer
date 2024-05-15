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
    internal abstract record class Factory { }

    internal abstract record class Factory<T, TFactory> : Factory
        where TFactory : Factory<T, TFactory>, new()
    {
        private static TFactory DefaultFactory { get; } = Activator.CreateInstance<TFactory>();
        public static T Make() => DefaultFactory.Create();
        public static uint CurrentIndex { get; private set; } = 0;
        public static TFactory Identity(T entity) => new TFactory { UsesIdentity = true, Impl = (_) => entity };

        public T Create()
        {
            T val;
            if (UsesIdentity)
                val = Impl((TFactory)this);
            lock (typeof(TFactory))
            {
                CurrentIndex++;
                val = Impl((TFactory)this);
            }
            LastValueStorage.Value = val;
            return val;
        }
        public static T LastValue => LastValueStorage.Value;
        private static ThreadLocal<T> LastValueStorage = new ThreadLocal<T>();

        protected abstract Func<T> Builder();

        private bool UsesIdentity { get; set; } = false;
        private Func<TFactory, T> Impl { get; set; } = (fac) => fac.Builder()();
    }
}
