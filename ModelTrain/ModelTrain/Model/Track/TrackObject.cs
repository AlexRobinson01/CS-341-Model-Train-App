using ModelTrain.Model.Pieces;

namespace ModelTrain.Model.Track
{
    /**
     * Description: Binds a Piece and Segment together with an API to manage rendering
     * and editing each of these
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public class TrackObject
    {
        // Provides a way to get image data and segment data out of the same object
        public Piece BoundPiece { get; private set; }
        public Segment BoundSegment { get; private set; }

        /// <summary>
        /// TrackObject constructor - Creates a new TrackObject bound to a given Segment,
        /// creating a new Piece to bind to from the Segment's SegmentType
        /// </summary>
        /// <param name="linkedSegment">The Segment to bind to this TrackObject</param>
        public TrackObject(Segment linkedSegment)
        {
            BoundPiece = new(linkedSegment.SegmentType);
            BoundSegment = linkedSegment;
        }

        /// <summary>
        /// TrackObject constructor - Creates a new TrackObject bound to a given Piece,
        /// creating a new Segment to bind to from the Piece's SegmentType,
        /// adding it to the given TrackBase in the process
        /// </summary>
        /// <param name="track">The TrackBase to add a new Segment to</param>
        /// <param name="linkedPiece">The Piece to bind to this TrackObject</param>
        public TrackObject(TrackBase track, Piece linkedPiece)
        {
            BoundPiece = linkedPiece;
            BoundSegment = new(linkedPiece.SegmentType);

            track.AddSegment(BoundSegment);
        }

        /// <summary>
        /// Moves the bound Segment to the given x and y values
        /// </summary>
        /// <param name="x">The x coordinate to move the bound Segment to</param>
        /// <param name="y">The y coordinate to move the bound Segment to</param>
        public void MoveTo(float x, float y)
        {
            BoundSegment.X = x;
            BoundSegment.Y = y;
        }

        /// <summary>
        /// Moves the bound Segment to the given x and y values
        /// </summary>
        /// <param name="x">The x coordinate to move the bound Segment to</param>
        /// <param name="y">The y coordinate to move the bound Segment to</param>
        public void MoveTo(double x, double y) => MoveTo((float)x, (float)y);

        /// <summary>
        /// Rotates the bound Segment to the given value in degrees
        /// </summary>
        /// <param name="rotation">The value to set the bound Segment's rotation to</param>
        public void Rotate(int rotation)
        {
            BoundSegment.Rotation = rotation;
        }

        /// <summary>
        /// Removes the bound Segment from a given TrackBase
        /// </summary>
        /// <param name="track">The TrackBase to remove the bound Segment from</param>
        public void RemoveFrom(TrackBase track)
        {
            track.RemoveSegment(BoundSegment);
        }

        /// <summary>
        /// Sets the bound Segment's SnappedStartSegment to the given Segment,
        /// unless the bound Segment is already snapped to something else
        /// </summary>
        /// <param name="snap">The Segment to attempt to snap the bound Segment to</param>
        public void SnapToStart(Segment snap)
        {
            BoundSegment.SnappedStartSegment ??= snap;
        }

        /// <summary>
        /// Sets the bound Segment's SnappedEndSegment to the given Segment,
        /// unless the bound Segment is already snapped to something else
        /// </summary>
        /// <param name="snap">The Segment to attempt to snap the bound Segment to</param>
        public void SnapToEnd(Segment snap)
        {
            BoundSegment.SnappedEndSegment ??= snap;
        }

        /// <summary>
        /// Disconnects the bound Segment's SnappedSTartSegment,
        /// while also unsnapping the snapped Segment from the bound one
        /// </summary>
        public void UnsnapStart()
        {
            Segment? snappedStart = BoundSegment.SnappedStartSegment;
            if (snappedStart?.SnappedStartSegment == BoundSegment)
                snappedStart.SnappedStartSegment = null;
            else if (snappedStart?.SnappedEndSegment == BoundSegment)
                snappedStart.SnappedEndSegment = null;

            BoundSegment.SnappedStartSegment = null;
        }

        /// <summary>
        /// Disconnects the bound Segment's SnappedEndSegment,
        /// while also unsnapping the snapped Segment from the bound one
        /// </summary>
        public void UnsnapEnd()
        {
            Segment? snappedEnd = BoundSegment.SnappedEndSegment;
            if (snappedEnd?.SnappedStartSegment == BoundSegment)
                snappedEnd.SnappedStartSegment = null;
            else if (snappedEnd?.SnappedEndSegment == BoundSegment)
                snappedEnd.SnappedEndSegment = null;

            BoundSegment.SnappedEndSegment = null;
        }
    }
}
