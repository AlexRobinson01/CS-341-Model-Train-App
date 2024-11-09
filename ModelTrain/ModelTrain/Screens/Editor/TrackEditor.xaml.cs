using ModelTrain.Model;
using ModelTrain.Model.Pieces;
using ModelTrain.Model.Track;
using ModelTrain.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;
namespace ModelTrain.Screens;

/**
 * Description: The main track editor page, allows someone to modify the track they have opened.
 * Author: Alex Robinson
 * Date: 10/30/2024
 */
public partial class TrackEditor : ContentPage
{
	private readonly IBusinessLogic businessLogic;

	private readonly PersonalProject loadedProject;
	private readonly ActionHandler actionHandler;

	private readonly List<TrackObject> objects = new();

	// TODO: generalize project to allow both PersonalProjects and SharedProjects
	public TrackEditor(PersonalProject? project = null)
	{
		InitializeComponent();
		
		// Adding icons to text
		Back.Text = IconFont.Arrow_back + " BACK";
		EditPieces.Text = IconFont.Settings;

		Save.Text = IconFont.Save + " SETTINGS";
		ChangeBackground.Text = IconFont.Image + " CHANGE BACKGROUND";
		
		HotbarCollection.ItemsSource = UserHotbar.Pieces;

		// For use in saving
		businessLogic = new BusinessLogic();
		// Default to a blank project to load for now
		project ??= new();

		// Cache the loaded project and prepare an ActionHandler for it
		loadedProject = project;
		// Contains a timeline of each edit made to a track to allow for undoing and redoing
		actionHandler = new(project.Track);

		project.Track.OnTrackReload += (s, e) => ReloadObjects();
		ReloadObjects();
	}

	private void ReloadObjects()
	{
		objects.Clear();

		foreach (Segment segment in loadedProject.Track.Segments)
		{
			TrackObject obj = new(segment)
			{
				ActionHandler = actionHandler
			};
			
			objects.Add(obj);
		}
	}
	
	private async void OnPieceEditButtonClicked(object sender, EventArgs e)
	{
		// Opens Piece Catalog
		await Navigation.PushAsync(new PieceCatalog());
	}

	private async void OnBackButtonClicked(object sender, EventArgs e)
	{
		// Will return to either My Tracks or Shared Tracks depending on how the user got here
		await Navigation.PopAsync();
	}

	private async void OnBackgroundButtonClicked(object sender, EventArgs e)
	{
		// Opens Change Background
		await Navigation.PushAsync(new ChangeBackground());
	}

	private async void OnSaveButtonClicked(object sender, EventArgs e)
	{
		// TODO: descriptive error messages
		if (businessLogic.SaveProject(loadedProject))
			await DisplayAlert("Success", "Track saved successfully!", "OK");
		else
			await DisplayAlert("Failure", "Track failed to save!", "OK");
	}

	private void OnUndoButtonClicked(object sender, EventArgs e)
	{
		actionHandler.Undo();
	}

	private void OnRedoButtonClicked(object sender, EventArgs e)
	{
		actionHandler.Redo();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Force Landscape mode when opening page
        DeviceOrientation.SetLandscape();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Revert to Portrait mode when closing page
        DeviceOrientation.SetPortrait();
    }

	private TrackObject? draggingObject;

	private void OnHotbarPiecePressed(object sender, EventArgs e)
	{
		if (sender is Button button)
		{
			// TODO: store which button was pressed to begin dragging
			string name = button.StyleId;
			button.BackgroundColor = Color.FromRgba(255, 0, 0, 255);
        }
	}

	private bool IsWithinEditorFrame(double x, double y)
	{
		return x >= EditorFrame.X && x <= EditorFrame.X + EditorFrame.Width
			&& y >= EditorFrame.Y && y <= EditorFrame.Y + EditorFrame.Height;
	}

	private void OnEditorWindowTouched(object sender, SKTouchEventArgs e)
	{
		SKPoint pos = e.Location;
		double x = pos.X;
		double y = pos.Y;

		Console.WriteLine(e.ActionType);

		switch (e.ActionType)
		{
			case SKTouchAction.Pressed:

				break;
			case SKTouchAction.Moved:
				if (draggingObject == null)
					return;

				// Move segment

				break;
			case SKTouchAction.Exited:
			case SKTouchAction.Cancelled:
			case SKTouchAction.Released:
				if (draggingObject == null)
					return;

                if (IsWithinEditorFrame(x, y))
                {
                    // Place segment
                }
                else
                {
                    // Remove segment
                }

                break;
		}

		e.Handled = true;
	}

	private void OnEditorPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
	{
		Console.WriteLine(e.Scale);
	}
}