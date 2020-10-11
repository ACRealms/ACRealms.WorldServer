using System;
using System.Collections.Generic;

using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;

namespace ACE.Entity.Models
{
    public class Realm
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public RealmType Type { get; set; }
        public ushort? ParentRealmID { get; set; }


        public IDictionary<RealmPropertyBool, AppliedRealmProperty<bool>> PropertiesBool { get; set; }
        public IDictionary<RealmPropertyFloat, AppliedRealmProperty<double>> PropertiesFloat { get; set; }
        public IDictionary<RealmPropertyInt, AppliedRealmProperty<int>> PropertiesInt { get; set; }
        public IDictionary<RealmPropertyInt64, AppliedRealmProperty<long>> PropertiesInt64 { get; set; }
        public IDictionary<RealmPropertyString, AppliedRealmProperty<string>> PropertiesString { get; set; }
        public bool NeedsRefresh { get; set; }
    }

    public class AppliedRealmProperty<T>
    {
        public RealmPropertyOptions<T> Options { get; }

        private T _value;
        public T Value
        {
            get
            {
                if (Options.RandomType == RealmPropertyRerollType.always)
                    RollValue();
                return _value;
            }
            private set => _value = value;
        }

        private AppliedRealmProperty() { }

        //Clone
        public AppliedRealmProperty(AppliedRealmProperty<T> prop)
        {
            this.Options = prop.Options;
            RollValue();
        }

        public AppliedRealmProperty(RealmPropertyOptions<T> options)
        {
            this.Options = options;
            RollValue();
        }

        public void RollValue() => Value = Options.RollValue();

        public override string ToString()
        {
            switch(Options.RandomType)
            {
                case RealmPropertyRerollType.always:
                    return $"Random ({Options.MinValue} to {Options.MaxValue})";
                case RealmPropertyRerollType.landblock:
                    return $"{Value} (Landblock reroll, random range {Options.MinValue} to {Options.MaxValue}";
                case RealmPropertyRerollType.manual:
                    return $"{Value} (Manual reroll, random range {Options.MinValue} to {Options.MaxValue}";
                case RealmPropertyRerollType.never:
                    return $"{Value}";
                default:
                    return $"{Value}";
            }
        }
    }
    public class RealmPropertyOptions<T>
    {
        static Random Randomizer = new Random();

        public T DefaultValue { get; private set; }
        public RealmPropertyRerollType RandomType { get; private set; }
        public T MinValue { get; private set; }
        public T MaxValue { get; private set; }
        public bool Locked { get; private set; }
        public double Probability { get; private set; }

        public RealmPropertyOptions() { }

        public void SeedPropertiesStatic(T defaultValue, bool locked, double? probability)
        {
            RandomType = RealmPropertyRerollType.never;
            Locked = locked;
            Probability = probability ?? 1.0;
            DefaultValue = defaultValue;
        }

        public void SeedPropertiesRandomized(byte randomType, T randomLowRange, T randomHighRange, bool locked, double? probability)
        {
            Locked = locked;
            Probability = probability ?? 1.0;
            RandomType = (RealmPropertyRerollType)randomType;
            MinValue = randomLowRange;
            MaxValue = randomHighRange;
            ClampMinMax();
        }

        private void ClampMinMax()
        {
            if (typeof(T) == typeof(double))
            {
                if ((double)(object)MinValue > (double)(object)MaxValue)
                    MaxValue = MinValue;
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            {
                if ((long)(object)MinValue > (long)(object)MaxValue)
                    MaxValue = MinValue;
            }
        }

        public T RollValue()
        {
            if (RandomType == RealmPropertyRerollType.never)
                return DefaultValue;

            if (typeof(T) == typeof(double))
                return (T)(object)(Randomizer.NextDouble() * ((double)(object)MaxValue - (double)(object)MinValue) + (double)(object)MinValue);
            else if (typeof(T) == typeof(int))
                return (T)(object)Randomizer.Next((int)(object)MinValue, (int)(object)MaxValue);
            else if (typeof(T) == typeof(long))
                return (T)(object)NextLong((long)(object)MinValue, (long)(object)MaxValue);
            else
                return DefaultValue;
        }

        private long NextLong(long min, long max)
        {
            if (max <= min)
                throw new ArgumentOutOfRangeException("max", "max must be > min!");

            ulong uRange = (ulong)(max - min);
            ulong ulongRand;
            do
            {
                byte[] buf = new byte[8];
                Randomizer.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (long)(ulongRand % uRange) + min;
        }
    }
}
