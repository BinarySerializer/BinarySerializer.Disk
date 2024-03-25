using System;

namespace BinarySerializer.Disk.Cue
{
    [Flags]
    public enum TrackFlags
    {
        None = 0,

        /// <summary>
        /// Digital copy permitted
        /// </summary>
        Dcp = 1 << 0,

        /// <summary>
        /// Four channel audio
        /// </summary>
        Ch4 = 1 << 1,

        /// <summary>
        /// Pre-emphasis enabled (audio tracks only)
        /// </summary>
        Pre = 1 << 2,

        /// <summary>
        /// Serial copy management system (not supported by all recorders)
        /// </summary>
        Scms = 1 << 3,

        /// <summary>
        /// Set for all non-audio tracks. This flag is set automatically based on the datatype of the track.
        /// </summary>
        Data = 1 << 4,
    }
}