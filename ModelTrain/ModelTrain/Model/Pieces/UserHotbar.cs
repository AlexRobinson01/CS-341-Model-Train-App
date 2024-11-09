using ModelTrain.Model.Track;
using System.Collections.ObjectModel;

namespace ModelTrain.Model.Pieces
{
    public class UserHotbar
    {
        public static ObservableCollection<PieceBase> Pieces { get; private set; }

        private static readonly ILocalDatabase localDatabase;

        static UserHotbar()
        {
            localDatabase = new LocalDatabase();
            Pieces = localDatabase.Pieces;
        }
        
        public static void MovePiece(int oldIndex, int newIndex)
        {
            Pieces.Move(oldIndex, newIndex);
        }

        public static void AddPiece(int index, SegmentType segmentType)
        {
            if (Pieces.FirstOrDefault(n => n.SegmentType == segmentType) == null)
                Pieces.Insert(index, new(segmentType));
        }

        public static void RemovePiece(int index)
        {
            Pieces.RemoveAt(index);
        }
    }
}
