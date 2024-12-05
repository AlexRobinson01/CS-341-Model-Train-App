using ModelTrain.Model;
using ModelTrain.Model.Pieces;
using ModelTrain.Model.Track;
using ModelTrain.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Numerics;

namespace ModelTrain.Screens;

/**
 * Description: The main track editor page, allows someone to modify the track they have opened
 * Author: Alex Robinson
 * Last updated: 12/5/2024
 */
public partial class TrackEditor : ContentPage
{
	// A green color that appears where a dragged piece would snap to if you release it
	private static readonly SKPaint SnapColor = new()
	{
		Color = SKColors.LimeGreen
	};

	// A slightly transparent gray color used for showing possible snap locations
	// and a shadow behind the currently selected piece
	private static readonly SKPaint SelectShadow = new()
	{
		Color = SKColor.Parse("#5F7F7F7F")
	};

	// Will be used for saving the loaded track to the database
	private readonly IBusinessLogic businessLogic;
	// The loaded project to retrieve the track and an ActionHandler for edit history
	private readonly PersonalProject loadedProject;
	private readonly ActionHandler actionHandler;
	// The list of currently placed TrackObjects
	private readonly List<TrackObject> objects = new();

	// The currently saved track string to compare against for an unsaved track indicator
	private string savedTrack;

	/// <summary>
	/// TrackEditor constructor - Loads an optional PersonalProject into the editor
	/// along with the necessary helper classes
	/// </summary>
	/// <param name="project">The PersonalProject to load into the Track Editor</param>
	public TrackEditor(PersonalProject? project = null)
	{
		InitializeComponent();
		
		// Adding icons to text
		Back.Text = IconFont.Arrow_back + " BACK";
		EditPieces.Text = IconFont.Settings;

		Save.Text = IconFont.Save + " SAVE";
		ChangeBackground.Text = IconFont.Image + " BACKGROUND";
		
		HotbarCollection.ItemsSource = UserHotbar.Pieces;

		// For use in saving
		businessLogic = new BusinessLogic();
		// Default to a blank project to load for now
		project ??= new() { Track = new() };

		// Cache the loaded project and prepare an ActionHandler for it
		loadedProject = project;
		// Contains a timeline of each edit made to a track to allow for undoing and redoing
		actionHandler = new(project.Track);

		// Connecting to the track's OnTrackReload event to detect when it changes
		project.Track.OnTrackReload += (s, e) => ReloadObjects();
		ReloadObjects();

		savedTrack = project.Track.GetSegmentsAsString();
		UpdateSavedIndicator();
	}

	/// <summary>
	/// Resets the list of TrackObjects so it's up to date with the track
	/// </summary>
	private void ReloadObjects()
	{
		objects.Clear();

		foreach (Segment segment in loadedProject.Track.Segments)
			objects.Add(new(segment));
	}
	
	private async void OnPieceEditButtonClicked(object sender, EventArgs e)
	{
		// Opens Piece Catalog
		await Navigation.PushAsync(new PieceCatalog());
	}

	private async void OnBackButtonClicked(object sender, EventArgs e)
    {
		bool isSaved = savedTrack == loadedProject.Track.GetSegmentsAsString();
		if (isSaved || await DisplayAlert("Unsaved Track", "Exit without saving?", "YES", "NO"))
		{
            // Revert to Portrait mode when closing page
            DeviceOrientation.SetPortrait();

            // Will return to either My Tracks or Shared Tracks depending on how the user got here
            await Navigation.PopAsync();
        }
	}

	private async void OnBackgroundButtonClicked(object sender, EventArgs e)
	{
		// Opens Change Background
		await Navigation.PushAsync(new ChangeBackground(loadedProject));
	}

	private async void OnSaveButtonClicked(object sender, EventArgs e)
	{
		// TODO: descriptive error messages, functional saving
		if (businessLogic.SaveProject(loadedProject))
		{
			savedTrack = loadedProject.Track.GetSegmentsAsString();
            await DisplayAlert("Success", "Track saved successfully!", "OK");
        }
		else
			await DisplayAlert("Failure", "Track failed to save!", "OK");

		UpdateSavedIndicator();
	}

