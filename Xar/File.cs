using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Xar
{
    public class File
    {
        private const int ARCHIVE_HEADER_SIZE = 0x0100;
        private const int ARCHIVE_INDEX_ENTRY_SIZE = 0x0100;

        public string Id { get; set; }
        public int Version { get; set; }
        public uint IndexSize { get; set; }
        public string FilePath { get; set; }

        private Dictionary<string, Entry> dict;

        public struct Entry
        {
            public string Id;
            public uint Size;
            public uint Offset;
        }


        public File(string filePath)
        {
            dict = new Dictionary<string, Entry>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                FilePath = filePath;

                var header = new byte[ARCHIVE_HEADER_SIZE];
                if (stream.Read(header, 0, ARCHIVE_HEADER_SIZE) != ARCHIVE_HEADER_SIZE)
                {
                    throw new FileException("Could not read archive header");
                }

                Id = Encoding.ASCII.GetString(header.Take(3).ToArray());
                if (!Id.Equals("CCA"))
                {
                    throw new FileException("Invalid file format");
                }

                Version = header[3];
                if (Version < 2 || Version > 2)
                {
                    throw new FileException("Invalid archive version " + Version.ToString());
                }

                IndexSize = (uint)(header[4] | header[5] << 8 | header[6] << 16 | header[24] << 16);
                for (int a = 0; a<IndexSize; a++)
                {
                    var entry = new byte[ARCHIVE_INDEX_ENTRY_SIZE];
                    stream.Read(entry, 0, ARCHIVE_INDEX_ENTRY_SIZE);

                    var entryStruct = new Entry();
                    entryStruct.Id = Encoding.ASCII.GetString(entry.Take(240).ToArray()).Replace("\0", "");
                    entryStruct.Size = (uint)(entry[240] | entry[241] << 8 | entry[242] << 16 | entry[243] << 24);
                    entryStruct.Offset = (uint)(entry[244] | entry[245] << 8 | entry[246] << 16 | entry[247] << 24);
                    dict.Add(entryStruct.Id, entryStruct);
                }

            }
        }

        public void ExtractFile(string id, string targetFilePath)
        {
            if (!dict.ContainsKey(id))
            {
                throw new FileException("Could not found file in archive");
            }

            var entry = dict[id];

            using (var streamReader = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                using (var streamWriter = new FileStream(targetFilePath, FileMode.Create, FileAccess.Write))
                {
                    long offset = (long)(entry.Offset + ARCHIVE_HEADER_SIZE + (dict.Count * ARCHIVE_INDEX_ENTRY_SIZE));

                    // OPTIMIZE ME!!!!!!!!!!
                    var buffer = new byte[entry.Size];
                    streamReader.Seek(offset, SeekOrigin.Begin);
                    streamReader.Read(
                        buffer,
                        0,
                        (int)entry.Size
                    );
                    streamWriter.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public Entry[] GetEntries()
        {
            return dict.Values.ToArray();
        }

    }
}
