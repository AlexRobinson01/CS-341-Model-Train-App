using ModelTrain.Model;
namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the reset password page
 * Author: Andrew Martin
 * Date: October 6, 2024
 */
public partial class ResetPassword : ContentPage
{
	public ResetPassword()
	{
		InitializeComponent();
	}

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        bool passwordsMatch = false;
        bool oldPassCorrect = false;
        bool correctPass = false;
        string oldPass = OldPasswordEntry.Text;
        string newPass = NewPasswordEntry.Text;
        string confirmNewPass = ConfirmNewPasswordEntry.Text;

        if (string.IsNullOrEmpty(oldPass) ||
            string.IsNullOrEmpty(newPass) ||
            string.IsNullOrEmpty(confirmNewPass))
        {
            await DisplayAlert("Error", "Failed to change password. " +
                "All inputs must contain values. Please try again.", "OK");
        }
        else
        {
            if (newPass == confirmNewPass)
            {
                passwordsMatch = true;
            }

            if (await BusinessLogic.Instance.IsCorrectPassword(oldPass))
            {
                correctPass = true;
            }

            if (passwordsMatch && correctPass && await BusinessLogic.Instance.ChangePassword(newPass))
            {
                await DisplayAlert("Success", "Password changed successfully!", "OK");

            }
            else
            {
                await DisplayAlert("Error", "Failed to change password. Incorrect current password or new passwords do not match", "OK");
            }
        }
    }
}