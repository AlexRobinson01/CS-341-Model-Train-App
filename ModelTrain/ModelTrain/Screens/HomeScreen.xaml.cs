namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the home page.
 * Author: Krystal Schneider & Taylor Showalter
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
        PersonalProject newProject = new PersonalProject();
        await Navigation.PushAsync(new TrackEditor());
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