using ModelTrain.Model;

namespace ModelTrain.Screens
{
    /*
     * This class is the background functionality/methods for the reset password page
     * Author: Andrew Martin and Taylor Showalter and Alex Robinson
     * Date: October 6, 2024
     */
    public partial class ResetPassword : ContentPage
    {
        /// <summary>
        /// Initializes the Reset Password page.
        /// </summary>
        public ResetPassword()
        {
            InitializeComponent(); 
        }

        /// <summary>
        /// Handles the Change Password button click event.
        /// Validates user input and updates the password if valid.
        /// </summary>
        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            bool passwordsMatch = false; // Tracks if new passwords match
            bool correctPass = false; // Tracks if the old password is correct

            string oldPass = OldPasswordEntry.Text; // Retrieve current password
            string newPass = NewPasswordEntry.Text; // Retrieve new password
            // Retrieve confirmation password
            string confirmNewPass = ConfirmNewPasswordEntry.Text;

            // Check if inputs are valid
            if (string.IsNullOrEmpty(oldPass) ||
                string.IsNullOrEmpty(newPass) ||
                string.IsNullOrEmpty(confirmNewPass))
            {
                await DisplayAlert("Error", "Failed to change password. " +
                      "All inputs must contain values. Please try again.", "OK");
            }
            else
            {
                ConfirmNewPasswordEntry.Unfocus(); // Close the keyboard

                if (newPass == confirmNewPass) // Check if new passwords match
                {
                    passwordsMatch = true;
                }

                // Verify current password
                if (await BusinessLogic.Instance.IsCorrectPassword(oldPass))
                {
                    correctPass = true;
                }

                // Change the password if all conditions are met
                if (passwordsMatch && correctPass && 
                await BusinessLogic.Instance.ChangePassword(newPass))
                {
                    // Notify the user of success and navigate back
                    await DisplayAlert("Success",
                          "Password changed successfully!", "OK");
                    await Navigation.PopAsync(); // Go back to account screen

                }
                else
                {
                    // Notify the user if the operation fails
                    await DisplayAlert("Error",
                          "Failed to change password. Incorrect current password or new passwords do not match.",
                          "OK");
                }
            }
        }
    }
}
