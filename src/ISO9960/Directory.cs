namespace BinarySerializer.Disk.ISO9960
{
    // Note: Every directory will start with 2 special entries: an empty string, describing the "." entry, and the string "\1" describing the ".." entry
    public class Directory : BinarySerializable
    {
        public DirectoryRecord[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Entries = s.SerializeObjectArrayUntil(
                obj: Entries,
                conditionCheckFunc: x => s.CurrentPointer - Offset >= ISO9960BinFile.SectorDataSize || x.Length == 0,
                getLastObjFunc: () => new DirectoryRecord(), 
                name: nameof(Entries));
        }
    }
}