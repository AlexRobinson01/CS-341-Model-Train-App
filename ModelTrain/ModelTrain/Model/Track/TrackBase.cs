using System.Numerics;

namespace ModelTrain.Model.Track
{
    public class TrackBase
    {
        public string ID { get; private set; }

        private readonly List<Segment> segments;

        public TrackBase(string id)
        {
            segments = new();
            ID = id;
        }

        public string GetSegmentsAsString()
        {
            try
            {
                MemoryStream stream = new();
                BinaryWriter writer = new(stream);

                for (int i = 0; i < segments.Count; i++)
                {
                    Segment segment = segments[i];

                    writer.Write((byte)segment.SegmentType);

                    writer.Write(segment.X);
                    writer.Write(segment.Y);
                    writer.Write((short)segment.Rotation);

                    writer.Write(segment.SnappedStartSegment == null ? -1 : segments.IndexOf(segment.SnappedStartSegment));
                    writer.Write(segment.SnappedEndSegment == null ? -1 : segments.IndexOf(segment.SnappedEndSegment));
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
            segments.Clear();
            
            try
            {
                byte[] bytes = Convert.FromBase64String(segmentsStr);

                MemoryStream stream = new(bytes);
                BinaryReader reader = new(stream);

                List<(Segment, Vector2)> set = new();

                while (stream.Position + 1 < stream.Length)
                {
                    SegmentType type = (SegmentType)reader.ReadByte();
                    Segment segment = new(type);

                    segment.MoveTo(reader.ReadSingle(), reader.ReadSingle());
                    segment.Rotate(reader.ReadInt16());

                    set.Add((segment, new(reader.ReadInt32(), reader.ReadInt32())));
                    segments.Add(segment);
                }

                for (int i = 0; i < set.Count; i++)
                {
                    Segment segment = set[i].Item1;
                    Vector2 snapped = set[i].Item2;

                    if (snapped.X >= 0)
                        segment.SnapToStart(segments[(int)snapped.X]);
                    if (snapped.Y >= 0)
                        segment.SnapToEnd(segments[(int)snapped.Y]);
                }

                reader.Close();
                stream.Close();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load segments from string:\n{e}");
                segments.Clear();

                return false;
            }
        }
    }
}
