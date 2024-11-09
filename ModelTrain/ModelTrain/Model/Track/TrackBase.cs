using System.Numerics;

namespace ModelTrain.Model.Track
{
    public class TrackBase
    {
        // The list of segments this track holds
        public readonly List<Segment> Segments = new();
        // An event that gets fired after calling LoadSegmentsFromString()
        public event EventHandler OnTrackReload;

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

                // TODO: better documentation
                MemoryStream stream = new();
                BinaryWriter writer = new(stream);

                for (int i = 0; i < Segments.Count; i++)
                {
                    Segment segment = Segments[i];

                    writer.Write((byte)segment.SegmentType);

                    writer.Write(segment.X);
                    writer.Write(segment.Y);
                    writer.Write((short)segment.Rotation);

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

                // TODO: better documentation
                byte[] bytes = Convert.FromBase64String(segmentsStr);

                MemoryStream stream = new(bytes);
                BinaryReader reader = new(stream);

                List<(Segment, Vector2)> set = new();

                while (stream.Position + 1 < stream.Length)
                {
                    SegmentType type = (SegmentType)reader.ReadByte();
                    Segment segment = new(type)
                    {
                        X = reader.ReadSingle(),
                        Y = reader.ReadSingle(),

                        Rotation = reader.ReadInt16()
                    };

                    set.Add((segment, new(reader.ReadInt32(), reader.ReadInt32())));
                    Segments.Add(segment);
                }

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

                OnTrackReload.Invoke(this, new());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load segments from string:\n{e}");
                Segments.Clear();

                OnTrackReload.Invoke(this, new());
                return false;
            }
        }

        public void AddSegment(Segment segment)
        {
            Segments.Add(segment);
        }

        public void RemoveSegment(Segment segment)
        {
            Segments.Remove(segment);
        }
    }
}
