namespace ModelTrain.Screens;
/*
 * This class is the background functionality/methods the New Track page
 * Author: Taylor Showalter
 * Date: October 16, 2024
 */
public partial class NewTrack : ContentPage
{
    public NewTrack()
    {
        InitializeComponent();
    }
    private async void OnHomeButtonClicked(object sender, EventArgs e)
    {
        // Clear the navigation stack by setting a new NavigationPage with HomeScreen as the root
        Application.Current.MainPage = new NavigationPage(new HomeScreen());
    }
    private async void OnPrivateButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TrackEditor());
    }
    private async void OnSharedButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TrackEditor());
    }
}