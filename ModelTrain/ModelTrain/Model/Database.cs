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
            connStringBuilder.Database = "modeltrain";
            connStringBuilder.ApplicationName = "whatever";

            return connStringBuilder.ConnectionString;
        }

        // Fetches the password
        static String FetchPassword()
        {
            return "F4J7LoSMe4qD8oKjgpEUsQ";
        }

        // Fetches the username
        static String FetchUsername()
        {
            return "teamf";
        }

        public User GetUser(string email)
        {
            User userToGet = null;
            var conn = new NpgsqlConnection(connString);                                                                      // Connection to the database
            conn.Open();                                                                                                      // Open the connection

            using var cmd = new NpgsqlCommand("SELECT email, firstname, lastname FROM users WHERE email = @email", conn);            // Create the sql line text 
            cmd.Parameters.AddWithValue("email", email);                                                                            // Add id as a parameter to the sql line

            using var reader = cmd.ExecuteReader();                                                                           // Used for SELECT statement, returns a forward-only traversable object
            if (reader.Read())
            {                                                                                                                   // There should be only one row, so we don't need a while loop TODO: Sanity check
                email = reader.GetString(0);                                                                                    // Get User Email
                String firstName = reader.GetString(1);                                                                         // Get User First Name
                String lastName = reader.GetString(2);                                                                          // Get User Last Name
                userToGet = new(email, firstName, lastName);                                                                  // Create User with Parameters
            }
            conn.Close();                                                                                                     // Close the connection
            return userToGet;
        }
    }
}
