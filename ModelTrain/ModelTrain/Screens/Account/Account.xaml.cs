using System.Xml;
using ModelTrain.Model;
using ModelTrain.Services;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Reload user data whenever the Account page appears (after email is changed, update it)
        LoadUserData();
    }

    private async void OnChangePasswordButtonClicked(object sender, EventArgs e)
    {
        // Navigation to Change Password page
        await Navigation.PushAsync(new ResetPassword());
    }

    private async void OnChangeEmailButtonClicked(object sender, EventArgs e)
    {
        // Navigation to Change Email page
        await Navigation.PushAsync(new ResetEmail());
    }

    private async void OnLogOutButtonClicked(object sender, EventArgs e)
    {
        // Clear SecureStorage
        SecureStorage.Remove("UserEmail");
        SecureStorage.Remove("UserPassword");

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
            UserPreferences.Set("UserTheme", "Dark");
        }
        else
        {
            Application.Current.UserAppTheme = AppTheme.Light;
            UserPreferences.Set("UserTheme", "Light");
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