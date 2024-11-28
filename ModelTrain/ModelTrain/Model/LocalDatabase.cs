using ModelTrain.Model.Pieces;
using ModelTrain.Model.Settings;
using System.Security.Cryptography;
using System.Text;

namespace ModelTrain.Model
{
    /**
     * Description: An implementation of ILocalDatabase with a settings file and default hotbar
     * Author: Alex Robinson
     * Last updated: 11/24/2024
     */
    public class LocalDatabase : ILocalDatabase
    {
        private readonly Dictionary<string, SettingBase> userSettings = new();

        public PieceList Hotbar { get; set; }

        public LocalDatabase()
        {
            Hotbar = PieceInfo.GetDefaultPieces();

            // TODO: load settings, properly fill hotbar
        }

        /// <summary>
        /// Used for separating different profiles based on user email
        /// without putting the email in plain text into the file
        /// (May switch to bcrypt or something if necessary, currently using SHA1 hash)
        /// </summary>
        /// <param name="email">The email to hash</param>
        /// <returns>The SHA1 hash of the given email</returns>
        private static string HashEmail(string email)
        {
            byte[] hash = SHA1.HashData(Encoding.UTF8.GetBytes(email));
            return Convert.ToBase64String(hash);
        }

        public bool SaveToFile()
        {
            // TODO: save data to file (json)

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
            // TODO: load data from file (json)

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
                string user = HashEmail(email);

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
