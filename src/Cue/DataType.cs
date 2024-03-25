namespace BinarySerializer.Disk.Cue
{
    public enum DataType
    {
        /// <summary>
        /// Audio/Music (2352)
        /// </summary>
        Audio,

        /// <summary>
        /// Karaoke CD+G (2448)
        /// </summary>
        Cdg,

        /// <summary>
        /// CDROM Mode1 Data (cooked)
        /// </summary>
        Mode1_2048,

        /// <summary>
        /// CDROM Mode1 Data (raw)
        /// </summary>
        Mode1_2352,

        /// <summary>
        /// CDROM-XA Mode2 Data
        /// </summary>
        Mode2_2336,

        /// <summary>
        /// CDROM-XA Mode2 Data
        /// </summary>
        Mode2_2352,

        /// <summary>
        /// CDI Mode2 Data
        /// </summary>
        Cdi_2336,

        /// <summary>
        /// CDI Mode2 Data
        /// </summary>
        Cdi_2352,
    }
}