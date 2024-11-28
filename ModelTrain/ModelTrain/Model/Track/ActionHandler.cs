namespace ModelTrain.Model.Track
{
    /**
     * Description: Keeps logs of a track's edits to allow undoing/redoing edits
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public class ActionHandler
    {
        // Holds a set of Base64 strings that represent each previous state of the track
        // as it is edited in TrackEditor
        private readonly List<string> snapshots;
        private int curIndex = -1;
        // The track that these actions will be applied to
        private readonly TrackBase linkedTrack;

        /// <summary>
        /// ActionHandler constructor - Initializes new edit history for a given TrackBase
        /// </summary>
        /// <param name="track">The track to apply edit history to</param>
        public ActionHandler(TrackBase track)
        {
            snapshots = new();
            linkedTrack = track;

            AddWaypoint();
        }

        public void AddWaypoint()
        {
            // If any actions have been undone and not redone, clear them from the list
            // to remove them from the edit history
            while (curIndex + 1 < snapshots.Count)
                snapshots.RemoveAt(curIndex + 1);
            curIndex++;

            // Save current track state and run the given action
            TakeSnapshot();
        }

        /// <summary>
        /// Undoes an edit in the edit history
        /// </summary>
        public void Undo()
        {
            // Nothing to undo
            if (curIndex <= 0)
                return;

            // Return to the previous state of the track in memory from the snapshots list
            curIndex--;
            LoadSnapshot();
        }

        /// <summary>
        /// Redoes an undone edit in the edit history
        /// </summary>
        public void Redo()
        {
            if (curIndex + 1 >= snapshots.Count)
                return;

            // Return to the next state of the track in memory from the snapshots list
            curIndex++;
            LoadSnapshot();
        }

        /// <summary>
        /// Takes a snapshot of the assigned track and stores it in the snapshots list
        /// </summary>
        private void TakeSnapshot()
        {
            // Store the current state of the track into the snapshots list
            string snapshot = linkedTrack.GetSegmentsAsString();
            snapshots.Add(snapshot);
        }

        /// <summary>
        /// Retrieves a snapshot from the snapshots list and applies it to the assigned track
        /// </summary>
        private void LoadSnapshot()
        {
            // Load the stored state of the track from the snapshots list
            string snapshot = snapshots[curIndex];
            linkedTrack.LoadSegmentsFromString(snapshot);
        }
    }
}
