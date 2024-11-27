namespace ModelTrain.Model.Track
{
    /**
     * Description: Each of the different pieces that can be placed on a track
     * Author: Alex Robinson
     * Last updated: 11/27/2024
     */
    public enum SegmentType
    {
        // Removed 15 and 75 degree curves
        Straight,
        //Curve15,
        Curve30,
        Curve45,
        Curve60,
        //Curve75,
        Curve90
    }
}
