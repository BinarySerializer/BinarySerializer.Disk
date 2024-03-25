#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinarySerializer.Disk.Cue
{
    /// <summary>
    /// Reads and writes .cue files
    /// </summary>
    public class CueSheet
    {
        #region Constructors

        public CueSheet() { }
        
        public CueSheet(string cueString)
        {
            using TextReader reader = new StringReader(cueString);
            ParseCueSheet(reader);
        }

        public CueSheet(TextReader reader)
        {
            ParseCueSheet(reader);
        }

        #endregion

        #region Properties

        /// <summary>
        /// A 13-digit UPC/EAN code, also referred to as the Media Catalog Number (MCN). 12-digit UPC codes should be prefixed with a "0".
        /// </summary>
        public string Catalog { get; set; } = String.Empty;

        /// <summary>
        /// A path to a file containing CD-Text info
        /// </summary>
        public string CDTextFile { get; set; } = String.Empty;

        /// <summary>
        /// Remarks/comments to be ignored
        /// </summary>
        public List<string> Comments { get; } = new();

        /// <summary>
        /// Unsupported lines in the sheet
        /// </summary>
        public List<string> UnsupportedLines { get; } = new();

        /// <summary>
        /// The performer name for CD-Text data
        /// </summary>
        public string Performer { get; set; } = String.Empty;

        /// <summary>
        /// The songwriter name for CD-Text data
        /// </summary>
        public string Songwriter { get; set; } = String.Empty;

        /// <summary>
        /// The title for CD-Text data
        /// </summary>
        public string Title { get; set; } = String.Empty;

        /// <summary>
        /// The tracks in the sheet
        /// </summary>
        public List<Track> Tracks { get; set; } = new();

        #endregion

        #region Public Static Methods

        public static CueSheet FromFile(string cuefilename)
        {
            using StreamReader reader = new(cuefilename);
            return new CueSheet(reader);
        }

        public static CueSheet FromFile(string cuefilename, Encoding encoding)
        {
            using StreamReader reader = new(cuefilename, encoding);
            return new CueSheet(reader);
        }

        #endregion

        #region Private Methods

        private void ParseCueSheet(TextReader reader)
        {
            int currentTrack = -1; // -1 is global
            DataFile? currentFile = null;

            while (reader.ReadLine() is { } line)
            {
                line = line.Trim();

                if (line == String.Empty)
                    continue;

                // Parse line
                int cmdSeparator = line.IndexOf(' ');
                string cmd = line.Substring(0, cmdSeparator).ToUpper();
                string cmdValue = line.Substring(cmdSeparator, line.Length - cmdSeparator).Trim();

                // Parse command
                switch (cmd)
                {
                    case "CATALOG":
                        if (currentTrack == -1)
                            Catalog = cmdValue.Trim('"');
                        break;

                    case "CDTEXTFILE":
                        if (currentTrack == -1)
                            CDTextFile = cmdValue.Trim('"');
                        break;

                    case "FILE":
                        int fileTypeSeparator = cmdValue.LastIndexOf(' ');
                        string fileType = cmdValue.Substring(fileTypeSeparator, cmdValue.Length - fileTypeSeparator).Trim();
                        string fileName = cmdValue.Substring(0, fileTypeSeparator).Trim().Trim('"');
                        currentFile = new DataFile(fileName, fileType);
                        break;

                    case "FLAGS":
                        if (currentTrack != -1)
                        {
                            string[] flags = cmdValue.Split(' ');
                            foreach (string flag in flags)
                            {
                                Tracks[currentTrack].AddFlag(flag.Trim());
                            }
                        }
                        break;

                    case "INDEX":
                        int indexSeparator = cmdValue.IndexOf(' ');
                        int number = Convert.ToInt32(cmdValue.Substring(0, indexSeparator));
                        string time = cmdValue.Substring(indexSeparator, cmdValue.Length - indexSeparator).Trim();
                        Tracks[currentTrack].Indexes.Add(new Index(number, time));
                        break;

                    case "ISRC":
                        if (currentTrack != -1)
                            Tracks[currentTrack].ISRC = cmdValue.Trim('"');
                        break;

                    case "PERFORMER":
                        if (currentTrack == -1)
                            Performer = cmdValue.Trim('"');
                        else
                            Tracks[currentTrack].Performer = cmdValue.Trim('"');
                        break;

                    case "POSTGAP":
                        Tracks[currentTrack].PostGap = new Index(0, cmdValue);
                        break;

                    case "PREGAP":
                        Tracks[currentTrack].PreGap = new Index(0, cmdValue);
                        break;

                    case "REM":
                        if (cmdValue != String.Empty)
                        {
                            if (currentTrack != -1)
                                Tracks[currentTrack].Comments.Add(cmdValue);
                            else
                                Comments.Add(cmdValue);
                        }
                        break;

                    case "SONGWRITER":
                        if (currentTrack == -1)
                            Songwriter = cmdValue.Trim('"');
                        else
                            Tracks[currentTrack].Songwriter = cmdValue.Trim('"');
                        break;

                    case "TITLE":
                        if (currentTrack == -1)
                            Title = cmdValue.Trim('"');
                        else
                            Tracks[currentTrack].Title = cmdValue.Trim('"');
                        break;

                    case "TRACK":
                        currentTrack++;

                        int trackSeparator = cmdValue.IndexOf(' ');
                        int trackNumber = Convert.ToInt32(cmdValue.Substring(0, trackSeparator));
                        string dataType = cmdValue.Substring(trackSeparator, cmdValue.Length - trackSeparator).Trim();

                        Tracks.Add(new Track(trackNumber, dataType));

                        if (currentFile != null)
                        {
                            Tracks[currentTrack].DataFile = currentFile;
                            currentFile = null;
                        }
                        break;

                    default:
                        if (currentTrack != -1)
                            Tracks[currentTrack].UnsupportedLines.Add(line);
                        else
                            UnsupportedLines.Add(line);
                        break;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves the cue sheet file to specified file
        /// </summary>
        /// <param name="filePath">The path of the file to save to</param>
        public void SaveToFile(string filePath)
        {
            SaveToFile(filePath, Encoding.Default);
        }

        /// <summary>
        /// Saves the cue sheet file to specified file with a specific encoding
        /// </summary>
        /// <param name="filePath">The path of the file to save to</param>
        /// <param name="encoding">The encoding used to save the file.</param>
        public void SaveToFile(string filePath, Encoding encoding)
        {
            using TextWriter tw = new StreamWriter(filePath, false, encoding);
            tw.WriteLine(ToString());
        }

        public override string ToString()
        {
            StringBuilder output = new();

            foreach (string comment in Comments)
                output.AppendLine($"REM {comment}");

            if (!String.IsNullOrWhiteSpace(Catalog))
                output.AppendLine($"CATALOG {Catalog}");

            if (!String.IsNullOrWhiteSpace(Performer))
                output.AppendLine($"PERFORMER \"{Performer}\"");

            if (!String.IsNullOrWhiteSpace(Songwriter))
                output.AppendLine($"SONGWRITER \"{Songwriter}\"");

            if (!String.IsNullOrWhiteSpace(Title))
                output.AppendLine($"TITLE \"{Title}\"");

            if (!String.IsNullOrWhiteSpace(CDTextFile))
                output.AppendLine($"CDTEXTFILE \"{CDTextFile.Trim()}\"");

            for (int i = 0; i < Tracks.Count; i++)
            {
                output.Append(Tracks[i]);

                if (i != Tracks.Count - 1)
                    output.AppendLine();
            }

            return output.ToString();
        }

        #endregion
    }
}