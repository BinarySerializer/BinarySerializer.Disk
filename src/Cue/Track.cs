#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace BinarySerializer.Disk.Cue
{
    public class Track
    {
        #region Contructors

        public Track(int tracknumber, string datatype)
        {
            TrackNumber = tracknumber;
            TrackDataType = datatype.Trim().ToUpper() switch
            {
                "AUDIO" => DataType.Audio,
                "CDG" => DataType.Cdg,
                "MODE1/2048" => DataType.Mode1_2048,
                "MODE1/2352" => DataType.Mode1_2352,
                "MODE2/2336" => DataType.Mode2_2336,
                "MODE2/2352" => DataType.Mode2_2352,
                "CDI/2336" => DataType.Cdi_2336,
                "CDI/2352" => DataType.Cdi_2352,
                _ => DataType.Audio
            };

            Songwriter = String.Empty;
            Title = String.Empty;
            ISRC = String.Empty;
            Performer = String.Empty;
            PreGap = new Index(-1, 0, 0, 0);
            PostGap = new Index(-1, 0, 0, 0);
            DataFile = null;
        }

        public Track(int tracknumber, DataType datatype)
        {
            TrackNumber = tracknumber;
            TrackDataType = datatype;

            Songwriter = String.Empty;
            Title = String.Empty;
            ISRC = String.Empty;
            Performer = String.Empty;
            PreGap = new Index(-1, 0, 0, 0);
            PostGap = new Index(-1, 0, 0, 0);
            DataFile = null;
        }

        #endregion

        #region Public Properties

        public List<string> Comments { get; } = new();
        public DataFile? DataFile { get; set; }
        public List<string> UnsupportedLines { get; } = new();
        public List<Index> Indexes { get; } = new();
        public string ISRC { get; set; }
        public string Performer { get; set; }
        public Index PostGap { get; set; }
        public Index PreGap { get; set; }
        public string Songwriter { get; set; }
        public string Title { get; set; }
        public DataType TrackDataType { get; set; }
        public TrackFlags Flags { get; set; }
        public int TrackNumber { get; set; }

        #endregion

        #region Methods

        public void AddFlag(string flag)
        {
            switch (flag.ToUpper())
            {
                case "DATA":
                    Flags |= TrackFlags.Data;
                    break;

                case "DCP":
                    Flags |= TrackFlags.Dcp;
                    break;

                case "4CH":
                    Flags |= TrackFlags.Ch4;
                    break;

                case "PRE":
                    Flags |= TrackFlags.Pre;
                    break;

                case "SCMS":
                    Flags |= TrackFlags.Scms;
                    break;
            }
        }

        public override string ToString()
        {
            StringBuilder output = new();

            if (DataFile != null && !String.IsNullOrWhiteSpace(DataFile.Filename))
                output.AppendLine($"FILE \"{DataFile.Filename.Trim()}\" {DataFile.GetFileTypeString()}");

            output.Append($@"  TRACK {TrackNumber.ToString().PadLeft(2, '0')} {TrackDataType switch
            {
                DataType.Audio => "AUDIO", 
                DataType.Cdg => "CDG", 
                DataType.Mode1_2048 => "MODE1/2048", 
                DataType.Mode1_2352 => "MODE1/2352", 
                DataType.Mode2_2336 => "MODE2/2336", 
                DataType.Mode2_2352 => "MODE2/2352", 
                DataType.Cdi_2336 => "CDI/2336",
                DataType.Cdi_2352 => "CDI/2352",
                _ => throw new ArgumentOutOfRangeException(nameof(TrackDataType), TrackDataType, null)
            }}");

            foreach (string comment in Comments)
                output.Append($"{Environment.NewLine}    REM {comment}");

            if (!String.IsNullOrWhiteSpace(Performer))
                output.Append($"{Environment.NewLine}    PERFORMER \"{Performer}\"");

            if (!String.IsNullOrWhiteSpace(Songwriter))
                output.Append($"{Environment.NewLine}    SONGWRITER \"{Songwriter}\"");

            if (!String.IsNullOrWhiteSpace(Title))
                output.Append($"{Environment.NewLine}    TITLE \"{Title}\"");

            if (Flags != TrackFlags.None)
            {
                output.Append($"{Environment.NewLine}    FLAGS");

                if ((Flags & TrackFlags.Data) != 0)
                    output.Append(" DATA");

                if ((Flags & TrackFlags.Dcp) != 0)
                    output.Append(" DCP");

                if ((Flags & TrackFlags.Ch4) != 0)
                    output.Append(" 4CH");

                if ((Flags & TrackFlags.Pre) != 0)
                    output.Append(" PRE");

                if ((Flags & TrackFlags.Scms) != 0)
                    output.Append(" SCMS");
            }

            if (!String.IsNullOrWhiteSpace(ISRC))
                output.Append($"{Environment.NewLine}    ISRC {ISRC.Trim()}");

            if (PreGap.Number != -1)
                output.Append($"{Environment.NewLine}    PREGAP {PreGap.Minutes.ToString().PadLeft(2, '0')}:{PreGap.Seconds.ToString().PadLeft(2, '0')}:{PreGap.Frames.ToString().PadLeft(2, '0')}");

            foreach (Index index in Indexes)
                output.Append($"{Environment.NewLine}    INDEX {index.Number.ToString().PadLeft(2, '0')} {index.Minutes.ToString().PadLeft(2, '0')}:{index.Seconds.ToString().PadLeft(2, '0')}:{index.Frames.ToString().PadLeft(2, '0')}");

            if (PostGap.Number != -1)
                output.Append($"{Environment.NewLine}    POSTGAP {PostGap.Minutes.ToString().PadLeft(2, '0')}:{PostGap.Seconds.ToString().PadLeft(2, '0')}:{PostGap.Frames.ToString().PadLeft(2, '0')}");

            return output.ToString();
        }

        #endregion
    }
}