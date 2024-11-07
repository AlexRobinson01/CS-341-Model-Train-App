using ModelTrain.Model;

namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the create account page
 * Author: Andrew Martin
 * Date: October 6, 2024
 */
public partial class CreateAccount : ContentPage
{
    private IBusinessLogic BusinessLogic { get; set; }
    public CreateAccount()
	{
		InitializeComponent();
        BusinessLogic = new BusinessLogic();
    }

    private async void CreateAccountBtnClicked(object sender, EventArgs e)
    {
        // Create account logic


        // Navigate to the Create Account page
        bool accountCreated = false;
        bool emptyInputs = false;
        string firstName = FirstNameEntry.Text;
        string lastName = LastNameEntry.Text;
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(firstName) ||
            string.IsNullOrEmpty(lastName) ||
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password))
            {
            emptyInputs = true;
            await DisplayAlert("Error", "Failed to create account. " +
                "All inputs must contain values. Please try again.", "OK");
        }

        if (!emptyInputs && BusinessLogic.CreateAccount(firstName, lastName, email, password))
        {
            await DisplayAlert("Success", "Account created successfully!", "OK");

            // Pop the CreateAccountPage off the stack to return to LoginPage
            await Navigation.PopAsync();
        } else
        {
            await DisplayAlert("Error", "Failed to create account. Please try again.", "OK");
        }
    }
}