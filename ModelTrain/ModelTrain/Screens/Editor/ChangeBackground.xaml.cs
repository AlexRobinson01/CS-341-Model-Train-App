using ModelTrain.Services;

namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the background changing page.
 * Note that this page is supposed to be in landscape only
 * Author: Krystal Schneider
 * Date: October 16, 2024
 */
public partial class ChangeBackground : ContentPage
{

    private PersonalProject? tempProject;

    public ChangeBackground(PersonalProject sentProject)
    {
        InitializeComponent();
        tempProject = sentProject;
    }

    private async void OnCameraButtonClicked(object sender, EventArgs e)
    {
        //Logic
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // Save the selected image path
                tempProject.BackgroundImage = photo.FullPath;
            }
        }

        await Navigation.PopAsync();
    }

    private async void OnGalleryButtonClicked(object sender, EventArgs e)
    {
        try
        {
            // Pick an image file
            FileResult? result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select an Image for the Background",
                FileTypes = FilePickerFileType.Images // Restrict to image files
            });

            if (result != null)
            {
                // Save the selected image path
                tempProject.BackgroundImage = result.FullPath;
            }
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to select image: {ex.Message}", "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DeviceOrientation.SetLandscape();
    }
}