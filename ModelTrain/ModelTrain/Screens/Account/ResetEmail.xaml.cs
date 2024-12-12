using ModelTrain.Model;
namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the reset email page
 * Author: Andrew Martin
 * Date: December 10, 2024
 */
public partial class ResetEmail : ContentPage
{
	public ResetEmail()
	{
		InitializeComponent();
	}

    private async void OnChangeEmailClicked(object sender, EventArgs e)
    {
        bool emailsMatch = false;
        bool uniqueEmail = false;
        string newEmail = NewEmailEntry.Text;
        string confirmNewEmail = ConfirmNewEmailEntry.Text;

        if (string.IsNullOrEmpty(newEmail) ||       // Dont accept if either entry is null or empty
            string.IsNullOrEmpty(confirmNewEmail))
        {
            await DisplayAlert("Error", "Failed to change email. " +
                "All inputs must contain values. Please try again.", "OK");
        }
        else
        {
            ConfirmNewEmailEntry.Unfocus();     // Close keyboard
            if (newEmail == confirmNewEmail)    // If emails are identical check if email is not associated with an account already
            {
                if (await BusinessLogic.Instance.IsUniqueEmail(newEmail))       // If new email is not in db, change it to this one
                {
                    if (await BusinessLogic.Instance.ChangeEmail(newEmail))
                    {
                        await DisplayAlert("Success", "Email Changed successfully!", "OK");
                        await Navigation.PopAsync();            // Go back to account screen
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to change email. Something went wrong.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Failed to change email. An account with this email already exists", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Failed to change email. New emails do not match", "OK");
            }

            
        }
    }
}