	private void OnUndoButtonClicked(object sender, EventArgs e)
	{
		actionHandler.Undo();
		RedrawCanvas();
		UpdateSavedIndicator();
	}

	private void OnRedoButtonClicked(object sender, EventArgs e)
	{
		actionHandler.Redo();
		RedrawCanvas();
		UpdateSavedIndicator();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Force Landscape mode when opening page
        DeviceOrientation.SetLandscape();
		// Begin autosave loop
		BeginAutosave();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		// Disable autosave loop for the current track
		autosaveIndex++;
    }

    // Keeping track of various object states to determine how a user is interacting with a TrackObject
    private TrackObject? draggingObject;
	private TrackObject? selectedObject;
	private TrackObject? snappedObject;

	private void OnHotbarPieceClicked(object sender, EventArgs e)
	{
		// Ensuring parameters are valid so clicking a hotbar button creates a valid piece
		if (sender is not Button button)
			return;
		if (!Enum.TryParse(typeof(SegmentType), button.ClassId, out object? type))
			return;
		if (type is not SegmentType segmentType)
			return;

        Piece piece = new(segmentType);
        TrackObject trackObject = new(loadedProject.Track, piece);
		
		// Attempting to place the piece in the center of the screen
		// This seems to be completely different depending on the device so I'm not sure if this is possible,
		// but at least it's not in a corner somewhere
		double x = EditorFrame.Width - trackObject.BoundSegment.Size.X / 4;
		double y = EditorFrame.Height - trackObject.BoundSegment.Size.Y / 4;

		trackObject.MoveTo(x, y);
        objects.Add(trackObject);

		// Marking this in the edit history so it can be returned to later
        actionHandler.AddWaypoint();
        RedrawCanvas();
		UpdateSavedIndicator();
    }

	/// <summary>
	/// Returns whether the given x and y coordinates are within the editor's canvas
	/// </summary>
	/// <param name="x">The x coordinate to check against the editor's canvas</param>
	/// <param name="y">The y coordinate to check against the editor's canvas</param>
	/// <returns>Whether the coordinates are within the editor's canvas</returns>
	private bool IsWithinEditorFrame(double x, double y)
	{
		return x >= EditorFrame.X && x <= EditorFrame.X + EditorCanvas.CanvasSize.Width
			&& y >= EditorFrame.Y && y <= EditorFrame.Y + EditorCanvas.CanvasSize.Height;
	}

	/// <summary>
	/// Attempts to find a location where a track object can be snapped to another object,
	/// returning that location if a valid one exists, otherwise null.
	/// isSnapToStartCloser and isToSnapStartCloser are populated while determining whether
	/// a valid snap location exists for the two given objects.
	/// </summary>
	/// <param name="toSnap">The TrackObject to snap to the snapTo object</param>
	/// <param name="snapTo">The TrackObject to be snapped to</param>
	/// <param name="isSnapToStartCloser">Whether the toSnap object is closest to snapTo's
	/// start snap point</param>
	/// <param name="isToSnapStartCloser">Whether the toSnap object's start snap point
	/// is closest to the resulting snap location</param>
	/// <returns>The location where the toSnap object would be moved to to snap it,
	/// or null if no valid snap points exist</returns>
	private static Vector2? GetSnapLocation(TrackObject toSnap, TrackObject snapTo, out bool isSnapToStartCloser, out bool isToSnapStartCloser)
	{
		// Lots of vectors and matrices to determine where the snap points are on a piece and whether they can be snapped to
		Matrix3x2 snapToRotation = Matrix3x2.CreateRotation(-snapTo.BoundSegment.RotationRads);
		Vector2 snapToStartOffset = Vector2.Transform(snapTo.BoundSegment.StartSnapOffset, snapToRotation);
		Vector2 snapToEndOffset = Vector2.Transform(snapTo.BoundSegment.EndSnapOffset, snapToRotation);

		Vector2 snapToPos = new(snapTo.BoundSegment.X, snapTo.BoundSegment.Y);
		Vector2 snapToStart = snapToPos + snapToStartOffset;
		Vector2 snapToEnd = snapToPos + snapToEndOffset;

		Matrix3x2 toSnapRotation = Matrix3x2.CreateRotation(-toSnap.BoundSegment.RotationRads);
		Vector2 toSnapStartOffset = Vector2.Transform(toSnap.BoundSegment.StartSnapOffset, toSnapRotation);
		Vector2 toSnapEndOffset = Vector2.Transform(toSnap.BoundSegment.EndSnapOffset, toSnapRotation);

		Vector2 toSnapPos = new(toSnap.BoundSegment.X, toSnap.BoundSegment.Y);
		Vector2 toSnapStart = toSnapPos + toSnapStartOffset;
		Vector2 toSnapEnd = toSnapPos + toSnapEndOffset;

        isSnapToStartCloser = (toSnapPos - snapToEnd).Length() > (toSnapPos - snapToStart).Length();
        Vector2 snapLocation = isSnapToStartCloser ? snapToStart + snapToStartOffset : snapToEnd + snapToEndOffset;
		isToSnapStartCloser = (snapLocation - toSnapEnd).Length() > (snapLocation - toSnapStart).Length();

		// Ensuring there isn't already something snapped to the nearest snap point
		if (isSnapToStartCloser && snapTo.BoundSegment.SnappedStartSegment != null)
			return null;
		else if (!isSnapToStartCloser && snapTo.BoundSegment.SnappedEndSegment != null)
			return null;
		return snapLocation;
    }

