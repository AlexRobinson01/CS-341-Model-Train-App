using ModelTrain.Model;
/*
     * This class is the background functionality/methods for the Collaborative Storage page
     * Author: Taylor Showalter and Alex Robinson and Andrew Martin
     * Date: November 13th, 2024
     */
namespace ModelTrain.Screens
{
    /// <summary>
    /// Represents a popup for creating a new project.
    /// Allows users to input a project name and notifies the parent page when a new project is created.
    /// </summary>
    public partial class CreateNewProjectPopup : ContentPage
    {
        /// <summary>
        /// Initializes the Create New Project Popup page.
        /// </summary>
        public CreateNewProjectPopup()
        {
            InitializeComponent(); 
        }

        /// <summary>
        /// Event triggered when a new project is successfully created.
        /// Passes the project name, project ID, and creation date to the subscriber.
        /// </summary>
        public event Action<string, string, string> ProjectCreated;

        /// <summary>
        /// Handles the "Submit" button click event.
        /// Validates the project name, generates a unique project ID, and notifies the parent page.
        /// </summary>
        private async void OnSubmitProjectClicked(object sender, EventArgs e)
        {
            string projectName = ProjectNameEntry.Text; // Retrieve the project name from the entry field

            if (!string.IsNullOrWhiteSpace(projectName)) // Ensure the project name is not empty
            {
                string projectId = await BusinessLogic.Instance.GetUniqueGuid(); // Generate a unique project ID
                string date = DateTime.Now.ToString("MM/dd/yyyy"); 

                ProjectNameEntry.Unfocus(); // Unfocus the entry field to close the keyboard

                // Trigger the ProjectCreated event to notify the parent page
                ProjectCreated?.Invoke(projectName, projectId, date);

                // Close the popup modal
                await Navigation.PopModalAsync();
            }
            else
            {
                // Alert the user if the project name is invalid
                await DisplayAlert("Error", "Project name cannot be empty.", "OK");
            }
        }

        /// <summary>
        /// Handles the "Cancel" button click event.
        /// Closes the popup modal without creating a project.
        /// </summary>
        private async void OnCancelProjectClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(); // Close the popup modal
        }
    }
}
