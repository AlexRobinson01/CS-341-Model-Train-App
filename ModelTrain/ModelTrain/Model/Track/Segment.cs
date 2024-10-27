using System.Numerics;

namespace ModelTrain.Model.Track
{
    public class Segment
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public int Rotation { get; private set; }

        private Segment? snappedStartSegment;
        private Segment? snappedEndSegment;

        public readonly Vector2 Size = Vector2.One;
        public readonly Vector2 StartSnapOffset = new(0, 0.5f);
        public readonly Vector2 EndSnapOffset = new(0, -0.5f);
        private readonly int endSnapRotationOffset = 0;

        public Segment(Vector2? size = null, Vector2? startSnapOffset = null, Vector2? endSnapOffset = null)
        {
            Size = size ?? Size;
            StartSnapOffset = startSnapOffset ?? StartSnapOffset;
            EndSnapOffset = endSnapOffset ?? EndSnapOffset;

            // TODO: do some trig to find the end rotation
        }

        public static Segment FromType(SegmentType type)
        {
            (Vector2?, Vector2?, Vector2?) metrics = SegmentMetrics.GetFromType(type);
            return new(metrics.Item1, metrics.Item2, metrics.Item3);
        }

        public void MoveTo(int x, int y)
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
            snappedStartSegment ??= snap;
            return snappedStartSegment == snap;
        }

        public bool SnapToEnd(Segment snap)
        {
            snappedEndSegment ??= snap;
            return snappedEndSegment == snap;
        }

        public void UnsnapStart()
        {
            snappedStartSegment = null;
        }

        public void UnsnapEnd()
        {
            snappedEndSegment = null;
        }
    }
}
