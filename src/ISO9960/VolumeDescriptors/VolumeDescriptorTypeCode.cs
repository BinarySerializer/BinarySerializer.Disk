namespace BinarySerializer.Disk.ISO9960
{
    public enum VolumeDescriptorTypeCode : byte
    {
        BootRecord = 0,
        PrimaryVolumeDescriptor = 1,
        SupplementaryVolumeDescriptor = 2,
        VolumePartitionDescriptor = 3,
        VolumeDescriptorSetTerminator = 0xFF,
    }
}