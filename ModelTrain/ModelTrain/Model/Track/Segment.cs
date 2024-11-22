using System.Numerics;

namespace ModelTrain.Model.Track
{
    public class Segment
    {
        // Represents a piece placed on the track, containing a position, rotation,
        // and other pieces this one is snapped to

        // TODO: documentation
        public SegmentType SegmentType { get; private set; }

        public float X { get; set; }
        public float Y { get; set; }

        public int Rotation { get; set; }

        public Segment? SnappedStartSegment { get; set; }
        public Segment? SnappedEndSegment { get; set; }

        public readonly Vector2 Size;
        public readonly Vector2 StartSnapOffset;
        public readonly Vector2 EndSnapOffset;
        public readonly Vector2 SnapRotationOffset;

        public Segment(SegmentType type)
        {
            SegmentType = type;

            SegmentInfo.GetMetrics(type, out Vector2 size, out Vector2 snapLengths, out Vector2 angles);
            float radX = MathF.PI * angles.X / 180f;
            float radY = MathF.PI * angles.Y / 180f;

            Size = size;
            StartSnapOffset = new Vector2((float)Math.Cos(radX), (float)Math.Sin(radX)) * snapLengths.X;
            EndSnapOffset = new Vector2((float)Math.Cos(radY), (float)Math.Sin(radY)) * snapLengths.Y;
            SnapRotationOffset = angles;
        }
    }
}
