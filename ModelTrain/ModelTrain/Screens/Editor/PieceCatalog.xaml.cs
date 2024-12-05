namespace ModelTrain.Screens;

using ModelTrain.Model;
using ModelTrain.Model.Pieces;
using ModelTrain.Model.Track;
using ModelTrain.Screens.Components;
using ModelTrain.Services;

/**
 * Description: Allows the user to change which pieces they see in the list to the right
 * of the track editor page, in case they don't like their current list's order.
 * Author: Alex Robinson
 * Last updated: 12/5/2024
 */
public partial class PieceCatalog : ContentPage
{
	private readonly PieceList defaultPieces = PieceInfo.GetDefaultPieces();
	private readonly PieceImage[] pieceImages;

	public PieceCatalog()
	{
		InitializeComponent();

		Edit.Text = IconFont.Settings + " EDIT PIECE";
		Back.Text = IconFont.Arrow_back + " BACK";

		HotbarCollection.ItemsSource = UserHotbar.Pieces;

		// pieceImages[0] will map to defaultPieces[0], etc.
		pieceImages = [LLImage, LImage, CImage, RImage, RRImage];

		RedrawPieces();
	}

	private async void OnEditButtonClicked(object sender, EventArgs e)
	{
		// Opens Piece Editor
		await Navigation.PushAsync(new PieceEditor(defaultPieces[2]));
	}

	private async void OnBackButtonClicked(object sender, EventArgs e)
	{
		// Should have gotten here from TrackEditor, pop to avoid memory leak
		await Navigation.PopAsync();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Force Landscape mode when opening page
        DeviceOrientation.SetLandscape();
    }

	/// <summary>
	/// Redraws all 5 piece images that can be clicked
	/// </summary>
	private void RedrawPieces()
	{
		for (int i = 0; i < pieceImages.Length; i++)
		{
			PieceImage image = pieceImages[i];
			// Wrap around defaultPieces if less than 5 pieces exist in the application
			int pieceIndex = i % defaultPieces.Count;

			// Update the path this 
			image.ClassId = defaultPieces[pieceIndex].Name;
			image.Redraw();
		}
	}

	private void OnRotateLeftButtonClicked(object sender, EventArgs e)
	{
		defaultPieces.RotateRight();
        RedrawPieces();
	}

	private void OnRotateRightButtonClicked(object sender, EventArgs e)
	{
		defaultPieces.RotateLeft();
        RedrawPieces();
	}

	private void OnLLButtonClicked(object sender, EventArgs e)
	{
		defaultPieces.RotateRight();
		defaultPieces.RotateRight();
        RedrawPieces();
	}

	private void OnLButtonClicked(object sender, EventArgs e)
	{
		defaultPieces.RotateRight();
        RedrawPieces();
	}

	private void OnRButtonClicked(object sender, EventArgs e)
	{
		defaultPieces.RotateLeft();
        RedrawPieces();
	}

	private void OnRRButtonClicked(object sender, EventArgs e)
	{
		defaultPieces.RotateLeft();
		defaultPieces.RotateLeft();
        RedrawPieces();
    }

    private void OnCButtonClicked(object sender, EventArgs e)
    {
        // The piece rendered on the center button is at index 2
        UserHotbar.AddPiece(defaultPieces[2].SegmentType);

        // TODO: save to user preferences
    }

    private void OnHotbarPieceClicked(object sender, EventArgs e)
    {
        // Ensuring parameters are valid so clicking a hotbar button properly removes the piece
        if (sender is not Button button)
            return;
        if (!Enum.TryParse(typeof(SegmentType), button.ClassId, out object? type))
            return;
        if (type is not SegmentType segmentType)
            return;

		UserHotbar.RemovePiece(segmentType);

		// TODO: save to user preferences
    }
}