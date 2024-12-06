using ModelTrain.Model.Track;

namespace ModelTrain.Model.Pieces
{
    /**
     * Description: A static container for a list of pieces and communication with user preferences
     * Author: Alex Robinson
     * Last updated: 12/5/2024
     */
    public static class UserHotbar
    {
        public static PieceList Pieces { get; private set; }

        /// <summary>
        /// Static UserHotbar constructor - initializes Pieces with saved preferences
        /// </summary>
        static UserHotbar()
        {
            // Whether the preferences contain every index they should for hotbar data
            bool isValid = true;
            // Get default pieces in case preferences are empty
            PieceList defaultPieces = PieceInfo.GetDefaultPieces();
            Pieces = [];

            // Preferences cannot have more pieces than the number of default pieces in the app
            for (int i = 0; i < defaultPieces.Count; i++)
            {
                // Grab each hotbar piece from preferences
                string key = $"Hotbar_Piece{i}";

                // If preferences are missing a piece, preference data is invalid
                if (!Preferences.ContainsKey(key))
                    isValid = false;
                else
                {
                    string value = Preferences.Get(key, "");

                    // Add the piece from the preferences to the hotbar
                    if (!string.IsNullOrWhiteSpace(value))
                        Pieces.Add(new((SegmentType)Enum.Parse(typeof(SegmentType), value)));
                }
            }

            // Revert to defaults
            if (!isValid)
                Pieces = defaultPieces;
        }

        /// <summary>
        /// Moves a piece in the hotbar located at an old index to a new index
        /// </summary>
        /// <param name="oldIndex">The location of the piece to move</param>
        /// <param name="newIndex">The location the piece should be moved to</param>
        public static void MovePiece(int oldIndex, int newIndex)
        {
            Pieces.Move(oldIndex, newIndex);
            SaveHotbar();
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

            SaveHotbar();
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

            SaveHotbar();
        }

        /// <summary>
        /// Removes a piece from the hotbar at a given index
        /// </summary>
        /// <param name="index">The location to remove a piece from</param>
        public static void RemovePiece(int index)
        {
            Pieces.RemoveAt(index);
            SaveHotbar();
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

            SaveHotbar();
        }

        /// <summary>
        /// Saves the user's hotbar to user preferences
        /// </summary>
        private static void SaveHotbar()
        {
            int pieceCount = PieceInfo.GetDefaultPieces().Count;

            // Iterate through every index preferences should have of hotbar pieces
            for (int i = 0; i < pieceCount; i++)
            {
                // Store each hotbar piece in preferences
                string key = $"Hotbar_Piece{i}";

                // Ensure the piece is present in the hotbar
                if (i < Pieces.Count)
                    Preferences.Set(key, Pieces[i].SegmentType.ToString());
                else // Default to an empty piece in preferences
                    Preferences.Set(key, "");
            }
        }
    }
}
