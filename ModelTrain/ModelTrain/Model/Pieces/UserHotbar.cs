using ModelTrain.Model.Track;

namespace ModelTrain.Model.Pieces
{
    public class UserHotbar
    {
        public static PieceList Pieces { get; private set; }

        private static readonly ILocalDatabase localDatabase;

        static UserHotbar()
        {
            localDatabase = new LocalDatabase();
            Pieces = localDatabase.Hotbar;
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
