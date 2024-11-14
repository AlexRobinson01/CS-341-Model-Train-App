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
                SegmentType.Curve15 => "piece_15curve.png",
                SegmentType.Curve30 => "piece_30curve.png",
                SegmentType.Curve45 => "piece_45curve.png",
                SegmentType.Curve60 => "piece_60curve.png",
                SegmentType.Curve75 => "piece_75curve.png",
                SegmentType.Curve90 => "piece_90curve.png",
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
