using System.Numerics;

namespace ModelTrain.Model.Track
{
    public enum SegmentType
    {

    }

    public class SegmentMetrics
    {
        public static (Vector2?, Vector2?, Vector2?) GetFromType(SegmentType type)
        {
            return type switch
            {
                _ => (null, null, null)
            };
        }
    }
}
