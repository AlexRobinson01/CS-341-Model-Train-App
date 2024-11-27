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
    public Account()
    {
        InitializeComponent();
        LoadUserData();
        DarkModeSwitch.IsToggled = Application.Current.UserAppTheme == AppTheme.Dark;
    }
    private async void OnChangePasswordButtonClicked(object sender, EventArgs e)
    {
        // Navigation to Change Password page
        await Navigation.PushAsync(new ResetPassword());
    }

    private async void OnLogOutButtonClicked(object sender, EventArgs e)
    {
        // Clear SecureStorage
        SecureStorage.Remove("UserEmail");
        SecureStorage.Remove("UserPassword");

        // Reset app preferences
        Preferences.Clear(); // This clears all stored preferences

        // Reset the app theme to the default (e.g., Light)
        Application.Current.UserAppTheme = AppTheme.Light;

        // Navigate to the Login screen
        Application.Current.MainPage = new NavigationPage(new Login());
    }


    private void OnDarkModeSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            Application.Current.UserAppTheme = AppTheme.Dark;
            Preferences.Set("UserTheme", "Dark");
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            Preferences.Set("UserTheme", "Light");
        }
    }
    private async void LoadUserData()
    {

        User user = BusinessLogic.Instance.GetUserFromEmail();

        if (user != null)
        {
            nameLabel.Text = user.FullName;
            emailLabel.Text = user.Email;
            initialsLabel.Text = user.Initials; // Set the initials in the profile circle
        }
    }
}