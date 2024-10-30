namespace ModelTrain.Services
{
    internal static class DeviceOrientation
    {
        public static void SetLandscape()
        {
#if ANDROID
            if (Platform.CurrentActivity != null)
                Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
#endif
        }

        public static void SetPortrait()
        {
#if ANDROID
            if (Platform.CurrentActivity != null)
                Platform.CurrentActivity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
#endif
        }
    }
}
