#nullable enable
using System;

namespace BinarySerializer.Disk.Cue
{
    public class DataFile
    {
        public DataFile(string filename, string fileType)
        {
            Filename = filename;
            FileType = fileType.Trim().ToUpper() switch
            {
                "BINARY" => FileType.Binary,
                "MOTOROLA" => FileType.Motorola,
                "AIFF" => FileType.Aiff,
                "WAVE" => FileType.Wave,
                "MP3" => FileType.Mp3,
                _ => FileType.Binary
            };
        }

        public DataFile(string filename, FileType fileType)
        {
            Filename = filename;
            FileType = fileType;
        }

        public string Filename { get; }
        public FileType FileType { get; }

        public string GetFileTypeString()
        {
            return FileType switch
            {
                FileType.Binary => "BINARY",
                FileType.Motorola => "MOTOROLA",
                FileType.Aiff => "AIFF",
                FileType.Wave => "WAVE",
                FileType.Mp3 => "MP3",
                _ => throw new ArgumentOutOfRangeException(nameof(FileType), FileType, null)
            };
        }
    }
}