namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the login page
 * Author: Andrew Martin
 * Date: October 6, 2024
 */
public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        // Login
        await Navigation.PushAsync(new HomeScreen()); ;
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        // Navigate to the Create Account page
        await Navigation.PushAsync(new CreateAccount());
    }
}