using ModelTrain.Model.Track;
using System.Collections.ObjectModel;

namespace ModelTrain.Model.Pieces
{
    public class PieceInfo
    {
        public static void GetInfo(SegmentType type, out string name, out string image)
        {
            name = type switch
            {
                SegmentType.Straight => "Straight",
                SegmentType.Curve15 => "Curve15",
                SegmentType.Curve30 => "Curve30",
                SegmentType.Curve45 => "Curve45",
                SegmentType.Curve60 => "Curve60",
                SegmentType.Curve75 => "Curve75",
                SegmentType.Curve90 => "Curve90",
                _ => ""
            };

            image = type switch
            {
                SegmentType.Straight => "piece_straight.png",
                SegmentType.Curve15 => "piece_curve15.png",
                SegmentType.Curve30 => "piece_curve30.png",
                SegmentType.Curve45 => "piece_curve45.png",
                SegmentType.Curve60 => "piece_curve60.png",
                SegmentType.Curve75 => "piece_curve75.png",
                SegmentType.Curve90 => "piece_curve90.png",
                _ => ""
            };
        }

        public static PieceList GetDefaultPieces()
        {
            PieceList pieces = new();

            foreach (SegmentType type in Enum.GetValues(typeof(SegmentType)))
                pieces.Add(new(type));

            return pieces;
        }
    }
}
