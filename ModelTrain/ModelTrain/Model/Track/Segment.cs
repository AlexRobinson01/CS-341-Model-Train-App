using System.Numerics;

namespace ModelTrain.Model.Track
{
    public class Segment
    {
        // Represents a piece placed on the track, containing a position, rotation,
        // and other pieces this one is snapped to, along with API for setting each

        // TODO: documentation
        public SegmentType SegmentType { get; private set; }

        public float X { get; private set; }
        public float Y { get; private set; }

        public int Rotation { get; private set; }

        public Segment? SnappedStartSegment { get; private set; }
        public Segment? SnappedEndSegment { get; private set; }

        public readonly Vector2 Size;
        public readonly Vector2 StartSnapOffset;
        public readonly Vector2 EndSnapOffset;
        public readonly int EndSnapRotationOffset;

        public Segment(SegmentType type)
        {
            SegmentType = type;

            SegmentInfo.GetMetrics(type, out Vector2 size, out Vector2 snapLengths, out Vector2 angles);

            Size = size;
            StartSnapOffset = new Vector2((float)Math.Cos(angles.X), (float)Math.Sin(angles.X)) * snapLengths.X;
            EndSnapOffset = new Vector2((float)Math.Cos(angles.Y), (float)Math.Sin(angles.Y)) * snapLengths.Y;

            EndSnapRotationOffset = (int)angles.Y;
        }

        public void MoveTo(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Rotate(int rotation)
        {
            Rotation = rotation;
        }

        public bool SnapToStart(Segment snap)
        {
            SnappedStartSegment ??= snap;
            return SnappedStartSegment == snap;
        }

        public bool SnapToEnd(Segment snap)
        {
            SnappedEndSegment ??= snap;
            return SnappedEndSegment == snap;
        }

        public void UnsnapStart()
        {
            SnappedStartSegment = null;
        }

        public void UnsnapEnd()
        {
            SnappedEndSegment = null;
        }
    }
}
