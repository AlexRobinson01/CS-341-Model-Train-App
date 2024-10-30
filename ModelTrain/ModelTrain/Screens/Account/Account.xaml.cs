using System.Xml;
using ModelTrain.Model;
namespace ModelTrain.Screens;
/*
 * This class is the background functionality/methods the Account page
 * Author: Taylor Showalter and Andrew Martin
 * Date: October 16, 2024
 */
public partial class Account : ContentPage
{
    private IBusinessLogic BusinessLogic { get; set; }
    public Account()
    {
        BusinessLogic = new BusinessLogic();
        InitializeComponent();
        LoadUserData();
    }
    private async void OnChangePasswordButtonClicked(object sender, EventArgs e)
    {
        // Navigation to Change Password page
        await Navigation.PushAsync(new ResetPassword());
    }

    private async void OnLogOutButtonClicked(object sender, EventArgs e)
    {
        // Navigation to Login page
        await Navigation.PushAsync(new Login());
    }
    private async void LoadUserData()
    {
        // Replace with the actual logic to get the logged-in user's email
        string userEmail = "johndoe@email.com";

        User user = BusinessLogic.GetUserFromEmail(userEmail);

        if (user != null)
        {
            nameLabel.Text = user.FullName;
            emailLabel.Text = user.Email;
            initialsLabel.Text = user.Initials; // Set the initials in the profile circle
        }
    }
}