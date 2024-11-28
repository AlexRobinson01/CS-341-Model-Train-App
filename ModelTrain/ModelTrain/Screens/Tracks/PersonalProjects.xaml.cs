using ModelTrain.Model;
namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the personal projects page
 * Author: Andrew Martin
 * Date: October 6, 2024
 */
public partial class PersonalProjects : ContentPage
{
    public PersonalProjects()
    {
        InitializeComponent();
        FetchPersonalProjects();

        //collectionView.ItemsSource = new List<PersonalProject>
        //{
        //new PersonalProject { ProjectName = "Project 1", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0001", Track = new Model.Track.TrackBase()},
        //new PersonalProject { ProjectName = "Project 2", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0002", Track = new Model.Track.TrackBase()},
        //new PersonalProject { ProjectName = "Project 3", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0003", Track = new Model.Track.TrackBase()},
        //new PersonalProject { ProjectName = "Project 4", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0004", Track = new Model.Track.TrackBase()}
        //};
    }

    private PersonalProject _selectedProject;
    private Frame _selectedFrame;

    private async void FetchPersonalProjects()
    {
        // Get the list of project IDs from the user's record in the users table
        var projectIds = await BusinessLogic.Instance.GetUserProjects();

        if (projectIds.Any())
        {
            // Fetch project details from the projects table based on the project IDs
            var projects = await BusinessLogic.Instance.GetProjectsByIds(projectIds);

            // Set the projects as the ItemsSource of the CollectionView
            collectionView.ItemsSource = projects;
        }
        else
        {
            // If the user has no projects, show a message or handle as necessary
            await DisplayAlert("No Projects", "You have no projects to display.", "OK");
        }
    }

    private async void OnProjectTapped(object sender, EventArgs e)
    {
        // If old selection, reset color to white
        if (_selectedFrame != null)
        {
            _selectedFrame.BackgroundColor = Colors.White;
        }

        // Get tapped Frame and find its bound Project
        var frame = sender as Frame;
        if (frame?.BindingContext is PersonalProject tappedProject)
        {
            _selectedProject = tappedProject;
            _selectedFrame = frame;

            // Apply bounce effect
            await _selectedFrame.ScaleTo(0.95, 50, Easing.CubicIn);  // Scale down slightly
            await _selectedFrame.ScaleTo(1.0, 50, Easing.CubicOut);  // Scale back

            // Set background color of the selected frame to provide feedback
            _selectedFrame.BackgroundColor = Colors.LightGray;
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        // Navigate to the Track Editor page
        if (_selectedProject != null)
        {
            await Navigation.PushAsync(new TrackEditor(_selectedProject));
        }
        else
        {
            await DisplayAlert("No Project Selected", "Please select a project to edit.", "OK");
        }
    }

    private async void OnConfigureButtonClicked(object sender, EventArgs e)
    {
        // Navigate to the Properties page
        await Navigation.PushAsync(new PropertiesScreen());
    }
    private async void OnHomeButtonClicked(object sender, EventArgs e)
    {
        // Clear the navigation stack by setting a new NavigationPage with HomeScreen as the root
        Application.Current.MainPage = new NavigationPage(new HomeScreen());
    }

    private async void OnDeleteIconTapped(object sender, EventArgs e)
    {
        // Confirm deletion
        var confirmed = await DisplayAlert("Delete Project", "Are you sure you want to delete this project?", "Yes", "No");
        if (!confirmed) return;

        // Get the tapped icon's bound project
        if (sender is Label label && label.BindingContext is PersonalProject projectToDelete)
        {
            // Call business logic to delete the project
            if (await BusinessLogic.Instance.DeleteProjectById(projectToDelete.ProjectID))
            {
                // Remove the project from the UI
                var items = (List<PersonalProject>)collectionView.ItemsSource;
                items.Remove(projectToDelete);
                collectionView.ItemsSource = null; // Refresh UI
                collectionView.ItemsSource = items;

                await DisplayAlert("Deleted", $"Project {projectToDelete.ProjectName} has been deleted.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Failed to delete project", "Close");
            }
        }
    }
}