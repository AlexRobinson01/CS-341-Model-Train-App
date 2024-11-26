using ModelTrain.Model;
using ModelTrain.Model.Pieces;
using ModelTrain.Model.Track;
using ModelTrain.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Numerics;
using System.Reflection;

namespace ModelTrain.Screens;

/**
 * Description: The main track editor page, allows someone to modify the track they have opened
 * Author: Alex Robinson
 * Last updated: 11/24/2024
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

	// TODO: generalize project to allow both PersonalProjects and SharedProjects
	public TrackEditor(PersonalProject? project = null)
	{
		InitializeComponent();
		
		// Adding icons to text
		Back.Text = IconFont.Arrow_back + " BACK";
		EditPieces.Text = IconFont.Settings;

		Save.Text = IconFont.Save + " SAVE";
		ChangeBackground.Text = IconFont.Image + " CHANGE BACKGROUND";
		
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
		// TODO: descriptive error messages, functional saving
		if (businessLogic.SaveProject(loadedProject))
			await DisplayAlert("Success", "Track saved successfully!", "OK");
		else
			await DisplayAlert("Failure", "Track failed to save!", "OK");
	}

	private void OnUndoButtonClicked(object sender, EventArgs e)
	{
		actionHandler.Undo();
		RedrawCanvas();
	}

	private void OnRedoButtonClicked(object sender, EventArgs e)
	{
		actionHandler.Redo();
		RedrawCanvas();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Force Landscape mode when opening page
        DeviceOrientation.SetLandscape();
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
		// Lots of vectors and math to determine where the snap points are on a piece and whether they can be snapped to
		Vector2 snapToStartOffset = snapTo.BoundSegment.StartSnapOffset;
		Vector2 snapToEndOffset = snapTo.BoundSegment.EndSnapOffset;

        Vector2 snapToPos = new(snapTo.BoundSegment.X, snapTo.BoundSegment.Y);
        Vector2 snapToStart = snapToStartOffset + snapToPos;
        Vector2 snapToEnd = snapToEndOffset + snapToPos;
        Vector2 snapToRotation = snapTo.BoundSegment.SnapRotationOffset;
		snapToRotation += Vector2.One * snapTo.BoundSegment.Rotation;

		Vector2 toSnapPos = new(toSnap.BoundSegment.X, toSnap.BoundSegment.Y);
        Vector2 toSnapRotation = toSnap.BoundSegment.SnapRotationOffset;
		toSnapRotation += Vector2.One * toSnap.BoundSegment.Rotation;

        isSnapToStartCloser = (toSnapPos - snapToEnd).Length() > (toSnapPos - snapToStart).Length();
        Vector2 snapLocation = isSnapToStartCloser ? snapToStart + snapToStartOffset : snapToEnd + snapToEndOffset;

        float rotationOffset = isSnapToStartCloser ? snapToRotation.X : snapToRotation.Y;
        float startRotation = (rotationOffset - toSnapRotation.X) % 360;
        float endRotation = (rotationOffset - toSnapRotation.Y) % 360;

        isToSnapStartCloser = Math.Abs(endRotation) > Math.Abs(startRotation);

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

        foreach (TrackObject obj in objects)
        {
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
						float rotation = snappedObject.BoundSegment.Rotation + snapRotationOffset.Y + dragRotationOffset.X;

						if (!isDragStartCloser)
							rotation += dragRotationOffset.Y - dragRotationOffset.X;
						rotation %= 360;

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
				RedrawCanvas();
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

		// Will be used for retrieving embedded images for track objects
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        // Prepare the canvas to be drawn to
		SKCanvas canvas = e.Surface.Canvas;
		canvas.Clear();

		foreach (TrackObject obj in objects)
		{
			// Get the image for this object as an embedded resource
			string resourceID = obj.BoundPiece.Image;
            using Stream? stream = assembly.GetManifestResourceStream(resourceID);

            if (stream != null)
            {
				// Prepare a bitmap to be drawn to the canvas
                SKBitmap bmp = SKBitmap.Decode(stream);
				Vector2 size = obj.BoundSegment.Size;
				Vector2 pos = -size / 2;

				// Align the canvas's center point where this piece should be rendered and draw the image
				SKRect dest = new(pos.X, pos.Y, pos.X + size.X, pos.Y + size.Y);
				canvas.Translate(obj.BoundSegment.X, obj.BoundSegment.Y);
				canvas.RotateDegrees(obj.BoundSegment.Rotation);
                canvas.DrawBitmap(bmp, dest);

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
}