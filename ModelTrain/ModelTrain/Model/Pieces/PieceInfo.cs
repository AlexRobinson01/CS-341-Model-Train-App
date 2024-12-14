using ModelTrain.Model.Track;
using ModelTrain.Services;
using System.Numerics;

namespace ModelTrain.Model.Pieces
{
    /**
     * Description: Static methods to get default image/name/etc. info for pieces
     * Author: Alex Robinson
     * Last updated: 12/8/2024
     */
    public static class PieceInfo
    {
        /// <summary>
        /// Takes in a SegmentType and populates two given strings with their relevant piece info
        /// associated with this type
        /// </summary>
        /// <param name="type">The SegmentType to fetch piece info for</param>
        /// <param name="name">A string reference to be set to the piece's name</param>
        /// <param name="image">A string reference to be set to the piece's image path</param>
        public static void GetInfo(SegmentType type, out string name, out string image)
        {
            name = type switch
            {
                SegmentType.Straight => "Straight",
                //SegmentType.Curve15 => "Curve15",
                SegmentType.Curve30 => "Curve30",
                SegmentType.Curve45 => "Curve45",
                SegmentType.Curve60 => "Curve60",
                //SegmentType.Curve75 => "Curve75",
                SegmentType.Curve90 => "Curve90",
                _ => ""
            };

            string? preference = UserPreferences.Get(name, null);
            image = preference ?? type switch
            {
                SegmentType.Straight => "piece_straight.png",
                //SegmentType.Curve15 => "piece_15curve.png",
                SegmentType.Curve30 => "piece_30curve.png",
                SegmentType.Curve45 => "piece_45curve.png",
                SegmentType.Curve60 => "piece_60curve.png",
                //SegmentType.Curve75 => "piece_75curve.png",
                SegmentType.Curve90 => "piece_90curve.png",
                _ => ""
            };

            // Default to the embedded resource
            if (preference == null || !File.Exists(preference))
                image = $"ModelTrain.DefaultImages.{image}";
        }

        /// <summary>
        /// Takes in a SegmentType and populates three values with their relevant piece info
        /// associated with this type
        /// </summary>
        /// <param name="pieceName">The name of the piece type to fetch piece RSO data for</param>
        /// <param name="rotation">A float reference to be set to this type's rotation</param>
        /// <param name="scale">A float reference to be set to this type's scale</param>
        /// <param name="offset">A Vector2 reference to be set to this type's offset</param>
        public static void GetRSO(string pieceName,
            out float rotation, out float scale, out Vector2 offset)
        {
            // Default rotation, scale, and offset values
            rotation = 0;
            scale = 1;
            offset = Vector2.Zero;

            try
            {
                // Get RSO from user preferences (changed in PieceEditor)
                rotation = UserPreferences.Get($"{pieceName}_rotation", 0f);
                scale = UserPreferences.Get($"{pieceName}_scale", 1f);

                // Offset is a Vector2 which can't be saved in preferences,
                // so two floats need to be used instead
                float offsetX = UserPreferences.Get($"{pieceName}_offsetX", 0f);
                float offsetY = UserPreferences.Get($"{pieceName}_offsetY", 0f);

                offset = new(offsetX, offsetY);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Fills a new PieceList with a piece for each of the default SegmentTypes
        /// </summary>
        /// <returns>A PieceList containing one piece per SegmentType value</returns>
        public static PieceList GetDefaultPieces()
        {
            PieceList pieces = new();

            // Making a default piece for each option in the SegmentType enum
            foreach (SegmentType type in Enum.GetValues(typeof(SegmentType)))
                pieces.Add(new(type));

            return pieces;
        }
    }
}
