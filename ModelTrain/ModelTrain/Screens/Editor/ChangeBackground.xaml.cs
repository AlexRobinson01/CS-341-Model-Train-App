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
    public ChangeBackground()
	{
        InitializeComponent();
    }

    private async void OnCameraButtonClicked(object sender, EventArgs e)
    {
        //Logic
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);
            }
        }
        await Navigation.PopAsync();
    }

    private async void OnGalleryButtonClicked(object sender, EventArgs e)
    {
        //Logic
        FileResult photo = await MediaPicker.Default.PickPhotoAsync();

        if (photo != null)
        {
            // save the file into local storage
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

            using Stream sourceStream = await photo.OpenReadAsync();
            using FileStream localFileStream = File.OpenWrite(localFilePath);

            await sourceStream.CopyToAsync(localFileStream);
        }
        await Navigation.PopAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DeviceOrientation.SetLandscape();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        DeviceOrientation.SetPortrait();
    }
}