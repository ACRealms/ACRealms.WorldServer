using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ACE.Common;
using ACE.DatLoader;
using MonoMod.Utils;
using Xunit;

namespace ACRealms.Tests.DatLoader
{
    public class DatTests
    {
        static string DatFileDir { get; } = ConfigManager.Config.Server.DatFilesDirectory;

        private static string cellDatLocation = $@"{DatFileDir}/client_cell_1.dat";
        private static int expectedCellDatFileCount = 805003;

        private static string portalDatLocation = $@"{DatFileDir}/client_portal.dat";
        private static int expectedPortalDatFileCount = 79694;

        private static string localEnglishDatLocation = $@"{DatFileDir}/client_local_English.dat";
        private static int expectedLocalEnglishDatFileCount = 118;

        const string skip = "Dat Tests Disabled";

        [Fact(Skip = skip)]
        public void LoadCellDat_NoExceptions()
        {
            DatDatabase dat = new DatDatabase(cellDatLocation);
            int count = dat.AllFiles.Count;
            //Assert.AreEqual(ExpectedCellDatFileCount, count);
            Assert.True(expectedCellDatFileCount <= count, $"Insufficient files parsed from .dat. Expected: >= {expectedCellDatFileCount}, Actual: {count}");
        }

        [Fact(Skip = skip)]
        public void LoadPortalDat_NoExceptions()
        {
            // Init our text encoding options. This will allow us to use more than standard ANSI text, which the client also supports.
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DatDatabase dat = new DatDatabase(portalDatLocation);
            int count = dat.AllFiles.Count;
            //Assert.AreEqual(expectedPortalDatFileCount, count);
            Assert.True(expectedPortalDatFileCount <= count, $"Insufficient files parsed from .dat. Expected: >= {expectedPortalDatFileCount}, Actual: {count}");
        }

        [Fact(Skip = skip)]
        public void LoadLocalEnglishDat_NoExceptions()
        {
            // Init our text encoding options. This will allow us to use more than standard ANSI text, which the client also supports.
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DatDatabase dat = new DatDatabase(localEnglishDatLocation);
            int count = dat.AllFiles.Count;
            //Assert.AreEqual(expectedPortalDatFileCount, count);
            Assert.True(expectedLocalEnglishDatFileCount <= count, $"Insufficient files parsed from .dat. Expected: >= {expectedLocalEnglishDatFileCount}, Actual: {count}");
        }


        [Fact(Skip = skip)]
        public void UnpackCellDatFiles_NoExceptions()
        {
            var assembly = typeof(DatDatabase).GetTypeInfo().Assembly;
            var types = assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(DatFileTypeAttribute), false).Length > 0).ToList();

            if (types.Count == 0)
                throw new Exception("Failed to locate any types with DatFileTypeAttribute.");

            DatDatabase dat = new DatDatabase(cellDatLocation);

