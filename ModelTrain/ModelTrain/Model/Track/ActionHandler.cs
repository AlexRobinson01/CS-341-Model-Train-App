namespace ModelTrain.Model.Track
{
    public class ActionHandler
    {
        private readonly List<string> snapshots;
        private int curIndex;

        private readonly TrackBase linkedTrack;

        public ActionHandler(TrackBase track)
        {
            snapshots = new();
            linkedTrack = track;
        }

        public void Run(Action action)
        {
            while (curIndex + 1 < snapshots.Count)
                snapshots.RemoveAt(curIndex + 1);

            TakeSnapshot();
            action.Invoke();
        }

        public void Undo()
        {
            if (curIndex <= 0)
                return;

            LoadSnapshot();
            curIndex--;
        }

        public void Redo()
        {
            if (curIndex >= snapshots.Count)
                return;

            curIndex++;
            LoadSnapshot();
        }

        private void TakeSnapshot()
        {
            string snapshot = linkedTrack.GetSegmentsAsString();
            snapshots.Add(snapshot);
        }

        private void LoadSnapshot()
        {
            string snapshot = snapshots[curIndex];
            linkedTrack.LoadSegmentsFromString(snapshot);
        }
    }
}
