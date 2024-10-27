using ModelTrain.Model.Pieces;
using System.Collections.ObjectModel;

namespace ModelTrain.Model
{
    public interface ILocalDatabase
    {
        public ObservableCollection<PieceBase> Pieces { get; set; }
    }
}