	/// <summary>
	/// Finds and returns the closest TrackObject to a given position,
	/// or null if none were close enough to be detected
	/// </summary>
	/// <param name="pos">The position to check the existing TrackObjects against</param>
	/// <param name="exclude">An optional TrackObject to be excluded from detection,
	/// setting this to an object also excludes fully snapped pieces from being checked
	/// (both start and end snap points are populated)</param>
	/// <returns>The closest TrackObject to the given position, or null
	/// if none were close enough</returns>
	private TrackObject? GetClosestTrackObject(SKPoint pos, TrackObject? exclude = null)
	{
        TrackObject? minDistObject = null;
        double minDist = double.MaxValue;

		// Iterating in reverse so objects placed later have greater priority
		for (int i = objects.Count; i > 0; i--)
		{
			TrackObject obj = objects[i - 1];
            if (obj == exclude)
                continue;

            // Ensure this object can be snapped to if an object to exclude was given
            if (exclude != null)
            {
                Segment? snappedStart = obj.BoundSegment.SnappedStartSegment;
                Segment? snappedEnd = obj.BoundSegment.SnappedEndSegment;

                if (snappedStart != null && snappedEnd != null)
                    continue;
            }

            SKPoint objectPos = new(obj.BoundSegment.X, obj.BoundSegment.Y);
            double distance = (objectPos - pos).Length;

            // Checking the distance against the width of the segment as a sort of hitbox
            if (distance < obj.BoundSegment.Size.X && distance < minDist)
            {
                minDistObject = obj;
                minDist = distance;
            }
        }

		return minDistObject;
    }

