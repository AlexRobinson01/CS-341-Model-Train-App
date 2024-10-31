namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the create account page
 * Author: Andrew Martin
 * Date: October 6, 2024
 */
public partial class CreateAccount : ContentPage
{
	public CreateAccount()
	{
		InitializeComponent();
	}

    private async void CreateAccountBtnClicked(object sender, EventArgs e)
    {
        // Create account logic


        // Navigate to the Create Account page
        bool accountCreated = true;

        if (accountCreated)
        {
            await DisplayAlert("Success", "Account created successfully!", "OK");

            // Pop the CreateAccountPage off the stack to return to LoginPage
            await Navigation.PopAsync();
        }
        else
        {
            // Handle account creation failure
            await DisplayAlert("Error", "Failed to create account. Please try again.", "OK");
        }
    }
}