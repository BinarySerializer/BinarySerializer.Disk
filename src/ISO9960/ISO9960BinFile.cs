using System;
using System.IO;
using System.Linq;

namespace BinarySerializer.Disk.ISO9960
{
    // See: https://wiki.osdev.org/ISO_9660 and https://problemkaputt.de/psxspx-cdrom-drive.htm
    public class ISO9960BinFile : BinarySerializable
    {
        public const uint SectorDataSize = 0x800;
        public const uint SectorHeaderSize = 0x18;
        public const uint SectorFooterSize = 0x118;
        public const uint SectorSize = SectorDataSize + SectorHeaderSize + SectorFooterSize;

        public Sector<VolumeDescriptor>[] VolumeDescriptors { get; set; }
        public PrimaryVolumeDescriptor PrimaryVolumeDescriptor => VolumeDescriptors.
            First(x => x.Object.TypeCode == VolumeDescriptorTypeCode.PrimaryVolumeDescriptor).
            Object.
            PrimaryVolumeDescriptor;

        public Sector<PathTable> PathTable { get; set; }
        public Sector<Directory>[] Directories { get; set; }

        protected virtual void SerializeSystemSectors(SerializerObject s) { }

        public Pointer LBAToPointer(long lba)
        {
            return Offset + lba * SectorSize;
        }

        public Sector<T> SerializeSector<T>(SerializerObject s, Sector<T> obj, long lba, string name = null)
            where T : BinarySerializable, new()
        {
            s.Goto(LBAToPointer(lba));
            return s.SerializeObject<Sector<T>>(obj, name: name);
        }

        public Sector<T>[] SerializeSectorsUntil<T>(SerializerObject s, Sector<T>[] obj, long lba, Func<Sector<T>, bool> conditionCheckFunc, string name = null)
            where T : BinarySerializable, new()
        {
            s.Goto(LBAToPointer(lba));
            return s.SerializeObjectArrayUntil<Sector<T>>(obj, conditionCheckFunc, name: name);
        }

        public Directory GetDirectory(string dirPath, bool throwOnError)
        {
            if (dirPath == null)
                throw new ArgumentNullException(nameof(dirPath));

            // Get the individual paths
            string[] paths = dirPath.
                Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).
                Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Default to root
            int dirIndex = 0;
            uint lba = PrimaryVolumeDescriptor.Root.ExtentLBA;

            // Get the directory LBA
            foreach (string dir in paths)
            {
                dirIndex = Array.FindIndex(PathTable.Object.Entries, x => x.ParentDirectoryIndex == dirIndex + 1 && x.DirectoryIdentifier == dir);

                if (dirIndex == -1)
                {
                    if (throwOnError)
                        throw new Exception($"Directory {dir} not found");
                    else
                        return null;
                }

                lba = PathTable.Object.Entries[dirIndex].ExtentLBA;
            }

            // Get the directory records to find the file
            Directory directory = Directories.FirstOrDefault(x => x.Object.Entries.First().ExtentLBA == lba)?.Object;

            if (directory == null)
            {
                if (throwOnError)
                    throw new Exception($"Directory not found for LBA {lba}");
                else
                    return null;
            }

            return directory;
        }

        public DirectoryRecord GetFile(string filePath, bool throwOnError)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            // Get the individual paths
            string[] paths = filePath.
                Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).
                Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (!paths.Any())
            {
                if (throwOnError)
                    throw new Exception("The file path can't be empty");
                else
                    return null;
            }

            // Default to root
            int dirIndex = 0;
            uint lba = PrimaryVolumeDescriptor.Root.ExtentLBA;

            // Get the directory LBA
            foreach (string dir in paths.Take(paths.Length - 1))
            {
                dirIndex = Array.FindIndex(PathTable.Object.Entries, x => x.ParentDirectoryIndex == dirIndex + 1 && x.DirectoryIdentifier == dir);

                if (dirIndex == -1)
                {
                    if (throwOnError)
                        throw new Exception($"Directory {dir} not found");
                    else
                        return null;
                }

                lba = PathTable.Object.Entries[dirIndex].ExtentLBA;
            }

            // Get the directory records to find the file
            DirectoryRecord[] dirRecords = Directories.FirstOrDefault(x => x.Object.Entries.First().ExtentLBA == lba)?.Object.Entries;

            if (dirRecords == null)
            {
                if (throwOnError)
                    throw new Exception($"Directory not found for LBA {lba}");
                else
                    return null;
            }

            // Find the file
            string fileName = paths.Last();
            DirectoryRecord file = dirRecords.FirstOrDefault(x => 
                x.FileIdentifier == fileName && !x.FileFlags.HasFlag(DirectoryRecord.RecordFileFlags.Directory));

            if (file == null)
            {
                if (throwOnError)
                    throw new Exception($"File {fileName} not found in directory with LBA {lba}");
                else
                    return null;
            }

            return file;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            // Sectors 0-15 are system reserved
            SerializeSystemSectors(s);

            // Serialize volume descriptors from sector 16
            VolumeDescriptors = SerializeSectorsUntil(
                s: s, 
                obj: VolumeDescriptors, 
                lba: 16,
                conditionCheckFunc: x => x.Object.TypeCode == VolumeDescriptorTypeCode.VolumeDescriptorSetTerminator,
                name: nameof(VolumeDescriptors));

            // Serialize path table
            PathTable = SerializeSector<PathTable>(s, PathTable, PrimaryVolumeDescriptor.PathTableLBA, name: nameof(PathTable));

            // Serialize directories
            Directories = s.InitializeArray(Directories, PathTable.Object.Entries.Length);
            s.DoArray(Directories, (x, i, name) => SerializeSector<Directory>(s, x, PathTable.Object.Entries[i].ExtentLBA, name: name));
        }
    }
}