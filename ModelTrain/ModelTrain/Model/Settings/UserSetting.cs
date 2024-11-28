namespace ModelTrain.Model.Settings
{
    /**
     * Description: An implementation of SettingBase that allows a generic type to be used
     * to simplify storing and using settings
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public class UserSetting<T> : SettingBase
    {
        public T Value;

        /// <summary>
        /// UserSetting constructor - Creates a new Setting with a given default value
        /// </summary>
        /// <param name="value"></param>
        public UserSetting(T value) => Value = value;

        public override K GetValue<K>()
        {
            // These should be the same type anyway, but C# sees T and K as different generics
            return (K)Convert.ChangeType(Value, typeof(K))!;
        }

        public override void SetValue<K>(K value)
        {
            // More type conversion even though it should be the same type in practice!
            Value = (T)Convert.ChangeType(value, typeof(T))!;
        }
    }
}
