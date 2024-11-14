using ModelTrain.Model.Track;

namespace ModelTrain
{
    public class PersonalProject
    {
        public string? ProjectName { get; set; }
        public string? DateCreated { get; set; }
        public string? ProjectID { get; set; }
        public string? DateModified { get; set; }
        public string? LastEditor { get; set; }
        public string? Size { get; set; }
        public string[]? Collaborators { get; set; }

        public TrackBase Track { get; set; }
    }
}
