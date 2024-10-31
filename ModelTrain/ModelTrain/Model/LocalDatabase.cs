using ModelTrain.Model.Pieces;
using System.Collections.ObjectModel;

namespace ModelTrain.Model
{
    public class LocalDatabase : ILocalDatabase
    {
        public ObservableCollection<PieceBase> Pieces { get; set; }

        public LocalDatabase()
        {
            Pieces = new();
        }
    }
}
