using ModelTrain.Model.Track;
using ModelTrain.Services;

namespace ModelTrain
{
    /// <summary>
    /// Represents a user's personal project in the ModelTrain application.
    /// Stores project details such as metadata, collaborators, and associated track information.
    /// </summary>
    public class PersonalProject
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string? ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the date the project was created.
        /// </summary>
        public string? DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the project.
        /// </summary>
        public string? ProjectID { get; set; }

        /// <summary>
        /// Gets or sets the date the project was last modified.
        /// </summary>
        public string? DateModified { get; set; }

        /// <summary>
        /// Gets or sets the name of the last user who edited the project.
        /// </summary>
        public string? LastEditor { get; set; }

        /// <summary>
        /// Gets or sets the size of the project (e.g., layout dimensions).
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// Gets or sets the list of collaborators who have access to the project.
        /// </summary>
        public string[]? Collaborators { get; set; }

        /// <summary>
        /// Gets or sets the background image for the project.
        /// The background image is stored in user preferences and associated with the project ID.
        /// </summary>
        public string? BackgroundImage
        {   // Retrieve background image from preferences
            get => UserPreferences.Get(ProjectID ?? "", "");
            // Save background image to preferences
            set => UserPreferences.Set(ProjectID ?? "", value); 
        }

        /// <summary>
        /// Gets or sets the track layout associated with the project.
        /// </summary>
        public TrackBase Track { get; set; }
    }
}
