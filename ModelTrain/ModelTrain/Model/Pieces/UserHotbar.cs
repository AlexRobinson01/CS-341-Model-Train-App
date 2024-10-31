using System.Collections.ObjectModel;

namespace ModelTrain.Model.Pieces
{
    public class UserHotbar
    {
        public static ObservableCollection<PieceBase> Pieces { get; private set; }

        private static ILocalDatabase localDatabase;

        static UserHotbar()
        {
            localDatabase = new LocalDatabase();
            Pieces = localDatabase.Pieces;
        }
        
        public static void UpdatePiece(int index, PieceBase piece)
        {

        }

        public static void AddPiece(int index, PieceBase piece)
        {

        }

        public static void RemovePiece(int index)
        {

        }
    }
}
