namespace ModelTrain.Screens;

using ModelTrain.Model;
using ModelTrain.Services;

/**
 * This page should be viewed in Landscape for the best experience.
 * 
 * Description: Allows the user to change which pieces they see in the list to the right
 * of the track editor page, in case they don't like their current list's order.
 * Author: Alex Robinson
 * Date: 10/16/2024
 */
public partial class PieceCatalog : ContentPage
{
	public PieceCatalog()
	{
		InitializeComponent();

		Edit.Text = IconFont.Settings + " EDIT PIECE";
		Back.Text = IconFont.Arrow_back + " BACK";
	}

	private async void OnEditButtonClicked(object sender, EventArgs e)
	{
		// Opens Piece Editor
		await Navigation.PushAsync(new PieceEditor());
	}

	private async void OnBackButtonClicked(object sender, EventArgs e)
	{
		// Should have gotten here from TrackEditor, pop to avoid memory leak
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