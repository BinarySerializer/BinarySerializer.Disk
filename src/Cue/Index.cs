#nullable enable
using System;

namespace BinarySerializer.Disk.Cue
{
    /// <summary>
    /// Used to specify indexes (or sub-indexes) within a track
    /// </summary>
    public class Index
    {
        #region Constructor

        /// <summary>
        /// Creates an index of a track
        /// </summary>
        /// <param name="number">Index number 0-99</param>
        /// <param name="minutes">Minutes (0-99)</param>
        /// <param name="seconds">Seconds (0-59)</param>
        /// <param name="frames">Frames (0-74)</param>
        public Index(int number, int minutes, int seconds, int frames)
        {
            _number = number;

            _minutes = minutes;
            _seconds = seconds;
            _frames = frames;
        }

        public Index(int number, string time)
        {
            _number = number;

            string[] components = time.Split(':');

            if (components.Length < 3)
                throw new Exception("Invalid format for index time");

            _minutes = Convert.ToInt32(components[0]);
            _seconds = Convert.ToInt32(components[1]);
            _frames = Convert.ToInt32(components[2]);
        }

        #endregion

        #region Private Fields

        private int _number;
        private int _minutes;
        private int _seconds;
        private int _frames;

        #endregion

        #region Public Properties

        /// <summary>
        /// Index number (0-99)
        /// </summary>
        public int Number
        {
            get => _number;
            set => _number = value switch
            {
                > 99 => 99,
                < 0 => 0,
                _ => value
            };
        }

        /// <summary>
        /// Possible values: 0-99
        /// </summary>
        public int Minutes
        {
            get => _minutes;
            set => _minutes = value switch
            {
                > 99 => 99,
                < 0 => 0,
                _ => value
            };
        }

        /// <summary>
        /// Possible values: 0-59
        /// There are 60 seconds/minute
        /// </summary>
        public int Seconds
        {
            get => _seconds;
            set => _seconds = value switch
            {
                >= 60 => 59,
                < 0 => 0,
                _ => value
            };
        }

        /// <summary>
        /// Possible values: 0-74
        /// There are 75 frames/second
        /// </summary>
        public int Frames
        {
            get => _frames;
            set => _frames = value switch
            {
                >= 75 => 74,
                < 0 => 0,
                _ => value
            };
        }

        #endregion
    }
}