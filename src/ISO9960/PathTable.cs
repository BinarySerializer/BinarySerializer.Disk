namespace BinarySerializer.Disk.ISO9960
{
    public class PathTable : BinarySerializable
    {
        public Entry[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Entries = s.SerializeObjectArrayUntil(Entries, x => x.DirectoryIdentifierLength == 0, getLastObjFunc: () => new Entry(), name: nameof(Entries));
        }

        public class Entry : BinarySerializable
        {
            public byte DirectoryIdentifierLength { get; set; }
            public byte ExtendedAttributeRecordLength { get; set; }
            public uint ExtentLBA { get; set; }
            public ushort ParentDirectoryIndex { get; set; }
            public string DirectoryIdentifier { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                DirectoryIdentifierLength = s.Serialize<byte>(DirectoryIdentifierLength, name: nameof(DirectoryIdentifierLength));
                
                if (DirectoryIdentifierLength == 0) 
                    return;
                
                ExtendedAttributeRecordLength = s.Serialize<byte>(ExtendedAttributeRecordLength, name: nameof(ExtendedAttributeRecordLength));
                ExtentLBA = s.Serialize<uint>(ExtentLBA, name: nameof(ExtentLBA));
                ParentDirectoryIndex = s.Serialize<ushort>(ParentDirectoryIndex, name: nameof(ParentDirectoryIndex));
                DirectoryIdentifier = s.SerializeString(DirectoryIdentifier, length: DirectoryIdentifierLength, name: nameof(DirectoryIdentifier));
                
                if (DirectoryIdentifierLength % 2 != 0)
                    s.SerializePadding(1, logIfNotNull: true);
            }
        }
    }
}