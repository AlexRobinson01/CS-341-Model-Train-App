namespace ModelTrain.Model.Settings
{
    public class UserSetting<T> : SettingBase
    {
        public T Value;

        public UserSetting(T value) => Value = value;

        public override K GetValue<K>()
        {
            return (K)Convert.ChangeType(Value, typeof(K))!;
        }

        public override void SetValue<K>(K value)
        {
            Value = (T)Convert.ChangeType(value, typeof(T))!;
        }
    }
}
