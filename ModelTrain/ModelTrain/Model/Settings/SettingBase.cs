namespace ModelTrain.Model.Settings
{
    public abstract class SettingBase
    {
        public abstract T GetValue<T>();
        public abstract void SetValue<T>(T value);
    }
}
