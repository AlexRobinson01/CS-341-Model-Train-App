using ModelTrain.Model;

namespace ModelTrain.Screens
{
    /*
     * This class is the background functionality/methods for the reset email page
     * Author: Andrew Martin and Taylor Showalter and Alex Robinson
     * Date: December 10, 2024
     */
    public partial class ResetEmail : ContentPage
    {
        /// <summary>
        /// Initializes the Reset Email page.
        /// </summary>
        public ResetEmail()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Change Email button click event.
        /// Validates user input and updates the email if valid.
        /// </summary>
        private async void OnChangeEmailClicked(object sender, EventArgs e)
        {
            bool emailsMatch = false; // Tracks if emails match
            bool uniqueEmail = false; // Tracks if email is unique

            string newEmail = NewEmailEntry.Text; // Retrieve new email input
            string confirmNewEmail = ConfirmNewEmailEntry.Text; // Retrieve confirmation email input

            // Check if inputs are valid
            if (string.IsNullOrEmpty(newEmail) ||       // Don't accept if either entry is null or empty
                string.IsNullOrEmpty(confirmNewEmail))
            {
                // Alert the user to fill all fields
                await DisplayAlert("Error", "Failed to change email. All inputs must contain values. Please try again.", "OK");
            }
            else
            {
                ConfirmNewEmailEntry.Unfocus();     // Close keyboard
                if (newEmail == confirmNewEmail)    // If emails are identical check if email is not associated with an account already
                {
                    emailsMatch = true;

                    // Check if the email is unique
                    if (await BusinessLogic.Instance.IsUniqueEmail(newEmail))
                    {
                        uniqueEmail = true;

                        // Attempt to change the email
                        if (await BusinessLogic.Instance.ChangeEmail(newEmail))
                        {
                            // Notify the user of success and navigate back
                            await DisplayAlert("Success", "Email changed successfully!", "OK");
                            await Navigation.PopAsync(); // Return to the Account page
                        }
                        else
                        {
                            // Notify the user of an error
                            await DisplayAlert("Error", "Failed to change email. Something went wrong.", "OK");
                        }
                    }
                    else
                    {
                        // Notify the user if the email is already in use
                        await DisplayAlert("Error", "Failed to change email. An account with this email already exists.", "OK");
                    }
                }
                else
                {
                    // Notify the user if emails do not match
                    await DisplayAlert("Error", "Failed to change email. New emails do not match.", "OK");
                }
            }
        }
    }
}
