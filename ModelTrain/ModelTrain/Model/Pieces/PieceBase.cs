using ModelTrain.Model.Track;
using System.Numerics;

namespace ModelTrain.Model.Pieces
{
    public class PieceBase
    {
        private SegmentType segmentType;
        private string image;

        private int imageRotation;
        private int imageScale;
        private Vector2 imageOffset;

        public PieceBase()
        {

        }

        public void SetSegment(SegmentType type)
        {
            segmentType = type;
        }
    }
}