	private void OnEditorPanelTouched(object sender, SKTouchEventArgs e)
	{
		// Fetching coordinates from this touch event
		SKPoint pos = e.Location;
		double x = pos.X;
		double y = pos.Y;

		switch (e.ActionType)
		{
			// The canvas was tapped, try to select a track object if one is close enough or clear selection
			case SKTouchAction.Pressed:
				TrackObject? closestObject = GetClosestTrackObject(pos);

				if (closestObject != null)
				{
					draggingObject = closestObject;
					selectedObject = closestObject;
				}
				else
					selectedObject = null;

				RedrawCanvas();
				break;
			// The canvas is being dragged, move an object if one is currently being dragged to this location
			case SKTouchAction.Moved:
				if (draggingObject != null)
				{
					// If an object is within range of the dragged one, mark it as one to snap to later if applicable
					snappedObject = GetClosestTrackObject(pos, draggingObject);
					draggingObject.MoveTo(x, y);
					// Unsnap the current object on both ends to pull it away
					draggingObject.UnsnapStart();
					draggingObject.UnsnapEnd();
				}

				RedrawCanvas();
				break;
			// The canvas has been released in some manner, stop dragging and attempt to snap the object
			case SKTouchAction.Released:
            case SKTouchAction.Exited:
            case SKTouchAction.Cancelled:
				// Delete the object if it has left the canvas
				if (draggingObject != null && !IsWithinEditorFrame(x, y))
				{
					draggingObject.RemoveFrom(loadedProject.Track);
					objects.Remove(draggingObject);
				}
				else if (draggingObject != null && snappedObject != null)
				{
					// Attempt to snap this piece to the closest object detected while dragging
					Vector2? snapLocation = GetSnapLocation(draggingObject, snappedObject, out bool isStartCloser, out bool isDragStartCloser);
					
					if (snapLocation != null)
					{
						Vector2 snapRotationOffset = snappedObject.BoundSegment.SnapRotationOffset;
						Vector2 dragRotationOffset = draggingObject.BoundSegment.SnapRotationOffset;
						// TODO: fix rotation math
						// Gets the degree of rotation to snap the dragged object to - took a *lot* of trial and error
						float snapRotation = isStartCloser ? snapRotationOffset.X : snapRotationOffset.Y;
						float rotation = snappedObject.BoundSegment.Rotation - snapRotation - dragRotationOffset.X;
						if (isDragStartCloser) // not yet working
							rotation += 2 * (180 - dragRotationOffset.Y) - dragRotationOffset.X;
						rotation = rotation % 360;

						// Snap the dragged object by position/rotation to the snap location
						draggingObject.MoveTo(snapLocation?.X ?? 0, snapLocation?.Y ?? 0);
						draggingObject.Rotate((int)rotation);

						// Snap the snapped object to the dragged one on whichever end is applicable
						if (isStartCloser)
							snappedObject.SnapToStart(draggingObject.BoundSegment);
						else
							snappedObject.SnapToEnd(draggingObject.BoundSegment);

						// Snap the dragged object to the snapped one on whichever end is applicable
						if (!isDragStartCloser)
							draggingObject.SnapToStart(snappedObject.BoundSegment);
						else
							draggingObject.SnapToEnd(snappedObject.BoundSegment);
					}
                }

				// Stop dragging the current object and update the edit history
				draggingObject = null;
				snappedObject = null;
				actionHandler.AddWaypoint();

				// Update track and save button with current data
				RedrawCanvas();
                UpdateSavedIndicator();
                break;
        }

		// Tell the OS that we would like to receive more touch events (move, release)
		e.Handled = true;
	}

	/// <summary>
	/// Marks the editor canvas as needing to be redrawn; should be called after modifying the track
	/// </summary>
	private void RedrawCanvas()
	{
		// Marks EditorCanvas as needing to be redrawn
		EditorCanvas.InvalidateSurface();
	}

	private void OnEditorPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
	{
		// This doesn't appear to print when emulated so this'll be something to look into
		Console.WriteLine(e.Scale);
	}

