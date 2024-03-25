namespace BinarySerializer.Disk.Cue
{
    public enum FileType
    {
        /// <summary>
        /// Intel binary file (least significant byte first)
        /// </summary>
        Binary,

        /// <summary>
        /// MOTOROLA - Motorola binary file (most significant byte first)
        /// </summary>
        Motorola,

        /// <summary>
        /// AIFF - Audio AIFF file
        /// </summary>
        Aiff,

        /// <summary>
        /// Audio WAVE file
        /// </summary>
        Wave,

        /// <summary>
        /// Audio MP3 file
        /// </summary>
        Mp3,
    }
}