using System.Collections.ObjectModel;

namespace ModelTrain.Model.Pieces
{
    // A wrapper for an ObservableCollection that allows rotating the list

    public class PieceList : ObservableCollection<PieceBase>
    {
        public void RotateLeft()
        {
            PieceBase first = this[0];
            RemoveAt(0);
            Add(first);
        }

        public void RotateRight()
        {
            PieceBase last = this[Count - 1];
            RemoveAt(Count - 1);
            Insert(0, last);
        }
    }
}
