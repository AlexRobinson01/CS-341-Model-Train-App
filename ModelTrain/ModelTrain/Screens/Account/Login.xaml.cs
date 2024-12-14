using ModelTrain.Model;

namespace ModelTrain.Screens
{
    /*
     * This class is the background functionality/methods for the login page
     * Author: Andrew Martin and Taylor Showalter and Alex Robinson
     * Date: October 6, 2024
     */
    public partial class Login : ContentPage
    {
        /// <summary>
        /// Initializes the Login page.
        /// </summary>
        public Login()
        {
            InitializeComponent(); // Initialize UI components
        }

        /// <summary>
        /// Handles the Login button click event.
        /// Validates user input and logs in if the credentials are correct.
        /// </summary>
        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text; // Retrieve the email input
            string password = PasswordEntry.Text; // Retrieve the password input

            // Check if inputs are valid
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                // Alert the user to fill all fields
                await DisplayAlert("Error", "Must input values to login. Please try again.", "OK");
            }
            else
            {
                // Validate login credentials using the business logic layer
                if (await BusinessLogic.Instance.ValidateLoginInput(email, password))
                {
                    // Save credentials securely
                    SecureStorage.SetAsync("UserEmail", email);
                    SecureStorage.SetAsync("UserPassword", password);

                    // Navigate to the Home Screen
                    Application.Current.MainPage = new NavigationPage(new HomeScreen());
                }
                else
                {
                    // Show an error if credentials are invalid
                    await DisplayAlert("Error", "Invalid email or password. Please try again.", "OK");
                }
            }
        }

        /// <summary>
        /// Navigates to the Create Account page when the Create Account button is clicked.
        /// </summary>
        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            // Navigate to the Create Account page
            await Navigation.PushAsync(new CreateAccount());
        }
    }
}