    private void OnPaintEditorCanvas(object sender, SKPaintSurfaceEventArgs e)
    {
		// This method fires when the canvas needs to be redrawn

        // Prepare the canvas to be drawn to
		SKCanvas canvas = e.Surface.Canvas;
		canvas.Clear();

		// Draw background on the canvas
		SKRect canvasRect = new(0, 0, e.Info.Width, e.Info.Height);
		SKBitmap? bkgd = ImageFileDecoder.GetBitmapFromFile(loadedProject.BackgroundImage ?? "");

		if (bkgd != null)
			canvas.DrawBitmap(bkgd, canvasRect);

		// Draw each track piece on the canvas
		foreach (TrackObject obj in objects)
		{
			// Get the image for this object as an embedded resource
			string resourceID = obj.BoundPiece.Image;
			SKBitmap? bmp = ImageFileDecoder.GetBitmapFromFile(resourceID);

            if (bmp != null)
            {
				// Prepare bitmap data for drawing bmp to the canvas
				Vector2 size = obj.BoundSegment.Size;
				Vector2 pos = -size / 2;
                SKRect dest = new(0, 0, size.X, size.Y);

                // Align the canvas's center point where this piece should be rendered and draw the image
                canvas.Translate(obj.BoundSegment.X, obj.BoundSegment.Y);
				// canvas.RotateDegrees applies clockwise, while trig functions apply counterclockwise
				canvas.RotateDegrees(-obj.BoundSegment.Rotation);
				SKMatrix curMatrix = canvas.TotalMatrix;

				// Apply piece image rotation, scale, and offset then draw the bitmap
				canvas.Translate(pos.X + dest.Width / 2, pos.Y + dest.Height / 2);
				canvas.RotateDegrees(obj.BoundPiece.ImageRotation);
                canvas.Scale(obj.BoundPiece.ImageScale);
                canvas.Translate(-dest.Width / 2, -dest.Height / 2);

				canvas.Translate(obj.BoundPiece.ImageOffset.X, obj.BoundPiece.ImageOffset.Y);
				canvas.DrawBitmap(bmp, dest);
				
				// Reset canvas matrix to before image rotation, scale, and offset
				canvas.SetMatrix(curMatrix);

				// Additional circles should be rendered if an object is being dragged
				if (draggingObject != null && draggingObject != obj)
				{
                    Vector2 startOffset = obj.BoundSegment.StartSnapOffset;
                    Vector2 endOffset = obj.BoundSegment.EndSnapOffset;

					// Draw a small snap shadow if the dragged piece is close enough to the current object
                    if (snappedObject == obj)
                    {
						if (obj.BoundSegment.SnappedStartSegment == null)
							canvas.DrawCircle(startOffset.X * 2, startOffset.Y * 2, 30, SelectShadow);
						if (obj.BoundSegment.SnappedEndSegment == null)
							canvas.DrawCircle(endOffset.X * 2, endOffset.Y * 2, 30, SelectShadow);
                    }

					// Draw green circles where open snap points are
					if (obj.BoundSegment.SnappedStartSegment == null)
						canvas.DrawCircle(startOffset.X, startOffset.Y, 10, SnapColor);
					if (obj.BoundSegment.SnappedEndSegment == null)
                        canvas.DrawCircle(endOffset.X, endOffset.Y, 10, SnapColor);
                }

				// Draw select shadow around the piece if it's selected
				if (selectedObject == obj || draggingObject == obj)
					canvas.DrawCircle(0, 0, obj.BoundSegment.Size.X / 2, SelectShadow);
				
				// Undoing previous translation/rotation
				canvas.ResetMatrix();
            }
        }

        if (snappedObject != null && draggingObject != null)
        {
			// Draw a green circle where the dragging object may or may not want to snap to
            Vector2? snapLocation = GetSnapLocation(draggingObject, snappedObject, out _, out _);

            if (snapLocation != null)
                canvas.DrawCircle(snapLocation?.X ?? 0, snapLocation?.Y ?? 0, 25, SnapColor);
        }

		// Just in case some residual changes in the canvas's matrix could carry over to another draw
		canvas.ResetMatrix();
    }

	/// <summary>
	/// Changes the background color of the Save button depending on whether the track is saved
	/// </summary>
	private void UpdateSavedIndicator()
	{
		bool isSaved = savedTrack == loadedProject.Track.GetSegmentsAsString();
		Save.BackgroundColor = isSaved ? Colors.Green : Colors.Orange;
	}

	private int autosaveIndex = 0;

	/// <summary>
	/// Begins a 5 minute autosave loop running on a separate thread
	/// </summary>
	private void BeginAutosave()
	{
		int index = ++autosaveIndex;

		// Running autosave loop on a separate thread
		Task.Run(() =>
		{
			while (index == autosaveIndex)
			{
				// 5 minutes in milliseconds
				Thread.Sleep(5 * 60000);

				if (index == autosaveIndex)
				{
					// Save the track
					bool success = businessLogic.SaveProject(loadedProject);
					if (success)
						savedTrack = loadedProject.Track.GetSegmentsAsString();
					UpdateSavedIndicator();
				}
			}
		});
	}
}