using ModelTrain.Model;
using ModelTrain.Services;

namespace ModelTrain.Screens
{
    /*
     * This class is the background functionality/methods the Account page
     * Author: Taylor Showalter and Andrew Martin
     * Date: October 16, 2024
     */
    public partial class Account : ContentPage
    {
        /// <summary>
        /// Initializes the Account page, loads user data, and sets the dark mode switch state.
        /// </summary>
        public Account()
        {
            InitializeComponent(); // Initialize UI components
            LoadUserData(); // Load user data into UI elements
            // Set the dark mode switch state
            DarkModeSwitch.IsToggled = Application.Current.UserAppTheme == AppTheme.Dark; 
        }

        /// <summary>
        /// Executes when the page appears; reloads user data to reflect any changes.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData(); // Refresh user data
        }

        /// <summary>
        /// Navigates to the Reset Password page.
        /// </summary>
        private async void OnChangePasswordButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ResetPassword()); // Open the password reset screen
        }

        /// <summary>
        /// Navigates to the Reset Email page.
        /// </summary>
        private async void OnChangeEmailButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ResetEmail()); // Open the email reset screen
        }

        /// <summary>
        /// Logs out the user by clearing secure storage and navigating to the Login page.
        /// </summary>
        private async void OnLogOutButtonClicked(object sender, EventArgs e)
        {
            SecureStorage.Remove("UserEmail"); // Remove stored email
            SecureStorage.Remove("UserPassword"); // Remove stored password

            Application.Current.UserAppTheme = AppTheme.Light; // Reset theme to default
            Application.Current.MainPage = new NavigationPage(new Login()); // Redirect to login
        }

        /// <summary>
        /// Toggles the application theme between dark and light modes.
        /// </summary>
        private void OnDarkModeSwitchToggled(object sender, ToggledEventArgs e)
        {
            if (e.Value) // If switch is toggled to dark mode
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
                UserPreferences.Set("UserTheme", "Dark"); // Save dark mode preference
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
                UserPreferences.Set("UserTheme", "Light"); // Save light mode preference
            }
        }

        /// <summary>
        /// Loads user data from the backend and updates the UI.
        /// </summary>
        private async void LoadUserData()
        {
            User user = BusinessLogic.Instance.GetUserFromEmail(); // Retrieve user details

            if (user != null)
            {
                nameLabel.Text = user.FullName; // Display full name
                emailLabel.Text = user.Email; // Display email address
                initialsLabel.Text = user.Initials; // Set the initials in the profile circle
            }
        }
    }
}
