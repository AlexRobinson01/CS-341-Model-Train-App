namespace ModelTrain.Services
{
    /*
     * Description: A static method to change the current theme of the app
     * Author: Alex Robinson
     * Last updated: 12/10/2024
     */
    internal static class LightDarkMode
    {
        /// <summary>
        /// Updates the theme of the app
        /// </summary>
        /// <param name="theme">The theme to apply to the app</param>
        public static void Switch(string theme)
        {
            if (theme == "Dark")
                Application.Current.UserAppTheme = AppTheme.Dark;
            else
                Application.Current.UserAppTheme = AppTheme.Light;
        }
    }
}
