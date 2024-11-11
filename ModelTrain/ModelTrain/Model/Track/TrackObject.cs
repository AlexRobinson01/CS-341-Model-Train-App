using ModelTrain.Model.Pieces;

namespace ModelTrain.Model.Track
{
    public class TrackObject
    {
        // Contains a bound PieceBase and Segment, with API to manage rendering and moving
        // an object on the track while implementing required actions for undo/redo capabilities

        public PieceBase BoundPiece { get; private set; }
        public Segment BoundSegment { get; private set; }

        public ActionHandler? ActionHandler { get; set; }

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

        private void Run(Action action)
        {
            if (ActionHandler != null)
                ActionHandler.Run(action);
            else
                action.Invoke();
        }

        public void MoveTo(float x, float y)
        {
            Run(() =>
            {
                BoundSegment.X = x;
                BoundSegment.Y = y;
            });
        }

        public void MoveTo(double x, double y) => MoveTo((float)x, (float)y);

        public void Rotate(int rotation)
        {
            Run(() =>
            {
                BoundSegment.Rotation = rotation;
            });
        }

        public void RemoveFrom(TrackBase track)
        {
            Run(() =>
            {
                track.RemoveSegment(BoundSegment);
            });
        }

        public void SnapToStart(Segment snap)
        {
            Run(() =>
            {
                BoundSegment.SnappedStartSegment ??= snap;
            });
        }

        public void SnapToEnd(Segment snap)
        {
            Run(() =>
            {
                BoundSegment.SnappedEndSegment ??= snap;
            });
        }

        public void UnsnapStart()
        {
            Run(() =>
            {
                BoundSegment.SnappedStartSegment = null;
            });
        }

        public void UnsnapEnd()
        {
            Run(() =>
            {
                BoundSegment.SnappedEndSegment = null;
            });
        }
    }
}
