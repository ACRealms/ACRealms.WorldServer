using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ACE.Entity.Models
{
    public static class PropertiesAllegianceExtensions
    {
        public static Dictionary<ulong, PropertiesAllegiance> GetApprovedVassals(this IDictionary<ulong, PropertiesAllegiance> value, ReaderWriterLockSlim rwLock)
        {
            rwLock.EnterReadLock();
            try
            {
                if (value == null)
                    return new Dictionary<ulong, PropertiesAllegiance>();

                return value.Where(i => i.Value.ApprovedVassal).ToDictionary(i => i.Key, i => i.Value);
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static Dictionary<ulong, PropertiesAllegiance> GetBanList(this IDictionary<ulong, PropertiesAllegiance> value, ReaderWriterLockSlim rwLock)
        {
            if (value == null)
                return new Dictionary<ulong, PropertiesAllegiance>();

            rwLock.EnterReadLock();
            try
            {
                return value.Where(i => i.Value.Banned).ToDictionary(i => i.Key, i => i.Value);
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static PropertiesAllegiance GetFirstOrDefaultByCharacterId(this IDictionary<ulong, PropertiesAllegiance> value, ulong characterId, ReaderWriterLockSlim rwLock)
        {
            if (value == null)
                return null;

            rwLock.EnterReadLock();
            try
            {
                value.TryGetValue(characterId, out var entity);

                return entity;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static void AddOrUpdateAllegiance(this IDictionary<ulong, PropertiesAllegiance> value, ulong characterId, bool isBanned, bool approvedVassal, ReaderWriterLockSlim rwLock)
        {
            rwLock.EnterWriteLock();
            try
            {
                if (!value.TryGetValue(characterId, out var entity))
                {
                    entity = new PropertiesAllegiance { Banned = isBanned, ApprovedVassal = approvedVassal };

                    value.Add(characterId, entity);
                }

                entity.Banned = isBanned;
                entity.ApprovedVassal = approvedVassal;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public static bool TryRemoveAllegiance(this IDictionary<ulong, PropertiesAllegiance> value, ulong characterId, ReaderWriterLockSlim rwLock)
        {
            if (value == null)
                return false;

            rwLock.EnterWriteLock();
            try
            {
                return value.Remove(characterId);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }
    }
}
