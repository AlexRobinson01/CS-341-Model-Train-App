using ModelTrain.Model;
namespace ModelTrain.Screens;

/**
 * This page should be viewed in Landscape for the best experience.
 * 
 * Description: Allows a user to change the image associated with their selected piece,
 * in case they don't like the appearane of the current piece(s).
 * Author: Alex Robinson
 * Date: 10/16/2024
 */
public partial class PieceEditor : ContentPage
{
	public PieceEditor()
	{
		InitializeComponent();

		RotateCCW.Text = IconFont.Rotate_left + " CCW";
		RotateCW.Text = IconFont.Rotate_right + " CW";

		Confirm.Text = IconFont.Check + " CONFIRM";
		ChangeImage.Text = IconFont.Image + " CHANGE IMAGE";
		Cancel.Text = IconFont.Cancel + " CANCEL";
    }

	private async void OnConfirmButtonClicked(object sender, EventArgs e)
	{
		// Should have gotten here from PieceCatalog, pop to avoid memory leak
		await Navigation.PopAsync();
	}

	private async void OnCancelButtonClicked(object sender, EventArgs e)
	{
		// Should have gotten here from PieceCatalog, pop to avoid memory leak
		await Navigation.PopAsync();
	}
}