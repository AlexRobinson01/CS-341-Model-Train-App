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
        // Revert to Portrait mode when closing page
        DeviceOrientation.SetPortrait();

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

    private TrackObject? draggingObject;

	private void OnHotbarPieceClicked(object sender, EventArgs e)
	{
		if (sender is not Button button)
			return;
		if (!Enum.TryParse(typeof(SegmentType), button.ClassId, out object? type))
			return;
		if (type is not SegmentType segmentType)
			return;

        PieceBase piece = new(segmentType);
        TrackObject trackObject = new(loadedProject.Track, piece);

		double x = EditorFrame.X + EditorFrame.Width / 2;
		double y = EditorFrame.Y + EditorFrame.Height / 2;

		trackObject.MoveTo(x, y);
        objects.Add(trackObject);
    }

	private bool IsWithinEditorFrame(double x, double y)
	{
		return x >= EditorFrame.X && x <= EditorFrame.X + EditorFrame.Width
			&& y >= EditorFrame.Y && y <= EditorFrame.Y + EditorFrame.Height;
	}

	private void OnEditorPanelTouched(object sender, SKTouchEventArgs e)
	{
		SKPoint pos = e.Location;
		double x = pos.X;
		double y = pos.Y;

		Console.WriteLine(e.ActionType);

		switch (e.ActionType)
		{
			case SKTouchAction.Pressed:
				TrackObject? minDistObject = null;
				double minDist = double.MaxValue;

				foreach (TrackObject trackObject in objects)
				{
					SKPoint objectPos = new(trackObject.BoundSegment.X, trackObject.BoundSegment.Y);
					double distance = (objectPos - pos).Length;

					if (distance < 50 && distance < minDist)
					{
						minDistObject = trackObject;
						minDist = distance;
					}
				}

				if (minDistObject != null)
					draggingObject = minDistObject;

				break;
			case SKTouchAction.Moved:
				draggingObject?.MoveTo((float)x, (float)y);
				break;
			case SKTouchAction.Released:
				draggingObject = null;
				break;
            case SKTouchAction.Exited:
            case SKTouchAction.Cancelled:
				if (draggingObject != null && !IsWithinEditorFrame(x, y))
				{
					draggingObject.RemoveFrom(loadedProject.Track);
					objects.Remove(draggingObject);
				}

				draggingObject = null;
				break;
        }

		e.Handled = true;
	}

	private void OnEditorPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
	{
		Console.WriteLine(e.Scale);
	}
}