            foreach (var kvp in dat.AllFiles)
            {
                if (kvp.Key == 0xFFFF0001) // // Iteration info
                    continue;

                if (kvp.Value.FileSize == 0) // DatFileType.LandBlock files can be empty
                    continue;

                var fileType = kvp.Value.GetFileType(DatDatabaseType.Cell);

                if ((kvp.Key & 0xFFFF) == 0xFFFE) fileType = DatFileType.LandBlockInfo;
                if ((kvp.Key & 0xFFFF) == 0xFFFF) fileType = DatFileType.LandBlock;

                //Assert.True(fileType != null, $"Key: 0x{kvp.Key:X8}, ObjectID: 0x{kvp.Value.ObjectId:X8}, FileSize: {kvp.Value.FileSize}, BitFlags:, 0x{kvp.Value.BitFlags:X8}");
                Assert.True(fileType != null, $"Key: 0x{kvp.Key:X8}, ObjectID: 0x{kvp.Value.ObjectId:X8}, FileSize: {kvp.Value.FileSize}");

                var type = types
                    .SelectMany(m => m.GetCustomAttributes(typeof(DatFileTypeAttribute), false), (m, a) => new { m, a })
                    .Where(t => ((DatFileTypeAttribute)t.a).FileType == fileType)
                    .Select(t => t.m);

                var first = type.FirstOrDefault();

                if (first == null)
                    throw new Exception($"Failed to Unpack fileType: {fileType}");

                var obj = Activator.CreateInstance(first);

                var unpackable = obj as IUnpackable;

                if (unpackable == null)
                    throw new Exception($"Class for fileType: {fileType} does not implement IUnpackable.");

                var datReader = new DatReader(cellDatLocation, kvp.Value.FileOffset, kvp.Value.FileSize, dat.Header.BlockSize);

                using (var memoryStream = new MemoryStream(datReader.Buffer))
                using (var reader = new BinaryReader(memoryStream))
                {
                    unpackable.Unpack(reader);

                    if (memoryStream.Position != kvp.Value.FileSize)
                        throw new Exception($"Failed to parse all bytes for fileType: {fileType}, ObjectId: 0x{kvp.Value.ObjectId:X8}. Bytes parsed: {memoryStream.Position} of {kvp.Value.FileSize}");
                }
            }
        }

        [Fact(Skip = skip)]
        public void UnpackPortalDatFiles_NoExceptions()
        {
            var assembly = typeof(DatDatabase).GetTypeInfo().Assembly;
            var types = assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(DatFileTypeAttribute), false).Length > 0).ToList();

            if (types.Count == 0)
                throw new Exception("Failed to locate any types with DatFileTypeAttribute.");

            DatDatabase dat = new DatDatabase(portalDatLocation);

            foreach (var kvp in dat.AllFiles)
            {
                if (kvp.Key == 0xFFFF0001) // Iteration info
                    continue;

                var fileType = kvp.Value.GetFileType(DatDatabaseType.Portal);

                //Assert.True(fileType != null, $"Key: 0x{kvp.Key:X8}, ObjectID: 0x{kvp.Value.ObjectId:X8}, FileSize: {kvp.Value.FileSize}, BitFlags:, 0x{kvp.Value.BitFlags:X8}");
                Assert.True(fileType != null, $"Key: 0x{kvp.Key:X8}, ObjectID: 0x{kvp.Value.ObjectId:X8}, FileSize: {kvp.Value.FileSize}");

                // These file types aren't converted yet
                if (fileType == DatFileType.KeyMap) continue;
                if (fileType == DatFileType.RenderMaterial) continue;
                if (fileType == DatFileType.MaterialModifier) continue;
                if (fileType == DatFileType.MaterialInstance) continue;
                if (fileType == DatFileType.ActionMap) continue;
                if (fileType == DatFileType.DbProperties) continue;

                var type = types
                    .SelectMany(m => m.GetCustomAttributes(typeof(DatFileTypeAttribute), false), (m, a) => new { m, a })
                    .Where(t => ((DatFileTypeAttribute)t.a).FileType == fileType)
                    .Select(t => t.m);

                var first = type.FirstOrDefault();

                if (first == null)
                    throw new Exception($"Failed to Unpack fileType: {fileType}");

                var obj = Activator.CreateInstance(first);

                var unpackable = obj as IUnpackable;

                if (unpackable == null)
                    throw new Exception($"Class for fileType: {fileType} does not implement IUnpackable.");

                var datReader = new DatReader(portalDatLocation, kvp.Value.FileOffset, kvp.Value.FileSize, dat.Header.BlockSize);

                using (var memoryStream = new MemoryStream(datReader.Buffer))
                using (var reader = new BinaryReader(memoryStream))
                {
                    unpackable.Unpack(reader);

                    if (memoryStream.Position != kvp.Value.FileSize)
                        throw new Exception($"Failed to parse all bytes for fileType: {fileType}, ObjectId: 0x{kvp.Value.ObjectId:X8}. Bytes parsed: {memoryStream.Position} of {kvp.Value.FileSize}");
                }
            }
        }

        [Fact(Skip = skip)]
        public void UnpackLocalEnglishDatFiles_NoExceptions()
        {
            var assembly = typeof(DatDatabase).GetTypeInfo().Assembly;
            var types = assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(DatFileTypeAttribute), false).Length > 0).ToList();

            if (types.Count == 0)
                throw new Exception("Failed to locate any types with DatFileTypeAttribute.");

            DatDatabase dat = new DatDatabase(localEnglishDatLocation);

            foreach (var kvp in dat.AllFiles)
            {
                if (kvp.Key == 0xFFFF0001) // Iteration info
                    continue;

                var fileType = kvp.Value.GetFileType(DatDatabaseType.Language);

                //Assert.True(fileType != null, $"Key: 0x{kvp.Key:X8}, ObjectID: 0x{kvp.Value.ObjectId:X8}, FileSize: {kvp.Value.FileSize}, BitFlags:, 0x{kvp.Value.BitFlags:X8}");
                Assert.True(fileType != null, $"Key: 0x{kvp.Key:X8}, ObjectID: 0x{kvp.Value.ObjectId:X8}, FileSize: {kvp.Value.FileSize}");

                // These file types aren't converted yet
                if (fileType == DatFileType.UiLayout) continue;

                var type = types
                    .SelectMany(m => m.GetCustomAttributes(typeof(DatFileTypeAttribute), false), (m, a) => new { m, a })
                    .Where(t => ((DatFileTypeAttribute)t.a).FileType == fileType)
                    .Select(t => t.m);

                var first = type.FirstOrDefault();

                if (first == null)
                    throw new Exception($"Failed to Unpack fileType: {fileType}");

                var obj = Activator.CreateInstance(first);

                var unpackable = obj as IUnpackable;

                if (unpackable == null)
                    throw new Exception($"Class for fileType: {fileType} does not implement IUnpackable.");

                var datReader = new DatReader(localEnglishDatLocation, kvp.Value.FileOffset, kvp.Value.FileSize, dat.Header.BlockSize);

                using (var memoryStream = new MemoryStream(datReader.Buffer))
                using (var reader = new BinaryReader(memoryStream))
                {
                    unpackable.Unpack(reader);

                    if (memoryStream.Position != kvp.Value.FileSize)
                        throw new Exception($"Failed to parse all bytes for fileType: {fileType}, ObjectId: 0x{kvp.Value.ObjectId:X8}. Bytes parsed: {memoryStream.Position} of {kvp.Value.FileSize}");
                }
            }
        }

        // Change Skip to null if you want to run this
        // I do not recommend running this because it will generate a huge number of files and cause major slowdowns with future test runs. It took 1 hour to delete the files
        [Fact(Skip = "Skipping slow test")]
        public void ExtractCellDatByLandblock()
        {
            string output = @$"{Helpers.Paths.LocalDataPath}\cell_dat_export_by_landblock";
            CellDatDatabase db = new CellDatDatabase(cellDatLocation);
            db.ExtractLandblockContents(output);
        }

        // Change Skip to null if you want to run this
        // I do not recommend running this because it will generate a huge number of files and cause major slowdowns with future test runs. It took 1 hour to delete the files
        [Fact(Skip = "Skipping slow test")]
        public void ExportPortalDatsWithTypeInfo()
        {
            string output = @$"{Helpers.Paths.LocalDataPath}\typed_portal_dat_export";
            PortalDatDatabase db = new PortalDatDatabase(portalDatLocation);
            db.ExtractCategorizedPortalContents(output);
        }
    }
}
