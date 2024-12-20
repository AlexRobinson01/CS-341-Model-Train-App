using ModelTrain.Model;

namespace ModelTrain.Screens
{
    /*
     * This class is the background functionality/methods for the create account page
     * Author: Andrew Martin and Taylor Showalter and Alex Robinson
     * Date: October 6, 2024
     */
    public partial class CreateAccount : ContentPage
    {
        /// <summary>
        /// Gets or sets the business logic service for account-related operations.
        /// </summary>
        private IBusinessLogic BusinessLogic { get; set; }

        /// <summary>
        /// Initializes the Create Account page and its dependencies.
        /// </summary>
        public CreateAccount()
        {
            InitializeComponent();
            BusinessLogic = new BusinessLogic(); 
        }

        /// <summary>
        /// Handles the Create Account button click event.
        /// Validates user input and attempts to create a new account.
        /// </summary>
        private async void CreateAccountBtnClicked(object sender, EventArgs e)
        {
            bool accountCreated = false; // Tracks account creation status
            bool emptyInputs = false; // Tracks if inputs are empty
            //retrieve user input for account information for creation
            string firstName = FirstNameEntry.Text; 
            string lastName = LastNameEntry.Text; 
            string email = EmailEntry.Text; 
            string password = PasswordEntry.Text; 

            // Check if any inputs are empty
            if (string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password))
            {
                emptyInputs = true; // Mark inputs as empty
                await DisplayAlert("Error", "Failed to create account. " +
                    "All inputs must contain values. Please try again.", "OK");
            }
            else
            {
                // Attempt to create account if inputs are valid
                if (!emptyInputs &&
                await BusinessLogic.CreateAccount(firstName, lastName, email, password))
                {
                    await DisplayAlert("Success", "Account created successfully!", "OK");
                    // Navigate back to login page
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error",
                        "Failed to create account. Email already in use.", "OK");
                }
            }
        }
    }
}
