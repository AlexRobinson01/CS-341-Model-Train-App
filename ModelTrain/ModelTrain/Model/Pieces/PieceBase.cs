using ModelTrain.Model.Track;
using System.Numerics;

namespace ModelTrain.Model.Pieces
{
    public class PieceBase
    {
        public SegmentType SegmentType { get; private set; }
        public string Name { get; private set; }
        public string Image { get; private set; }

        public float ImageRotation { get; private set; }
        public float ImageScale { get; private set; }
        public Vector2 ImageOffset { get; private set; }

        public PieceBase(SegmentType type)
        {
            SegmentType = type;
            
            PieceInfo.GetInfo(type, out string name, out string image);
            Name = name;
            Image = image;
        }

        public void UpdateImageRSO(float rotation, float scale, Vector2 offset)
        {
            ImageRotation = rotation;
            ImageScale = scale;
            ImageOffset = offset;
        }
    }
}
