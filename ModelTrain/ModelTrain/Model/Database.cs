using System.Text.Json;
using Npgsql;

namespace ModelTrain.Model
{
    public class Database : IDatabase
    {

        private static System.Random rng = new();
        private String connString;

        JsonSerializerOptions options;

        public Database()
        {
            connString = GetConnectionString();
            options = new JsonSerializerOptions { WriteIndented = true };
        }

        // Builds a ConnectionString, which is used to connect to the database
        static String GetConnectionString()
        {
            var connStringBuilder = new NpgsqlConnectionStringBuilder();
            connStringBuilder.Host = "potent-wasp-14353.5xj.gcp-us-central1.cockroachlabs.cloud";
            connStringBuilder.Port = 26257;
            connStringBuilder.SslMode = SslMode.Prefer;
            connStringBuilder.Username = FetchUsername();
            connStringBuilder.Password = FetchPassword();
            connStringBuilder.Database = "";
            connStringBuilder.ApplicationName = "whatever";

            return connStringBuilder.ConnectionString;
        }

        // Fetches the password
        static String FetchPassword()
        {
            return "";
        }

        // Fetches the username
        static String FetchUsername()
        {
            return "";
        }
    }
}
