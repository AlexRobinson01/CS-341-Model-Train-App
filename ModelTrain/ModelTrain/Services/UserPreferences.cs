using ModelTrain.Model;

namespace ModelTrain.Services
{
    /*
     * Description: A wrapper for the Preferences class that allows per-user preferences
     * rather than making them all global
     * Author: Alex Robinson
     * Last updated: 12/10/2024
     */
    internal static class UserPreferences
    {
        private static string userName = "";

        /// <summary>
        /// Updates the user whose preferences should be used by this class
        /// </summary>
        /// <param name="user">The user whose preferences should be used by this class</param>
        public static void UpdateUser(User? user)
        {
            userName = user?.FullName ?? "";
            LightDarkMode.Switch(Get("UserTheme", "Light") ?? "");
        }

        /// <summary>
		/// Gets the value for a given key, or the default specified if the key does not exist.
		/// </summary>
		/// <param name="key">The key to retrieve the value for.</param>
		/// <param name="defaultValue">The default value to return
        /// when no existing value for <paramref name="key"/> exists.</param>
		/// <returns>Value for the given key, or the value
        /// in <paramref name="defaultValue"/> if it does not exist.</returns>
        public static string? Get(string key, string? defaultValue)
            => Preferences.Get($"{userName}_{key}", defaultValue);

        /// <summary>
		/// Gets the value for a given key, or the default specified if the key does not exist.
		/// </summary>
		/// <param name="key">The key to retrieve the value for.</param>
		/// <param name="defaultValue">The default value to return
        /// when no existing value for <paramref name="key"/> exists.</param>
		/// <returns>Value for the given key, or the value
        /// in <paramref name="defaultValue"/> if it does not exist.</returns>
        public static float Get(string key, float defaultValue)
            => Preferences.Get($"{userName}_{key}", defaultValue);

        /// <summary>
		/// Sets a value for a given key.
		/// </summary>
		/// <param name="key">The key to set the value for.</param>
		/// <param name="value">Value to set</param>
        public static void Set(string key, string? value)
            => Preferences.Set($"{userName}_{key}", value);

        /// <summary>
		/// Sets a value for a given key
		/// </summary>
		/// <param name="key">The key to set the value for</param>
		/// <param name="value">Value to set</param>
        public static void Set(string key, float value)
            => Preferences.Set($"{userName}_{key}", value);

        /// <summary>
		/// Checks the existence of a given key
		/// </summary>
		/// <param name="key">The key to check</param>
		/// <returns>Whether the key exists in the preferences</returns>
        public static bool ContainsKey(string key)
            => Preferences.ContainsKey($"{userName}_{key}");
    }
}
