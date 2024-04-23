using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using ACE.Database.Models.Shard;
using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ACE.Database
{
    public class ShardConfigDatabase
    {
        public IDbContextFactory<ShardDbContext> ContextFactory { get; }

        public ShardConfigDatabase(IServiceProvider services)
        {
            ContextFactory = services.GetRequiredService<IDbContextFactory<ShardDbContext>>();
        }

        public bool BoolExists(string key)
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesBoolean.Any(r => r.Key == key);
        }

        public bool DoubleExists(string key)
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesDouble.Any(r => r.Key == key);
        }

        public bool LongExists(string key)
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesLong.Any(r => r.Key == key);
        }

        public bool StringExists(string key)
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesString.Any(r => r.Key == key);
        }


        public void AddBool(string key, bool value, string description = null)
        {
            var stat = new ConfigPropertiesBoolean
            {
                Key = key,
                Value = value,
                Description = description
            };

            using (var context = ContextFactory.CreateDbContext())
            {
                context.ConfigPropertiesBoolean.Add(stat);

                context.SaveChanges();
            }
        }

        public void AddLong(string key, long value, string description = null)
        {
            var stat = new ConfigPropertiesLong
            {
                Key = key,
                Value = value,
                Description = description
            };

            using (var context = ContextFactory.CreateDbContext())
            {
                context.ConfigPropertiesLong.Add(stat);

                context.SaveChanges();
            }
        }

        public void AddDouble(string key, double value, string description = null)
        {
            var stat = new ConfigPropertiesDouble
            {
                Key = key,
                Value = value,
                Description = description
            };

            using (var context = ContextFactory.CreateDbContext())
            {
                context.ConfigPropertiesDouble.Add(stat);

                context.SaveChanges();
            }
        }

        public void AddString(string key, string value, string description = null)
        {
            var stat = new ConfigPropertiesString
            {
                Key = key,
                Value = value,
                Description = description
            };

            using (var context = ContextFactory.CreateDbContext())
            {
                context.ConfigPropertiesString.Add(stat);

                context.SaveChanges();
            }
        }


        public ConfigPropertiesBoolean GetBool(string key)
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesBoolean.AsNoTracking().FirstOrDefault(r => r.Key == key);
        }

        public ConfigPropertiesLong GetLong(string key)
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesLong.AsNoTracking().FirstOrDefault(r => r.Key == key);
        }

        public ConfigPropertiesDouble GetDouble(string key)
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesDouble.AsNoTracking().FirstOrDefault(r => r.Key == key);
        }

        public ConfigPropertiesString GetString(string key)
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesString.AsNoTracking().FirstOrDefault(r => r.Key == key);
        }


        public List<ConfigPropertiesBoolean> GetAllBools()
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesBoolean.AsNoTracking().ToList();
        }

        public List<ConfigPropertiesLong> GetAllLongs()
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesLong.AsNoTracking().ToList();
        }

        public List<ConfigPropertiesDouble> GetAllDoubles()
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesDouble.AsNoTracking().ToList();
        }

        public List<ConfigPropertiesString> GetAllStrings()
        {
            using (var context = ContextFactory.CreateDbContext())
                return context.ConfigPropertiesString.AsNoTracking().ToList();
        }


        public void SaveBool(ConfigPropertiesBoolean stat)
        {
            using (var context = ContextFactory.CreateDbContext())
            {
                context.Entry(stat).State = EntityState.Modified;

                context.SaveChanges();
            }
        }

        public void SaveLong(ConfigPropertiesLong stat)
        {
            using (var context = ContextFactory.CreateDbContext())
            {
                context.Entry(stat).State = EntityState.Modified;

                context.SaveChanges();
            }
        }

        public void SaveDouble(ConfigPropertiesDouble stat)
        {
            using (var context = ContextFactory.CreateDbContext())
            {
                context.Entry(stat).State = EntityState.Modified;

                context.SaveChanges();
            }
        }

        public void SaveString(ConfigPropertiesString stat)
        {
            using (var context = ContextFactory.CreateDbContext())
            {
                context.Entry(stat).State = EntityState.Modified;

                context.SaveChanges();
            }
        }
    }
}
