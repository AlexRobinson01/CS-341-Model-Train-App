using ModelTrain.Model;

namespace ModelTrain.Screens
{
    /*
     * This class is the background functionality/methods for the home page.
     * Author: Krystal Schneider, Taylor Showalter & Andrew Martin
     * Date: October 16, 2024
     */
    public partial class HomeScreen : BasePage
    {
        public HomeScreen()
        {
            InitializeComponent();
        }

        private async void OnCreateNewButtonClicked(object sender, EventArgs e)
        {
            // Navigation to New Track Screen
            //await Navigation.PushAsync(new NewTrack());

            // Instantiate the modal popup page for creating a new project
            var createProjectPage = new CreateNewProjectPopup();

            // Subscribe to the ProjectCreated event
            createProjectPage.ProjectCreated += async (name, id, date) =>
            {
                // Create the new project object
                PersonalProject newProject = new PersonalProject
                {
                    ProjectName = name,
                    ProjectID = id,
                    DateCreated = date,
                    Track = new Model.Track.TrackBase()
                };

                if (await BusinessLogic.Instance.AddProjectToDB(newProject))
                {
                    // Navigate to the TrackEditor page with the newly created project
                    await Navigation.PushAsync(new TrackEditor(newProject));
                }
                else
                {
                    // If error creating project, tell user
                    await DisplayAlert("Error", "Project could not be created.", "OK");
                }

            };

            // Show the popup modally
            await Navigation.PushModalAsync(createProjectPage);
        }

        private async void OnEditPreviousButtonClicked(object sender, EventArgs e)
        {
            // Navigation to Personal Storage
            await Navigation.PushAsync(new PersonalProjects());
        }

        private async void OnCollaborateButtonClicked(object sender, EventArgs e)
        {
            // Navigation to Collaborative Storage
            await Navigation.PushAsync(new CollaborativeStorage());
        }
    }
}