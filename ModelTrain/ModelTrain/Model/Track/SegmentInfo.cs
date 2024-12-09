using System.Numerics;

namespace ModelTrain.Model.Track
{
    /**
     * Description: A static method to get the default size, snap length, and angle metrics
     * for a Segment
     * Author: Alex Robinson
     * Last updated: 12/8/2024
     */
    public static class SegmentInfo
    {
        /// <summary>
        /// Retrieves default size, snap lengths, and angles for a given SegmentType
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="snapLengths"></param>
        /// <param name="angles"></param>
        public static void GetMetrics(SegmentType type,
            out Vector2 size, out Vector2 snapLengths, out Vector2 angles)
        {
            // May change later depending on curve, but for now this is fine
            size = Vector2.One * 100;
            snapLengths = Vector2.One * 50;

            // SegmentType maps to the angle associated with that type
            angles = new Vector2(0, 180 - type switch
            {
                SegmentType.Straight => 0,
                //SegmentType.Curve15 => 15,
                SegmentType.Curve30 => 30,
                SegmentType.Curve45 => 45,
                SegmentType.Curve60 => 60,
                //SegmentType.Curve75 => 75,
                SegmentType.Curve90 => 90,
                _ => 0
            }) + new Vector2(90, 90);
        }
    }
}
