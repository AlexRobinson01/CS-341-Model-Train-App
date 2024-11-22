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
 * Description: The main track editor page, allows someone to modify the track they have opened.
 * Author: Alex Robinson
 * Date: 10/30/2024
 */
public partial class TrackEditor : ContentPage
{
	private static readonly SKPaint SnapColor = new()
	{
		Color = SKColors.LimeGreen
	};

	private static readonly SKPaint SelectShadow = new()
	{
		Color = SKColor.Parse("#5F7F7F7F")
	};

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

		project.Track.OnTrackReload += (s, e) => ReloadObjects();
		ReloadObjects();
	}

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
		// TODO: descriptive error messages
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

    private TrackObject? draggingObject;
	private TrackObject? selectedObject;
	private TrackObject? snappedObject;

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
		
		double x = EditorFrame.Width - trackObject.BoundSegment.Size.X / 4;
		double y = EditorFrame.Height - trackObject.BoundSegment.Size.Y / 4;

		trackObject.MoveTo(x, y);
        objects.Add(trackObject);

        actionHandler.AddWaypoint();
        RedrawCanvas();
    }

	private bool IsWithinEditorFrame(double x, double y)
	{
		return x >= EditorFrame.X && x <= EditorFrame.X + EditorCanvas.CanvasSize.Width
			&& y >= EditorFrame.Y && y <= EditorFrame.Y + EditorCanvas.CanvasSize.Height;
	}

	private static Vector2? GetSnapLocation(TrackObject toSnap, TrackObject snapTo, out bool isSnapToStartCloser, out bool isToSnapStartCloser)
	{
		Vector2 snapToStartOffset = snapTo.BoundSegment.StartSnapOffset;
		Vector2 snapToEndOffset = snapTo.BoundSegment.EndSnapOffset;

        Vector2 snapToPos = new(snapTo.BoundSegment.X, snapTo.BoundSegment.Y);
        Vector2 snapToStart = snapToStartOffset + snapToPos;
        Vector2 snapToEnd = snapToEndOffset + snapToPos;
        Vector2 snapToRotation = snapTo.BoundSegment.SnapRotationOffset;

		Vector2 toSnapPos = new(toSnap.BoundSegment.X, toSnap.BoundSegment.Y);
        Vector2 toSnapRotation = toSnap.BoundSegment.SnapRotationOffset;

        isSnapToStartCloser = (toSnapPos - snapToEnd).Length() > (toSnapPos - snapToStart).Length();
        Vector2 snapLocation = isSnapToStartCloser ? snapToStart + snapToStartOffset : snapToEnd + snapToEndOffset;

        float rotationOffset = isSnapToStartCloser ? snapToRotation.X : snapToRotation.Y;
        float startRotation = (rotationOffset - toSnapRotation.X) % 360;
        float endRotation = (rotationOffset - toSnapRotation.Y) % 360;

        isToSnapStartCloser = Math.Abs(endRotation) > Math.Abs(startRotation);

		if (isSnapToStartCloser && snapTo.BoundSegment.SnappedStartSegment != null)
			return null;
		else if (!isSnapToStartCloser && snapTo.BoundSegment.SnappedEndSegment != null)
			return null;
		return snapLocation;
    }

	private TrackObject? GetClosestTrackObject(SKPoint pos, TrackObject? exclude = null)
	{
        TrackObject? minDistObject = null;
        double minDist = double.MaxValue;

        foreach (TrackObject obj in objects)
        {
			if (obj == exclude)
				continue;

			if (exclude != null)
			{
				Segment? snappedStart = obj.BoundSegment.SnappedStartSegment;
				Segment? snappedEnd = obj.BoundSegment.SnappedEndSegment;

				if (snappedStart != null && snappedEnd != null)
					continue;
			}

            SKPoint objectPos = new(obj.BoundSegment.X, obj.BoundSegment.Y);
            double distance = (objectPos - pos).Length;

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
		SKPoint pos = e.Location;
		double x = pos.X;
		double y = pos.Y;
		Vector2 posVec = new((float)x, (float)y);

		switch (e.ActionType)
		{
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
			case SKTouchAction.Moved:
				if (draggingObject != null)
				{
					snappedObject = GetClosestTrackObject(pos, draggingObject);
					draggingObject.MoveTo(posVec.X, posVec.Y);

					draggingObject.UnsnapStart();
					draggingObject.UnsnapEnd();
				}

				RedrawCanvas();
				break;
			case SKTouchAction.Released:
            case SKTouchAction.Exited:
            case SKTouchAction.Cancelled:
				if (draggingObject != null && !IsWithinEditorFrame(x, y))
				{
					draggingObject.RemoveFrom(loadedProject.Track);
					objects.Remove(draggingObject);
				}
				else if (draggingObject != null && snappedObject != null)
				{
					Vector2? snapLocation = GetSnapLocation(draggingObject, snappedObject, out bool isStartCloser, out bool isDragStartCloser);
					if (snapLocation != null)
					{
						draggingObject.MoveTo(snapLocation?.X ?? 0, snapLocation?.Y ?? 0);

						if (isStartCloser)
							snappedObject.SnapToStart(draggingObject.BoundSegment);
						else
							snappedObject.SnapToEnd(draggingObject.BoundSegment);

						if (!isDragStartCloser)
							draggingObject.SnapToStart(snappedObject.BoundSegment);
						else
							draggingObject.SnapToEnd(snappedObject.BoundSegment);
					}
                }

				draggingObject = null;
				snappedObject = null;
				actionHandler.AddWaypoint();
				RedrawCanvas();
				break;
        }

		e.Handled = true;
	}

	private void RedrawCanvas()
	{
		EditorCanvas.InvalidateSurface();
	}

	private void OnEditorPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
	{
		Console.WriteLine(e.Scale);
	}

    private void OnPaintEditorCanvas(object sender, SKPaintSurfaceEventArgs e)
    {
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        SKCanvas canvas = e.Surface.Canvas;
		canvas.Clear();

		foreach (TrackObject obj in objects)
		{
			string resourceID = obj.BoundPiece.Image;
            using Stream? stream = assembly.GetManifestResourceStream(resourceID);

            if (stream != null)
            {
                SKBitmap bmp = SKBitmap.Decode(stream);
				Vector2 size = obj.BoundSegment.Size;
				Vector2 pos = -size / 2;

				SKRect dest = new(pos.X, pos.Y, pos.X + size.X, pos.Y + size.Y);
				canvas.Translate(obj.BoundSegment.X, obj.BoundSegment.Y);
				canvas.RotateDegrees(obj.BoundSegment.Rotation);
                canvas.DrawBitmap(bmp, dest);

				if (draggingObject != null && draggingObject != obj)
				{
                    Vector2 startOffset = obj.BoundSegment.StartSnapOffset;
                    Vector2 endOffset = obj.BoundSegment.EndSnapOffset;

                    if (snappedObject == obj)
                    {
						if (obj.BoundSegment.SnappedStartSegment == null)
							canvas.DrawCircle(startOffset.X * 2, startOffset.Y * 2, 30, SelectShadow);
						if (obj.BoundSegment.SnappedEndSegment == null)
							canvas.DrawCircle(endOffset.X * 2, endOffset.Y * 2, 30, SelectShadow);
                    }

					if (obj.BoundSegment.SnappedStartSegment == null)
						canvas.DrawCircle(startOffset.X, startOffset.Y, 10, SnapColor);
					if (obj.BoundSegment.SnappedEndSegment == null)
                        canvas.DrawCircle(endOffset.X, endOffset.Y, 10, SnapColor);
                }

				if (selectedObject == obj || draggingObject == obj)
					canvas.DrawCircle(0, 0, obj.BoundSegment.Size.X / 2, SelectShadow);

				canvas.ResetMatrix();
            }

            if (snappedObject != null && draggingObject != null)
            {
				Vector2? snapLocation = GetSnapLocation(draggingObject, snappedObject, out _, out _);

				if (snapLocation != null)
				{
                    canvas.ResetMatrix();
                    canvas.DrawCircle(snapLocation?.X ?? 0, snapLocation?.Y ?? 0, 25, SnapColor);
                }
            }
        }
    }
}