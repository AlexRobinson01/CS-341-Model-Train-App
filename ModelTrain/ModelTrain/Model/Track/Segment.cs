using System.Numerics;

namespace ModelTrain.Model.Track
{
    /**
     * Description: Represents a piece placed on the track, containing info regarding its position,
     * rotation, snapped pieces, and how it snaps to other pieces
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public class Segment
    {
        // The SegmentType to assign to this Segment, used for defining default snap offsets
        public SegmentType SegmentType { get; private set; }

        // X/Y positions on the track editor's canvas
        public float X { get; set; }
        public float Y { get; set; }
        // This segment's rotation in degrees
        public int Rotation { get; set; }
        // This segment's rotation in radians (derived from degrees), readonly
        public float RotationRads => MathF.PI * Rotation / 180f;

        // The Segments this one is snapped to, or null if nothing is snapped to a side
        public Segment? SnappedStartSegment { get; set; }
        public Segment? SnappedEndSegment { get; set; }

        // The size of the segment to be rendered on the track
        public readonly Vector2 Size;
        // The offsets from the center of this segment where other segments should snap to
        public readonly Vector2 StartSnapOffset;
        public readonly Vector2 EndSnapOffset;
        // The rotations of each snap point to control how snapped segments should be oriented
        // X: Start rotation, Y: End rotation
        public readonly Vector2 SnapRotationOffset;

        /// <summary>
        /// Segment constructor - Assigns a SegmentType for this Segment which defines default values
        /// </summary>
        /// <param name="type">The SegmentType to assign to this Segment</param>
        public Segment(SegmentType type)
        {
            SegmentType = type;

            // Default metrics for the snap lengths and angles for snap positions
            SegmentInfo.GetMetrics(type, out Vector2 size, out Vector2 snapLengths, out Vector2 angles);
            // Radians are easier to use in trig methods
            float radX = MathF.PI * angles.X / 180f;
            float radY = MathF.PI * angles.Y / 180f;

            Size = size;
            // The unit circle shows up again! (cos(r), sin(r)) * length = snap offset
            StartSnapOffset = new Vector2((float)Math.Cos(radX), (float)Math.Sin(radX)) * snapLengths.X;
            EndSnapOffset = new Vector2((float)Math.Cos(radY), (float)Math.Sin(radY)) * snapLengths.Y;
            SnapRotationOffset = angles;
        }
    }
}
