using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
namespace ConnectorCenter.Services.Archive
{
    public static class ArchiveService
    {
        public static byte[] GetArchive(File file)
        {
            using (MemoryStream compressedFileStream = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                    ZipArchiveEntry zipEntry = zipArchive.CreateEntry(file.Name);
                    using (MemoryStream originalFileStream = new MemoryStream(file.Data))
                    using (Stream zipEntryStream = zipEntry.Open())
                    {
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                }
                return compressedFileStream.ToArray();
            }
        }
        public static byte[] GetArchive(File[] files)
        {
            using (MemoryStream compressedFileStream = new MemoryStream())
            {
                using (ZipArchive zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                {
                    foreach (File file in files)
                    {
                        ZipArchiveEntry zipEntry = zipArchive.CreateEntry(file.Name);
                        using (MemoryStream originalFileStream = new MemoryStream(file.Data))
                            using (Stream zipEntryStream = zipEntry.Open())
                            {
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                    }
                }
                return compressedFileStream.ToArray();
            }
        }
    }

}
