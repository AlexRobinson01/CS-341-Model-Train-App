using System.Numerics;

namespace ModelTrain.Model.Track
{
    public enum SegmentType
    {
        // TODO: add more angles
        Straight,
        Curve15,
        Curve30,
        Curve45,
        Curve60,
        Curve75,
        Curve90
    }

    public class SegmentMetrics
    {
        public static void GetFromType(SegmentType type, out Vector2 size, out Vector2 snapLengths, out Vector2 angles)
        {
            int angle;

            switch (type)
            {
                case SegmentType.Curve15:
                    angle = 15;
                    break;
                case SegmentType.Curve30:
                    angle = 30;
                    break;
                case SegmentType.Curve45:
                    angle = 45;
                    break;
                case SegmentType.Curve60:
                    angle = 60;
                    break;
                case SegmentType.Curve75:
                    angle = 75;
                    break;
                case SegmentType.Curve90:
                    angle = 90;
                    break;
                case SegmentType.Straight:
                default:
                    angle = 0;
                    break;
            }

            // may change later depending on curve, but for now this is fine
            size = Vector2.One;
            snapLengths = Vector2.One;
            angles = new(0, 180 + angle);
        }
    }
}
