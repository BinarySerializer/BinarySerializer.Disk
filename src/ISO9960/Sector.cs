namespace BinarySerializer.Disk.ISO9960
{
    public class Sector<T> : BinarySerializable
        where T : BinarySerializable, new()
    {
        public byte[] Sync { get; set; } // 00 FF FF FF FF FF FF FF FF FF FF 00
        public byte[] Header { get; set; } // Minute, Second, Sector, Mode
        public byte[] SubHeader { get; set; } // x2: File, Channel, Submode, Codinginfo

        public T Object { get; set; } // Sector data, 0x800 bytes reserved

        public byte[] EDC { get; set; } // Checksum
        public byte[] ECC { get; set; } // Error correction

        public override void SerializeImpl(SerializerObject s)
        {
            Sync = s.SerializeArray<byte>(Sync, 12, name: nameof(Sync));
            Header = s.SerializeArray<byte>(Header, 4, name: nameof(Header));
            SubHeader = s.SerializeArray<byte>(SubHeader, 8, name: nameof(SubHeader));

            Object = s.SerializeObject<T>(Object, name: nameof(Object));
            s.Goto(Offset + ISO9960BinFile.SectorDataSize + ISO9960BinFile.SectorHeaderSize);

            EDC = s.SerializeArray<byte>(EDC, 4, name: nameof(EDC));
            ECC = s.SerializeArray<byte>(ECC, 276, name: nameof(ECC));
        }
    }
}