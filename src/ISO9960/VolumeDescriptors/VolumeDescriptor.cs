using System;

namespace BinarySerializer.Disk.ISO9960
{
    public class VolumeDescriptor : BinarySerializable
    {
        public VolumeDescriptorTypeCode TypeCode { get; set; }
        public byte Version { get; set; }

        // Types
        public PrimaryVolumeDescriptor PrimaryVolumeDescriptor { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TypeCode = s.Serialize<VolumeDescriptorTypeCode>(TypeCode, name: nameof(TypeCode));
            s.SerializeMagicString("CD001", 5);
            Version = s.Serialize<byte>(Version, name: nameof(Version));

            if (Version != 1)
                throw new UnsupportedFormatVersionException(this, $"Unsupported volume descriptor version {Version}");

            switch (TypeCode)
            {
                case VolumeDescriptorTypeCode.BootRecord:
                    throw new NotImplementedException();
                
                case VolumeDescriptorTypeCode.PrimaryVolumeDescriptor:
                    PrimaryVolumeDescriptor = s.SerializeObject<PrimaryVolumeDescriptor>(PrimaryVolumeDescriptor, name: nameof(PrimaryVolumeDescriptor));
                    break;
                
                case VolumeDescriptorTypeCode.SupplementaryVolumeDescriptor:
                    throw new NotImplementedException();

                case VolumeDescriptorTypeCode.VolumePartitionDescriptor:
                    throw new NotImplementedException();

                case VolumeDescriptorTypeCode.VolumeDescriptorSetTerminator:
                    // Do nothing
                    break;

                default:
                    throw new BinarySerializableException(this, $"Unsupported volume descriptor type code {TypeCode}");
            }
        }
    }
}