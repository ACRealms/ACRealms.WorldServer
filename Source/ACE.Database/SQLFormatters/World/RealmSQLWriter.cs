using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ACE.Database.Models.World;
using ACE.Entity.Enum;
using ACE.Entity.Enum.Properties;

namespace ACE.Database.SQLFormatters.World
{
    public class RealmSQLWriter : SQLWriter
    {
        public string GetDefaultFileName(Realm input, bool descOnly = false)
        {
            string description = input.Name;
            if (descOnly)
                return description;

            string fileName = input.Id.ToString("00000");
            if (!string.IsNullOrEmpty(description))
                fileName += " " + description;
            fileName = IllegalInFileName.Replace(fileName, "_");
            fileName += ".sql";

            return fileName;
        }

        public void CreateSQLDELETEStatement(Realm input, StreamWriter writer)
        {
            writer.WriteLine($"DELETE FROM `realm` WHERE `id` = {input.Id};");
        }

        public void CreateSQLINSERTStatement(Realm input, StreamWriter writer)
        {
            writer.WriteLine("INSERT INTO `realm` (`id`, `name`)");

            var output = "VALUES (" +
                         $"{input.Id}, " +
                         $"'{input.Name}'" +
                         ");";

            output = FixNullFields(output);

            writer.WriteLine(output);

            if (input.RealmPropertiesBool != null && input.RealmPropertiesBool.Count > 0)
            {
                writer.WriteLine();
                CreateSQLINSERTStatement(input.Id, input.RealmPropertiesBool.ToList(), writer);
            }
            if (input.RealmPropertiesFloat != null && input.RealmPropertiesFloat.Count > 0)
            {
                writer.WriteLine();
                CreateSQLINSERTStatement(input.Id, input.RealmPropertiesFloat.ToList(), writer);
            }
            if (input.RealmPropertiesInt != null && input.RealmPropertiesInt.Count > 0)
            {
                writer.WriteLine();
                CreateSQLINSERTStatement(input.Id, input.RealmPropertiesInt.ToList(), writer);
            }
            if (input.RealmPropertiesInt64 != null && input.RealmPropertiesInt64.Count > 0)
            {
                writer.WriteLine();
                CreateSQLINSERTStatement(input.Id, input.RealmPropertiesInt64.ToList(), writer);
            }
            if (input.RealmPropertiesString != null && input.RealmPropertiesString.Count > 0)
            {
                writer.WriteLine();
                CreateSQLINSERTStatement(input.Id, input.RealmPropertiesString.ToList(), writer);
            }
        }

        public void CreateSQLINSERTStatement(uint realmId, IList<RealmPropertiesInt> input, StreamWriter writer)
        {
            writer.WriteLine("INSERT INTO `realm_properties_int` (`realm_id`, `type`, `value`)");

            var lineGenerator = new Func<int, string>(i => $"{realmId}, {input[i].Type}, {input[i].Value.ToString().PadLeft(3)})" +
            $" /* {Enum.GetName(typeof(RealmPropertyBool), input[i].Type)} */");

            ValuesWriter(input.Count, lineGenerator, writer);
        }

        public void CreateSQLINSERTStatement(uint realmId, IList<RealmPropertiesBool> input, StreamWriter writer)
        {
            writer.WriteLine("INSERT INTO `realm_properties_bool` (`realm_id`, `type`, `value`)");

            var lineGenerator = new Func<int, string>(i => $"{realmId}, {input[i].Type}, {input[i].Value.ToString().PadLeft(3)})" +
            $" /* {Enum.GetName(typeof(RealmPropertyBool), input[i].Type)} */");

            ValuesWriter(input.Count, lineGenerator, writer);
        }

        public void CreateSQLINSERTStatement(uint realmId, IList<RealmPropertiesInt64> input, StreamWriter writer)
        {
            writer.WriteLine("INSERT INTO `realm_properties_int64` (`realm_id`, `type`, `value`)");

            var lineGenerator = new Func<int, string>(i => $"{realmId}, {input[i].Type}, {input[i].Value.ToString().PadLeft(3)})" +
            $" /* {Enum.GetName(typeof(RealmPropertyInt64), input[i].Type)} */");
                
            ValuesWriter(input.Count, lineGenerator, writer);
        }
        public void CreateSQLINSERTStatement(uint realmId, IList<RealmPropertiesString> input, StreamWriter writer)
        {
            writer.WriteLine("INSERT INTO `realm_properties_string` (`realm_id`, `type`, `value`)");

            var lineGenerator = new Func<int, string>(i => $"{realmId}, {input[i].Type}, '{input[i].Value.ToString().PadLeft(3)}')" +
            $" /* {Enum.GetName(typeof(RealmPropertyString), input[i].Type)} */");

            ValuesWriter(input.Count, lineGenerator, writer);
        }

        public void CreateSQLINSERTStatement(uint realmId, IList<RealmPropertiesFloat> input, StreamWriter writer)
        {
            writer.WriteLine("INSERT INTO `realm_properties_float` (`realm_id`, `type`, `value`)");

            var lineGenerator = new Func<int, string>(i => $"{realmId}, {input[i].Type}, {input[i].Value.ToString().PadLeft(3)})" +
            $" /* {Enum.GetName(typeof(RealmPropertyFloat), input[i].Type)} */");

            ValuesWriter(input.Count, lineGenerator, writer);
        }
    }
}
