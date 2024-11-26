using SkiaSharp;
using System.Reflection;
using SkiaSharp.Views.Maui;
using ModelTrain.Model.Track;
using ModelTrain.Model.Pieces;

namespace ModelTrain.Screens.Components;

public partial class PieceImage : Grid
{
	public PieceImage()
	{
		InitializeComponent();
	}

    private void OnPaintPieceImage(object sender, SKPaintSurfaceEventArgs e)
    {
        // Ensure the ClassId of this control maps to a valid SegmentType
        if (!Enum.TryParse(typeof(SegmentType), PieceImageGrid.ClassId, out object? type))
            return;
        if (type is not SegmentType segmentType)
            return;

        // Will be used for retrieving an embedded image for this piece
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        // Prepare the canvas to be drawn to
        SKCanvas canvas = e.Surface.Canvas;
        canvas.Clear();

        // Uses the ClassId from earlier to determine image data
        Piece piece = new(segmentType);
        string resourceID = piece.Image;
        using Stream? stream = assembly.GetManifestResourceStream(resourceID);

        if (stream != null)
        {
            // Prepare a bitmap to be drawn to the canvas
            SKBitmap bmp = SKBitmap.Decode(stream);
            SKRect dest = new(0, 0, e.Info.Width, e.Info.Height);
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