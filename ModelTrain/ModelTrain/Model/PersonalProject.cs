using ModelTrain.Model.Track;

namespace ModelTrain
{
    public class PersonalProject
    {
        public string ProjectName { get; set; }
        public string DateCreated { get; set; }
        public string ProjectID { get; set; }

        public TrackBase Track { get; private set; }
    }
}
