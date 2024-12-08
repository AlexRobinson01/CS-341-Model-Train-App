using System.Numerics;

namespace ModelTrain.Model.Track
{
    /**
     * Description: Represents a piece placed on the track, containing info regarding its position,
     * rotation, snapped pieces, and how it snaps to other pieces
     * Author: Alex Robinson
     * Last updated: 12/7/2024
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

        /// <summary>
        /// Gets the position of the start snap point relative to the canvas rather than the segment
        /// </summary>
        /// <returns>A Vector2 position of this segment's start snap point</returns>
        public Vector2 GetStartSnapPosition()
        {
            Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(-MathF.PI * Rotation / 180);
            return Vector2.Transform(StartSnapOffset, rotationMatrix) + new Vector2(X, Y);
        }

        /// <summary>
        /// Gets the position where a segment should be placed to snap to this segment's start snap point
        /// </summary>
        /// <returns>A Vector2 position for placing a segment to this segment's start snap point</returns>
        public Vector2 GetExtendedStartSnapPosition()
        {
            Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(-MathF.PI * Rotation / 180);
            return 2 * Vector2.Transform(StartSnapOffset, rotationMatrix) + new Vector2(X, Y);
        }

        /// <summary>
        /// Gets the position of the end snap point relative to the canvas rather than the segment
        /// </summary>
        /// <returns>A Vector2 position of this segment's end snap point</returns>
        public Vector2 GetEndSnapPosition()
        {
            Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(-MathF.PI * Rotation / 180);
            return Vector2.Transform(EndSnapOffset, rotationMatrix) + new Vector2(X, Y);
        }

        /// <summary>
        /// Gets the position where a segment should be placed to snap to this segment's end snap point
        /// </summary>
        /// <returns>A Vector2 position for placing a segment to this segment's end snap point</returns>
        public Vector2 GetExtendedEndSnapPosition()
        {
            Matrix3x2 rotationMatrix = Matrix3x2.CreateRotation(-MathF.PI * Rotation / 180);
            return 2 * Vector2.Transform(EndSnapOffset, rotationMatrix) + new Vector2(X, Y);
        }
    }
}
