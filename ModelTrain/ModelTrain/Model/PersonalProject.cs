﻿using ModelTrain.Model.Track;
using ModelTrain.Services;

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

        public string? BackgroundImage
        {
            get => UserPreferences.Get(ProjectID ?? "", "");
            set => UserPreferences.Set(ProjectID ?? "", value);
        }

        public TrackBase Track { get; set; }
    }
}
