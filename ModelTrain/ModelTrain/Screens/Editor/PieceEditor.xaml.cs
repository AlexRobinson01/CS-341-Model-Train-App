using ModelTrain.Model;
using ModelTrain.Services;
using ModelTrain.Model.Pieces;

namespace ModelTrain.Screens;

/**
 * Description: Allows a user to change the image associated with their selected piece,
 * in case they don't like the appearance of the current piece(s).
 * Author: Alex Robinson & Taylor Showalter
 * Date: 12/8/2024
 */
public partial class PieceEditor : ContentPage
{
	// Store the path and rotation state
	private string _selectedImagePath;
	private float _currentRotation;
	private readonly Piece _piece;

	public PieceEditor(Piece piece)
	{
		InitializeComponent();
		_piece = piece;
		PieceImage.PieceOverride = piece;
		RotateCCW.Text = IconFont.Rotate_left + " Rotate Left";
		RotateCW.Text = IconFont.Rotate_right + " Rotate Right";

		Confirm.Text = IconFont.Check + " CONFIRM";
		ChangeImage.Text = IconFont.Image + " CHANGE IMAGE";
		Cancel.Text = IconFont.Cancel + " CANCEL";

		PieceImage.ClassId = piece.Name;
		PieceImage.Redraw();

		_currentRotation = piece.ImageRotation;
	}

	private async void OnChangeImageButtonClicked(object sender, EventArgs e)
	{
		try
		{
			// Pick an image file
			FileResult? result = await FilePicker.PickAsync(new PickOptions
			{
				PickerTitle = "Select an Image for the Track Piece",
				FileTypes = FilePickerFileType.Images // Restrict to image files
			});

			if (result != null)
			{
				// Save the selected image path
				_selectedImagePath = result.FullPath;

				// Update the image in the preview
				PieceImage.PieceOverride!.Image = _selectedImagePath;
				PieceImage.Redraw();
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to select image: {ex.Message}", "OK");
		}
	}

	private void OnRotateCCWButtonClicked(object sender, EventArgs e)
	{
		// Rotate counterclockwise
		_currentRotation = (_currentRotation - 90) % 360; // Keep rotation in range [0, 360)

		// Show changes
		_piece.UpdateImageRSO(_currentRotation);
		PieceImage.Redraw();
	}

	private void OnRotateCWButtonClicked(object sender, EventArgs e)
	{
		// Rotate clockwise
		_currentRotation = (_currentRotation + 90) % 360; // Keep rotation in range [0, 360)

		// Show changes
		_piece.UpdateImageRSO(_currentRotation);
		PieceImage.Redraw();
	}

	private async void OnConfirmButtonClicked(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(_piece.Image))
        {
			// Save changes in preferences
            Preferences.Set($"{_piece.Name}_rotation", _currentRotation);
            Preferences.Set(_piece.Name, _selectedImagePath);
            await DisplayAlert("Success", "Track piece updated locally.", "OK");
		}
		else
		{
			await DisplayAlert("Error", "No image selected. Please select an image before confirming.", "OK");
		}

		// Return to the previous screen
		await Navigation.PopAsync();
	}

	private async void OnCancelButtonClicked(object sender, EventArgs e)
	{
		// Discard changes and return to the previous screen
		await Navigation.PopAsync();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		// Force Landscape mode when opening page
		DeviceOrientation.SetLandscape();
	}
}
