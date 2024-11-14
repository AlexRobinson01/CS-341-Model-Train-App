using ModelTrain.Model;
namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the login page
 * Author: Andrew Martin
 * Date: October 6, 2024
 */
public partial class Login : ContentPage
{
    private IBusinessLogic BusinessLogic { get; set; }
    public Login()
	{
		InitializeComponent();
        BusinessLogic = new BusinessLogic();
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Must input values to login. Please try again.", "OK");
        } else
        {
            if(await BusinessLogic.ValidateLoginInput(email, password))
            {
                //ADD LOGIN FUNCTIONALITY HERE
                await Navigation.PushAsync(new HomeScreen());
            } else
            {
                await DisplayAlert("Error", "Invalid email or password. Please try again.", "OK");
            }
            
        }
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        // Navigate to the Create Account page
        await Navigation.PushAsync(new CreateAccount());
    }
}