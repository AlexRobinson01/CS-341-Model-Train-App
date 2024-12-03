using ModelTrain.Model;
using ModelTrain.Services;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using ModelTrain.Model.Pieces;

namespace ModelTrain.Screens;

/**
 * Description: Allows a user to change the image associated with their selected piece,
 * in case they don't like the appearane of the current piece(s).
 * Author: Alex Robinson & Taylor Showalter
 * Date: 11/27/2024
 */
public partial class PieceEditor : ContentPage
{
	// Store the path and rotation state
	private string _selectedImagePath;
	private double _currentRotation = 0;
	private readonly Piece _piece;
	public PieceEditor(Piece piece)
	{
		InitializeComponent();
		_piece = piece;

		RotateCCW.Text = IconFont.Rotate_left + " CCW";
		RotateCW.Text = IconFont.Rotate_right + " CW";

		Confirm.Text = IconFont.Check + " CONFIRM";
		ChangeImage.Text = IconFont.Image + " CHANGE IMAGE";
		Cancel.Text = IconFont.Cancel + " CANCEL";

		// Assign click events
		ChangeImage.Clicked += OnChangeImageButtonClicked;
		RotateCCW.Clicked += OnRotateCCWButtonClicked;
		RotateCW.Clicked += OnRotateCWButtonClicked;
	}

	private async void OnChangeImageButtonClicked(object sender, EventArgs e)
	{
		try
		{
			// Pick an image file
			FileResult result = await FilePicker.PickAsync(new PickOptions
			{
				PickerTitle = "Select an Image for the Track Piece",
				FileTypes = FilePickerFileType.Images // Restrict to image files
			});

			if (result != null)
			{
				// Save the selected image path
				_selectedImagePath = result.FullPath;

				// Open the stream once, load the image, and dispose of the stream immediately
				using Stream stream = await result.OpenReadAsync();
				PieceImage.Source = ImageSource.FromStream(() => new MemoryStream(ReadFully(stream)));
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"Failed to select image: {ex.Message}", "OK");
		}
	}


	private static byte[] ReadFully(Stream input)
	{
		using MemoryStream ms = new MemoryStream();
		input.CopyTo(ms);
		return ms.ToArray();
	}

	private void OnRotateCCWButtonClicked(object sender, EventArgs e)
	{
		// Rotate counterclockwise
		_currentRotation -= 90;
		if (_currentRotation < 0) _currentRotation += 360; // Keep rotation in range [0, 360)
		PieceImage.Rotation = _currentRotation;
	}

	private void OnRotateCWButtonClicked(object sender, EventArgs e)
	{
		// Rotate clockwise
		_currentRotation += 90;
		if (_currentRotation >= 360) _currentRotation -= 360; // Keep rotation in range [0, 360)
		PieceImage.Rotation = _currentRotation;
	}

	private async void OnConfirmButtonClicked(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(_selectedImagePath))
		{
			await DisplayAlert("Success", "Track piece updated locally.", "OK");
			Preferences.Set($"{_piece.Name}_rotation", _currentRotation);
			Preferences.Set(_piece.Name, _selectedImagePath);
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
