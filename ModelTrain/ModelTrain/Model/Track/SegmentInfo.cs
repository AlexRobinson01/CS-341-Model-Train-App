using System.Numerics;

namespace ModelTrain.Model.Track
{
    public class SegmentInfo
    {
        public static void GetMetrics(SegmentType type, out Vector2 size, out Vector2 snapLengths, out Vector2 angles)
        {
            // May change later depending on curve, but for now this is fine
            size = Vector2.One;
            snapLengths = Vector2.One;

            // SegmentType maps to the angle associated with that type
            angles = new(0, 180 + type switch
            {
                SegmentType.Straight => 0,
                SegmentType.Curve15 => 15,
                SegmentType.Curve30 => 30,
                SegmentType.Curve45 => 45,
                SegmentType.Curve60 => 60,
                SegmentType.Curve75 => 75,
                SegmentType.Curve90 => 90,
                _ => 0
            });
        }
    }
}
