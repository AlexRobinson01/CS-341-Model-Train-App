using SkiaSharp;
using SkiaSharp.Views.Maui;
using ModelTrain.Model.Track;
using ModelTrain.Model.Pieces;
using ModelTrain.Services;

namespace ModelTrain.Screens.Components;

public partial class PieceImage : Grid
{
    public Piece? CurrentPiece { get; private set; }

	public PieceImage()
	{
		InitializeComponent();
        Redraw();
	}

    private void OnPaintPieceImage(object sender, SKPaintSurfaceEventArgs e)
    {
        // Ensure the ClassId of this control maps to a valid SegmentType
        if (!Enum.TryParse(typeof(SegmentType), PieceImageGrid.ClassId, out object? type))
            return;
        if (type is not SegmentType segmentType)
            return;

        // Prepare the canvas to be drawn to
        SKCanvas canvas = e.Surface.Canvas;
        canvas.Clear();

        // Uses the ClassId from earlier to determine image data
        CurrentPiece ??= new(segmentType);
        string resourceID = CurrentPiece.Image;
        SKBitmap? bmp = ImageFileDecoder.GetBitmapFromFile(resourceID);

        if (bmp != null)
        {
            SKRect dest = new(0, 0, e.Info.Width, e.Info.Height);

            // Adjust the canvas with the piece's image info
            canvas.RotateDegrees(CurrentPiece.ImageRotation);
            canvas.Scale(CurrentPiece.ImageScale);
            canvas.Translate(CurrentPiece.ImageOffset.X, CurrentPiece.ImageOffset.Y);
            // Draw the bitmap on the canvas
            canvas.DrawBitmap(bmp, dest);
        }
    }

    /// <summary>
    /// Marks this piece image as needing to be redrawn
    /// </summary>
    public void Redraw()
    {
        // Marks ImageCanvas as needing to be redrawn
        ImageCanvas.InvalidateSurface();
    }
}