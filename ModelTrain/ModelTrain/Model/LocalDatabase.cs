using ModelTrain.Model.Pieces;
using ModelTrain.Model.Settings;
using ModelTrain.Model.Track;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace ModelTrain.Model
{
    public class LocalDatabase : ILocalDatabase
    {
        private readonly Dictionary<string, SettingBase> userSettings = new();

        public ObservableCollection<PieceBase> Hotbar { get; set; }

        public LocalDatabase()
        {
            Hotbar = new();

            foreach (SegmentType type in Enum.GetValues(typeof(SegmentType)))
            {
                Hotbar.Add(new(type));
            }

            // TODO: load settings, properly fill hotbar
        }

        private static string EncodeEmail(string email)
        {
            // Used for separating different profiles based on user email
            // without putting the email in plain text into the file
            byte[] hash = SHA1.HashData(Encoding.UTF8.GetBytes(email));
            return Convert.ToBase64String(hash);
        }

        public bool SaveToFile()
        {
            // TODO: save data to file (toml?)

            try
            {
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool LoadFromFile()
        {
            // TODO: load data from file (toml?)

            try
            {
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool SetUser(string email)
        {
            // TODO: update data depending on which user is connected

            try
            {
                string user = EncodeEmail(email);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public T GetSetting<T>(string setting)
        {
            SettingBase userSetting = userSettings[setting];
            return userSetting.GetValue<T>();
        }

        public void SetSetting<T>(string setting, T value)
        {
            SettingBase userSetting = userSettings[setting];
            userSetting.SetValue(value);
        }
    }
}
