using System.Numerics;

namespace ModelTrain.Model.Track
{
    /**
     * Description: Holds a list of Segments and ways to convert between segments and strings
     * to allow for saving, loading, and edit history
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public class TrackBase
    {
        // The list of segments this track holds
        public readonly List<Segment> Segments = [];
        // An event that gets fired after calling LoadSegmentsFromString()
        public event EventHandler? OnTrackReload;

        /// <summary>
        /// Get a string representing the current list of Segments
        /// </summary>
        /// <returns>A string representing the current list of Segments</returns>
        public string GetSegmentsAsString()
        {
            // Return a Base64 representation of the data in the current segments

            try
            {
                // Format:
                // Byte:  Segment type
                // Float: X position
                // Float: Y position
                // Short: Degrees of rotation
                // Int32: Index of snapped start segment
                // Int32: Index of snapped end segment

                MemoryStream stream = new();
                BinaryWriter writer = new(stream);

                // Writes the segments to the BinaryWriter based on the format given above
                for (int i = 0; i < Segments.Count; i++)
                {
                    Segment segment = Segments[i];

                    writer.Write((byte)segment.SegmentType);

                    writer.Write(segment.X);
                    writer.Write(segment.Y);
                    writer.Write((short)segment.Rotation);

                    // -1: nothing is snapped to that side
                    writer.Write(segment.SnappedStartSegment == null ? -1 : Segments.IndexOf(segment.SnappedStartSegment));
                    writer.Write(segment.SnappedEndSegment == null ? -1 : Segments.IndexOf(segment.SnappedEndSegment));
                }

                byte[] bytes = stream.ToArray();
                writer.Close();
                stream.Close();

                return Convert.ToBase64String(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to convert segments to string:\n{e}");
                return "";
            }
        }

        /// <summary>
        /// Replaces the current list of Segments with the list represented by the given string
        /// </summary>
        /// <param name="segmentsStr">The string representation of a list of Segments to load into the track</param>
        /// <returns>Whether the operation succeeded</returns>
        public bool LoadSegmentsFromString(string segmentsStr)
        {
            // Take in a Base64 representation of segment data and replace the current segments
            // with that data
            Segments.Clear();

            try
            {
                // Format:
                // Byte:  Segment type
                // Float: X position
                // Float: Y position
                // Short: Degrees of rotation
                // Int32: Index of snapped start segment
                // Int32: Index of snapped end segment

                byte[] bytes = Convert.FromBase64String(segmentsStr);

                MemoryStream stream = new(bytes);
                BinaryReader reader = new(stream);

                // Holds snapped indices for each Segment from the string
                List<(Segment, Vector2)> set = [];

                // Load the Segments from the given string using the format above
                while (stream.Position + 1 < stream.Length)
                {
                    SegmentType type = (SegmentType)reader.ReadByte();
                    Segment segment = new(type)
                    {
                        X = reader.ReadSingle(),
                        Y = reader.ReadSingle(),

                        Rotation = reader.ReadInt16()
                    };

                    // Store snapped indices for later
                    set.Add((segment, new(reader.ReadInt32(), reader.ReadInt32())));
                    Segments.Add(segment);
                }

                // Go back through the list of Segments, snapping them together as needed
                for (int i = 0; i < set.Count; i++)
                {
                    Segment segment = set[i].Item1;
                    Vector2 snapped = set[i].Item2;

                    if (snapped.X >= 0)
                        segment.SnappedStartSegment = Segments[(int)snapped.X];
                    if (snapped.Y >= 0)
                        segment.SnappedEndSegment = Segments[(int)snapped.Y];
                }

                reader.Close();
                stream.Close();
                // Reload the linked TrackEditor if one linked itself to this track
                OnTrackReload?.Invoke(this, new());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load segments from string:\n{e}");
                Segments.Clear();
                // Reload the linked TrackEditor if one linked itself to this track
                OnTrackReload?.Invoke(this, new());
                return false;
            }
        }

        /// <summary>
        /// Adds a given Segment to the stored list
        /// </summary>
        /// <param name="segment">The Segment to add to the list</param>
        public void AddSegment(Segment segment)
        {
            Segments.Add(segment);
        }

        /// <summary>
        /// Removes a given Segment from the stored list
        /// </summary>
        /// <param name="segment">The Segment to remove from the list</param>
        public void RemoveSegment(Segment segment)
        {
            Segments.Remove(segment);
        }
    }
}
