using System.Collections.ObjectModel;

namespace ModelTrain.Model.Pieces
{
    /**
     * Description: An ObservableCollection with two additional methods to allow easy rotating
     * of its elements
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public class PieceList : ObservableCollection<Piece>
    {
        /// <summary>
        /// Rotates the contents of this PieceList left, by moving the first element to the end
        /// </summary>
        public void RotateLeft()
        {
            // Rotating left moves the first item to the end
            Piece first = this[0];
            RemoveAt(0);
            Add(first);
        }

        /// <summary>
        /// Rotates the contents of this PieceList right, by moving the last element to the front
        /// </summary>
        public void RotateRight()
        {
            // Rotating right moves the last item to the front
            Piece last = this[Count - 1];
            RemoveAt(Count - 1);
            Insert(0, last);
        }
    }
}
