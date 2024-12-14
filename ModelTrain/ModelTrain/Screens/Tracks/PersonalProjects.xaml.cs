using ModelTrain.Model;

namespace ModelTrain.Screens
{
    /*
     * This class is the background functionality/methods for the personal projects page
     * Author: Andrew Martin
     * Date: October 6, 2024
     */
    public partial class PersonalProjects : ContentPage
    {
        /// <summary>
        /// Initializes the Personal Projects page.
        /// Fetches and displays the user's saved projects.
        /// </summary>
        public PersonalProjects()
        {
            InitializeComponent(); 
            FetchPersonalProjects(); // Load the user's projects

            // Example data for testing purposes (commented out for production use)
            // collectionView.ItemsSource = new List<PersonalProject>
            // {
            //     new PersonalProject { ProjectName = "Project 1", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0001", Track = new Model.Track.TrackBase()},
            //     new PersonalProject { ProjectName = "Project 2", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0002", Track = new Model.Track.TrackBase()},
            //     new PersonalProject { ProjectName = "Project 3", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0003", Track = new Model.Track.TrackBase()},
            //     new PersonalProject { ProjectName = "Project 4", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0004", Track = new Model.Track.TrackBase()}
            // };
        }

        private PersonalProject _selectedProject; // Stores the currently selected project
        private Frame _selectedFrame; // Stores the currently selected UI frame

        /// <summary>
        /// Fetches the user's personal projects from the database and displays 
        /// them in the CollectionView.
        /// </summary>
        private async void FetchPersonalProjects()
        {
            // Retrieve the project IDs associated with the user
            var projectIds = await BusinessLogic.Instance.GetUserProjects();

            if (projectIds.Any()) // Check if the user has any projects
            {
                // Retrieve project details based on the project IDs
                var projects = await BusinessLogic.Instance.GetProjectsByIds(projectIds);

                // Bind the projects to the CollectionView
                collectionView.ItemsSource = projects;
            }
            else
            {
                // Notify the user if no projects are available
                await DisplayAlert("No Projects", "You have no projects to display.", "OK");
            }
        }

        /// <summary>
        /// Handles tapping on a project to select it.
        /// Provides visual feedback and updates the selected project.
        /// </summary>
        private async void OnProjectTapped(object sender, EventArgs e)
        {
            // Reset the previous selection's background color
            if (_selectedFrame != null)
            {
                _selectedFrame.BackgroundColor = Colors.White;
            }

            // Identify the tapped frame and associated project
            var frame = sender as Frame;
            if (frame?.BindingContext is PersonalProject tappedProject)
            {
                _selectedProject = tappedProject; // Update the selected project
                _selectedFrame = frame; // Update the selected frame

                // Apply a bounce animation for visual feedback
                await _selectedFrame.ScaleTo(0.95, 50, Easing.CubicIn);  // Scale down slightly
                await _selectedFrame.ScaleTo(1.0, 50, Easing.CubicOut);  // Scale back

                // Set background color of the selected frame to provide feedback
                _selectedFrame.BackgroundColor = Colors.LightGray;
            }
        }

        /// <summary>
        /// Navigates to the Track Editor page for the selected project.
        /// </summary>
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (_selectedProject != null)
            {
                // Navigate to the editor for the selected project
                await Navigation.PushAsync(new TrackEditor(_selectedProject));
            }
            else
            {
                // Alert the user if no project is selected
                await DisplayAlert("No Project Selected", "Please select a project to edit.", "OK");
            }
        }

        /// <summary>
        /// Navigates to the Properties screen.
        /// </summary>
        private async void OnConfigureButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PropertiesScreen());
        }

        /// <summary>
        /// Navigates back to the Home screen.
        /// Clears the navigation stack for a clean transition.
        /// </summary>
        private async void OnHomeButtonClicked(object sender, EventArgs e)
        {
            // Clear the navigation stack by setting a new NavigationPage with 
            // HomeScreen as the root
            Application.Current.MainPage = new NavigationPage(new HomeScreen());
        }

        /// <summary>
        /// Handles deleting a project. Prompts for confirmation before proceeding.
        /// Updates the UI after successful deletion.
        /// </summary>
        private async void OnDeleteIconTapped(object sender, EventArgs e)
        {
            // Confirm deletion
            var confirmed = await DisplayAlert("Delete Project", "Are you sure you want to delete this project?", "Yes", "No");
            if (!confirmed) return; // Exit if the user cancels

            // Identify the tapped icon's associated project
            if (sender is Label label && label.BindingContext is PersonalProject projectToDelete)
            {
                // Attempt to delete the project via the business logic layer
                if (await BusinessLogic.Instance.DeleteProjectById(projectToDelete.ProjectID))
                {
                    // Update the UI to remove the deleted project
                    var items = (List<PersonalProject>)collectionView.ItemsSource;
                    items.Remove(projectToDelete);
                    collectionView.ItemsSource = null; // Refresh CollectionView binding
                    collectionView.ItemsSource = items;

                    await DisplayAlert("Deleted", 
                    $"Project {projectToDelete.ProjectName} has been deleted.", "OK");
                }
                else
                {
                    // Notify the user of an error
                    await DisplayAlert("Error", "Failed to delete project", "Close");
                }
            }
        }
    }
}