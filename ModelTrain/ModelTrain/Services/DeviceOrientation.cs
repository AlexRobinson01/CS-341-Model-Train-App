namespace ModelTrain.Services
{
    /**
     * Description: Static methods to change the requested orientation for this application
     * NOTE: Not yet implemented for iOS devices
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public static class DeviceOrientation
    {
        /// <summary>
        /// Sets the requested orientation of this application to Landscape
        /// </summary>
        public static void SetLandscape()
        {
#if ANDROID
            if (Platform.CurrentActivity != null)
                Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
#endif
        }

        /// <summary>
        /// Sets the requested orientation of this application to Portrait
        /// </summary>
        public static void SetPortrait()
        {
#if ANDROID
            if (Platform.CurrentActivity != null)
                Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
#endif
        }
    }
}
