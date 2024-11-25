namespace ModelTrain.Model.Settings
{
    /**
     * Description: An abstract class for settings that lets inheriting classes be stored in a list
     * despite having generic types
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public abstract class SettingBase
    {
        /// <summary>
        /// Gets the value associated with this Setting
        /// </summary>
        /// <typeparam name="T">The type this Setting is assigned with</typeparam>
        /// <returns>The value to be retrieved</returns>
        public abstract T GetValue<T>();

        /// <summary>
        /// Sets the value associated with this Setting
        /// </summary>
        /// <typeparam name="T">The type this Setting is assigned with</typeparam>
        /// <param name="value">The value to be set</param>
        public abstract void SetValue<T>(T value);
    }
}
