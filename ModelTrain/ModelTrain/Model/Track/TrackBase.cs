namespace ModelTrain.Model.Track
{
    public class TrackBase
    {
        public string ID { get; private set; }

        private readonly List<Segment> segments;

        public TrackBase(string id)
        {
            segments = new();
            ID = id;
        }

        public string GetSegmentsAsString()
        {
            // TODO: convert segments to string
            return "";
        }

        public void LoadSegmentsFromString(string segmentsStr)
        {
            // TODO: load string into segments
        }
    }
}
