using SkiaSharp;
using SkiaSharp.Views.Maui;
using ModelTrain.Model.Track;
using ModelTrain.Model.Pieces;
using ModelTrain.Services;

namespace ModelTrain.Screens.Components
{
    public partial class PieceImage : Grid
    {
        // A light gray blur behind pieces to make them more visible on dark backgrounds
        private static readonly SKPaint PieceBackdrop = new()
        {
            // Recolor all pixels to a light gray while keeping their alpha channel
            ColorFilter = SKColorFilter.CreateLighting(
                SKColor.Parse("#FF000000"), SKColor.Parse("#FFD3D3D3")),
            // Apply a Gaussian blur
            ImageFilter = SKImageFilter.CreateBlur(5, 5)
        };

        public Piece? PieceOverride = null;

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

            // Prepare the canvas to be drawn to
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();

            // Uses the ClassId from earlier to determine image data
            Piece piece = PieceOverride ?? new(segmentType);
            string resourceID = piece.Image;
            SKBitmap? bmp = ImageFileDecoder.GetBitmapFromFile(resourceID);

            if (bmp != null)
            {
                SKRect dest = new(0, 0, e.Info.Width, e.Info.Height);

                // Adjust the canvas with the piece's image info
                canvas.Translate(dest.Width / 2, dest.Height / 2);
                canvas.RotateDegrees(piece.ImageRotation);
                canvas.Scale(piece.ImageScale);
                canvas.Translate(-dest.Width / 2, -dest.Height / 2);

                canvas.Translate(piece.ImageOffset.X, piece.ImageOffset.Y);

                // Draw a light gray shadow behind the bitmap
                // so it's easier to see on different backgrounds
                canvas.DrawBitmap(bmp, dest, PieceBackdrop);

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
}