namespace ModelTrain.Model.Track
{
    public class ActionHandler
    {
        // Holds a set of Base64 strings that represent each previous state of the track
        // as it is edited in TrackEditor
        private readonly List<string> snapshots;
        private int curIndex;
        // The track that these actions will be applied to
        private readonly TrackBase linkedTrack;

        public ActionHandler(TrackBase track)
        {
            snapshots = new();
            linkedTrack = track;
        }

        public void Run()
        {
            // If any actions have been undone and not redone, clear them from the list
            // to remove them from the edit history
            while (curIndex + 1 < snapshots.Count)
                snapshots.RemoveAt(curIndex + 1);

            // Save current track state and run the given action
            TakeSnapshot();
        }

        public void Undo()
        {
            if (curIndex <= 0)
                return;

            // Return to the previous state of the track in memory from the snapshots list
            LoadSnapshot();
            curIndex--;
        }

        public void Redo()
        {
            if (curIndex >= snapshots.Count)
                return;

            // Return to the next state of the track in memory from the snapshots list
            curIndex++;
            LoadSnapshot();
        }

        private void TakeSnapshot()
        {
            // Store the current state of the track into the snapshots list
            string snapshot = linkedTrack.GetSegmentsAsString();
            snapshots.Add(snapshot);
        }

        private void LoadSnapshot()
        {
            // Load the stored state of the track from the snapshots list
            string snapshot = snapshots[curIndex];
            linkedTrack.LoadSegmentsFromString(snapshot);
        }
    }
}
