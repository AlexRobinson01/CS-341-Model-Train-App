using System.Numerics;

namespace ModelTrain.Model.Track
{
    public enum SegmentType
    {
        // TODO: add more angles
        Straight = 0,
        Curve15 = 15,
        Curve30 = 30,
        Curve45 = 45,
        Curve60 = 60,
        Curve75 = 75,
        Curve90 = 90
    }

    public class SegmentMetrics
    {
        public static void GetFromType(SegmentType type, out Vector2 size, out Vector2 snapLengths, out Vector2 angles)
        {
            // May change later depending on curve, but for now this is fine
            size = Vector2.One;
            snapLengths = Vector2.One;
            // SegmentType maps to the angle associated with that type
            angles = new(0, 180 + (int)type);
        }
    }
}
