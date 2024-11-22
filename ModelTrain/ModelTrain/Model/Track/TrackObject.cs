using ModelTrain.Model.Pieces;

namespace ModelTrain.Model.Track
{
    public class TrackObject
    {
        // Contains a bound PieceBase and Segment, with API to manage rendering and moving
        // an object on the track while implementing required actions for undo/redo capabilities

        public PieceBase BoundPiece { get; private set; }
        public Segment BoundSegment { get; private set; }

        public TrackObject(Segment linkedSegment)
        {
            BoundPiece = new(linkedSegment.SegmentType);
            BoundSegment = linkedSegment;
        }

        public TrackObject(TrackBase track, PieceBase linkedPiece)
        {
            BoundPiece = linkedPiece;
            BoundSegment = new(linkedPiece.SegmentType);

            track.AddSegment(BoundSegment);
        }

        public void MoveTo(float x, float y)
        {
            BoundSegment.X = x;
            BoundSegment.Y = y;
        }

        public void MoveTo(double x, double y) => MoveTo((float)x, (float)y);

        public void Rotate(int rotation)
        {
            BoundSegment.Rotation = rotation;
        }

        public void RemoveFrom(TrackBase track)
        {
            track.RemoveSegment(BoundSegment);
        }

        public void SnapToStart(Segment snap)
        {
            BoundSegment.SnappedStartSegment ??= snap;
        }

        public void SnapToEnd(Segment snap)
        {
            BoundSegment.SnappedEndSegment ??= snap;
        }

        public void UnsnapStart()
        {
            Segment? snappedStart = BoundSegment.SnappedStartSegment;
            if (snappedStart?.SnappedStartSegment == BoundSegment)
                snappedStart.SnappedStartSegment = null;
            else if (snappedStart?.SnappedEndSegment == BoundSegment)
                snappedStart.SnappedEndSegment = null;

            BoundSegment.SnappedStartSegment = null;
        }

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
