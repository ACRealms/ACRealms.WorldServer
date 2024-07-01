using ACE.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Entity.ACRealms
{
    public abstract record CanonicalName<TEntity, TName>(string Name)
        where TEntity : ICanonicallyResolvable<TEntity, TName>
        where TName : CanonicalName<TEntity, TName>
    {

    }

    public interface ICanonicallyResolvable<TEntity, TName>
        where TEntity : ICanonicallyResolvable<TEntity, TName>
        where TName : CanonicalName<TEntity, TName>
    {
        CanonicalName<TEntity, TName> CanonicalName { get; }
    }


    public abstract record CanonicalResolverContext<TEntity, TName>
        where TEntity : ICanonicallyResolvable<TEntity, TName>
        where TName : CanonicalName<TEntity, TName>
    {
        public abstract bool TryResolve(out IEnumerable<TEntity> resolveCandidates);
        public abstract TName CanonicalName { get; }
    }
}
