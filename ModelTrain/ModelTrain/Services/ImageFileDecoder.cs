using SkiaSharp;
using System.Reflection;

namespace ModelTrain.Services
{
    /**
     * Description: A static method to retrieve a bitmap from a given file path
     * Author: Alex Robinson
     * Last updated: 12/5/2024
     */
    public static class ImageFileDecoder
    {
        /// <summary>
        /// Gets an SKBitmap from a file contained at the given path
        /// </summary>
        /// <param name="path">The path to the bitmap, either as an embedded resource
        /// or a file on the system</param>
        /// <returns></returns>
        public static SKBitmap? GetBitmapFromFile(string path)
        {
            Stream? stream = null;

            // Open the file or resource from the path
            if (File.Exists(path))
                stream = File.OpenRead(path);
            else if (!string.IsNullOrWhiteSpace(path))
            {
                // The path points to an embedded resource, attempt to retrieve it
                Assembly assembly = Assembly.GetExecutingAssembly();
                stream = assembly.GetManifestResourceStream(path);
            }

            // Get a bitmap from the file
            SKBitmap? bmp = null;
            if (stream != null)
                bmp = SKBitmap.Decode(stream);
            // Clean up
            stream?.Dispose();

            return bmp;
        }
    }
}
