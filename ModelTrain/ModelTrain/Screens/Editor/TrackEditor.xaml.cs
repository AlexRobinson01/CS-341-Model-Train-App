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
 * Last updated: 12/8/2024
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

	// A gray color showing the outline of the rotation dial
	private static readonly SKPaint RotationDialOuter = new()
	{
		Color = SKColor.Parse("#FF7F7F7F")
	};

	// A light gray-ish color inside the rotation dial
	private static readonly SKPaint RotationDialInner = new()
	{
		Color = SKColor.Parse("#FFD3D3D3")
	};

	// Will be used for saving the loaded track to the database
	private readonly IBusinessLogic businessLogic;
	// The loaded project to retrieve the track and an ActionHandler for edit history
	private readonly PersonalProject loadedProject;
	private readonly ActionHandler actionHandler;
	// The list of currently placed TrackObjects
	private readonly List<TrackObject> objects = [];

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
	private TrackObject? rotatingObject;

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

		// Placing the piece in the center of the screen
		double x = EditorCanvas.CanvasSize.Width / 2;
		double y = EditorCanvas.CanvasSize.Height / 2;

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
		return x >= EditorCanvas.X && x <= EditorCanvas.X + EditorCanvas.CanvasSize.Width
			&& y >= EditorCanvas.Y && y <= EditorCanvas.Y + EditorCanvas.CanvasSize.Height;
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
		// Lots of vectors to determine where the snap points are on a piece and whether they can be snapped to
		Vector2 snapToStart = snapTo.BoundSegment.GetStartSnapPosition();
		Vector2 snapToEnd = snapTo.BoundSegment.GetEndSnapPosition();

        Vector2 toSnapPos = new(toSnap.BoundSegment.X, toSnap.BoundSegment.Y);
        Vector2 toSnapStart = toSnap.BoundSegment.GetStartSnapPosition();
		Vector2 toSnapEnd = toSnap.BoundSegment.GetEndSnapPosition();

		isSnapToStartCloser = (toSnapPos - snapToEnd).Length() > (toSnapPos - snapToStart).Length();
		Vector2 snapLocation = isSnapToStartCloser ? snapToStart : snapToEnd;
		isToSnapStartCloser = (snapLocation - toSnapEnd).Length() > (snapLocation - toSnapStart).Length();

		// Ensuring there isn't already something snapped to the nearest snap point
		if (isSnapToStartCloser && snapTo.BoundSegment.SnappedStartSegment != null)
			return null;
		else if (!isSnapToStartCloser && snapTo.BoundSegment.SnappedEndSegment != null)
			return null;
		return isSnapToStartCloser ? snapTo.BoundSegment.GetExtendedStartSnapPosition()
            : snapTo.BoundSegment.GetExtendedEndSnapPosition();
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
		// Used for checking if the user is trying to rotate a piece
        SKPoint rotationDialPos = EditorCanvas.CanvasSize.ToPoint() - new SKPoint(85, 85);

        // Fetching coordinates from this touch event
        SKPoint pos = e.Location;
		float x = pos.X;
		float y = pos.Y;

		switch (e.ActionType)
		{
			// The canvas was tapped, try to select a track object if one is close enough or clear selection
			// unless piece rotation was attempted
			case SKTouchAction.Pressed:
				TrackObject? closestObject = GetClosestTrackObject(pos);

				if (closestObject != null)
				{
					draggingObject = closestObject;
					selectedObject = closestObject;
				}
				else if ((rotationDialPos - pos).Length < 100)
					rotatingObject = selectedObject;
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
				else if (rotatingObject != null)
				{
					Segment? inSnap = rotatingObject.BoundSegment.SnappedStartSegment;
					Segment? outSnap = rotatingObject.BoundSegment.SnappedEndSegment;

					// Making sure this piece isn't snapped before trying to rotate it
					if (inSnap == null && outSnap == null)
					{
                        // Getting a unit vector of the position relative to the rotation dial
                        Vector2 rotation = new(x - rotationDialPos.X, y - rotationDialPos.Y);
                        rotation /= rotation.Length();
                        // That unit vector can now give the angle the user is rotating the piece to
                        // (with some more trig of course)
                        float angle = -MathF.Atan2(rotation.Y, rotation.X);

                        // Converting to degrees and adjusting it before applying it to this piece
                        rotatingObject.Rotate((int)(angle * 180 / MathF.PI - 90) % 360);
                    }
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
						// Gets the degree of rotation to snap the dragged object to - took a *lot* of trial and error
						float snapRotation = isStartCloser ? snapRotationOffset.X : snapRotationOffset.Y;
						float rotation = snappedObject.BoundSegment.Rotation - snapRotation - dragRotationOffset.X;
						if (isDragStartCloser) // it works now!
							rotation += 360 - dragRotationOffset.X + dragRotationOffset.Y;
						rotation %= 360;

						// Snap the dragged object by position/rotation to the snap location
						draggingObject.MoveTo(snapLocation?.X ?? 0, snapLocation?.Y ?? 0);
						draggingObject.Rotate((int)rotation);

						// Snap the snapped object to the dragged one
						// on whichever end is applicable
						if (isStartCloser)
							snappedObject.SnapToStart(draggingObject.BoundSegment);
						else
							snappedObject.SnapToEnd(draggingObject.BoundSegment);

						// Snap the dragged object to the snapped one
						// on whichever end is applicable
						if (!isDragStartCloser)
							draggingObject.SnapToStart(snappedObject.BoundSegment);
						else
							draggingObject.SnapToEnd(snappedObject.BoundSegment);

						// Secondary snap check in case this object fills a gap between two existing objects

						// Used to determine if any other objects are close enough to snap to
						Vector2 toCheck = isDragStartCloser
							? draggingObject.BoundSegment.GetStartSnapPosition()
							: draggingObject.BoundSegment.GetEndSnapPosition();
						float checkRotation = draggingObject.BoundSegment.Rotation
							+ (isDragStartCloser ? dragRotationOffset.X : dragRotationOffset.Y);

						TrackObject? toSnap = null;

						// Finding nearby objects to attempt to snap 
						foreach (TrackObject obj in objects)
						{
							if (obj != draggingObject)
							{
								if (obj.BoundSegment.SnappedStartSegment == null)
								{
									// Ensuring rotations and positions align before snapping
									float objRotation = obj.BoundSegment.Rotation
										+ obj.BoundSegment.SnapRotationOffset.X;
									float rotationDiff = Math.Abs(checkRotation - (objRotation + 180)) % 360;

									Vector2 snapPos = obj.BoundSegment.GetStartSnapPosition();

									// Accounting for floating point errors in difference checks
									if ((snapPos - toCheck).Length() < 0.01f && rotationDiff < 0.01f)
									{
										// Snap the dragging object to this object and exit the loop
										toSnap = obj;
										toSnap.SnapToStart(draggingObject.BoundSegment);
										break;
									}
								}

								if (obj.BoundSegment.SnappedEndSegment == null)
								{
									// Ensuring rotations and positions align before snapping
									float objRotation = obj.BoundSegment.Rotation
										+ obj.BoundSegment.SnapRotationOffset.Y;
									float rotationDiff = Math.Abs(checkRotation - (objRotation + 180)) % 360;

									Vector2 snapPos = obj.BoundSegment.GetEndSnapPosition();
									// Accounting for floating point errors in difference checks
									if ((snapPos - toCheck).Length() < 0.01f)
									{
										// Snap the dragging object to this object and exit the loop
										toSnap = obj;
										toSnap.SnapToEnd(draggingObject.BoundSegment);
										break;
									}
								}
							}
						}

						// Check if another object was snapped to the dragging one
						if (toSnap != null)
						{
							if (isDragStartCloser)
								draggingObject.SnapToStart(toSnap.BoundSegment);
							else
								draggingObject.SnapToEnd(toSnap.BoundSegment);
						}
					}
				}

				// Stop interacting with the current object and update the edit history
				draggingObject = null;
				snappedObject = null;
				rotatingObject = null;
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

		// Show rotation dial
		if (selectedObject != null && draggingObject == null)
		{
			// Draw main dial
			canvas.Translate(e.Info.Width - 85, e.Info.Height - 85);
			canvas.DrawCircle(0, 0, 50, RotationDialOuter);
			canvas.DrawCircle(0, 0, 48, RotationDialInner);

			// Draw secondary dial for rotation
			float rotationRads = -MathF.PI * selectedObject.BoundSegment.Rotation / 180;
			Matrix3x2 dialRotation = Matrix3x2.CreateRotation(rotationRads);
			Vector2 offset = Vector2.Transform(-Vector2.UnitY, dialRotation) * 49;
			
			canvas.Translate(offset.X, offset.Y);
			canvas.DrawCircle(0, 0, 15, RotationDialOuter);
			canvas.DrawCircle(0, 0, 13, RotationDialInner);

			canvas.ResetMatrix();
		}

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