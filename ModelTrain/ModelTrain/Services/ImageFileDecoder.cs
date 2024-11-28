﻿using SkiaSharp;
using System.Reflection;

namespace ModelTrain.Services
{
    /**
     * Description: A static method to retrieve a bitmap from a given file path
     * Author: Alex Robinson
     * Last updated: 11/27/2024
     */
    public static class ImageFileDecoder
    {
        public static SKBitmap? GetBitmapFromFile(object sender, string path)
        {
            Stream? stream;

            if (File.Exists(path))
                stream = File.OpenRead(path);
            else
            {
                // Retrieve an embedded file from the path
                Assembly assembly = Assembly.GetExecutingAssembly();
                stream = assembly.GetManifestResourceStream(path);
            }

            SKBitmap? bmp = stream == null ? null : SKBitmap.Decode(stream);
            stream?.Dispose();

            return bmp;
        }
    }
}
