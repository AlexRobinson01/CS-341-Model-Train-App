using ModelTrain.Model.Pieces;

namespace ModelTrain.Model
{
    /**
     * Description: An interface for a local database (settings file) which manages hotbar
     * and other user settings
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public interface ILocalDatabase
    {
        // The loaded list of pieces a user has on their hotbar in the Track Editor
        public PieceList Hotbar { get; set; }
        
        /// <summary>
        /// Saves the current local settings out to a settings file
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public bool SaveToFile();
        
        /// <summary>
        /// Loads the current local settings from a settings file
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public bool LoadFromFile();
        
        /// <summary>
        /// Switches which user has their settings currently loaded by their email
        /// </summary>
        /// <param name="email">The email of the user to switch to</param>
        /// <returns>Whether the operation succeeded</returns>
        public bool SetUser(string email);

        /// <summary>
        /// Retrieves a saved setting's value by its name
        /// </summary>
        /// <typeparam name="T">The Type of the setting to retrieve</typeparam>
        /// <param name="setting">The name of the setting to retrieve</param>
        /// <returns>The retrieved setting's value</returns>
        public T GetSetting<T>(string setting);
        
        /// <summary>
        /// Assigns a setting's value denoted by its name to a new value
        /// </summary>
        /// <typeparam name="T">The Type of the setting to assign</typeparam>
        /// <param name="setting">The name of the setting to assign</param>
        /// <param name="value">The value to assign to the setting</param>
        public void SetSetting<T>(string setting, T value);
    }
}
