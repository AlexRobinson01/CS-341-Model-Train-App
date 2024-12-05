using ModelTrain.Model.Track;

namespace ModelTrain.Model.Pieces
{
    /**
     * Description: A static container for a list of pieces and a local database that it communicates with
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public static class UserHotbar
    {
        public static PieceList Pieces { get; private set; }

        /// <summary>
        /// Static UserHotbar constructor - initializes localDatabase and Pieces
        /// </summary>
        static UserHotbar()
        {
            Pieces = PieceInfo.GetDefaultPieces();
        }
        
        /// <summary>
        /// Moves a piece in the hotbar located at an old index to a new index
        /// </summary>
        /// <param name="oldIndex">The location of the piece to move</param>
        /// <param name="newIndex">The location the piece should be moved to</param>
        public static void MovePiece(int oldIndex, int newIndex)
        {
            Pieces.Move(oldIndex, newIndex);
        }

        /// <summary>
        /// Adds a piece to the hotbar defined by its SegmentType with a given location
        /// </summary>
        /// <param name="index">The location to add a new piece at</param>
        /// <param name="segmentType">The SegmentType that the new piece should be associated with</param>
        public static void AddPiece(int index, SegmentType segmentType)
        {
            // Ensures no pieces with this SegmentType exist already, avoiding duplicates
            if (Pieces.FirstOrDefault(n => n.SegmentType == segmentType) == null)
                Pieces.Insert(index, new(segmentType));
        }

        /// <summary>
        /// Adds a piece to the hotbar defined by its SegmentType
        /// </summary>
        /// <param name="segmentType">The SegmentType that the new piece should be associated with</param>
        public static void AddPiece(SegmentType segmentType)
        {
            // Ensures no pieces with this SegmentType exist already, avoiding duplicates
            if (Pieces.FirstOrDefault(n => n.SegmentType == segmentType) == null)
                Pieces.Add(new(segmentType));
        }

        /// <summary>
        /// Removes a piece from the hotbar at a given index
        /// </summary>
        /// <param name="index">The location to remove a piece from</param>
        public static void RemovePiece(int index)
        {
            Pieces.RemoveAt(index);
        }

        /// <summary>
        /// Removes a piece from the hotbar with a given SegmentType
        /// </summary>
        /// <param name="segmentType">The SegmentType to remove from the hotbar</param>
        public static void RemovePiece(SegmentType segmentType)
        {
            Piece? piece = Pieces.FirstOrDefault(n => n.SegmentType == segmentType);

            if (piece != null)
                Pieces.Remove(piece);
        }
    }
}
