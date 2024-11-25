using ModelTrain.Model.Track;
using System.Numerics;

namespace ModelTrain.Model.Pieces
{
    /**
     * Description: A container for image data associated with whatever SegmentType is assigned to it
     * Author: Alex Robinson
     * Last updated: 11/23/2024
     */
    public class PieceBase
    {
        public SegmentType SegmentType { get; private set; }
        public string Name { get; private set; }
        public string Image { get; private set; }

        // Image metrics that define how the image will be rendered on buttons and the track
        public float ImageRotation { get; private set; }
        public float ImageScale { get; private set; }
        public Vector2 ImageOffset { get; private set; }

        /// <summary>
        /// PieceBase constructor - sets default values to those assigned to the given SegmentType
        /// </summary>
        /// <param name="type">The SegmentType to assign to this piece</param>
        public PieceBase(SegmentType type)
        {
            SegmentType = type;

            // Default name and image are dependent on the segment type, which are stored in PieceInfo
            PieceInfo.GetInfo(type, out string name, out string image);
            Name = name;
            Image = image;
        }

        /// <summary>
        /// Updates the current PieceBase's image rotation, scale, and offset to the given values
        /// </summary>
        /// <param name="rotation">The new rotation to apply to this piece's image</param>
        /// <param name="scale">The new scale to apply to this piece's image</param>
        /// <param name="offset">The new offset to apply to this piece's image</param>
        public void UpdateImageRSO(float rotation, float scale, Vector2 offset)
        {
            ImageRotation = rotation;
            ImageScale = scale;
            ImageOffset = offset;
        }
    }
}
