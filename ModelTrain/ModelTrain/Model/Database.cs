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

        public async Task<bool> IsNewEmail(string email)
        {
            // SQL query to check if email exists
            string query = "SELECT COUNT(*) FROM users WHERE email = @Email";

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    await conn.OpenAsync();

                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        // Execute the command and check if any records were found
                        var count = (long)await command.ExecuteScalarAsync();
                        return count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }

        public async Task CreateAccount(string firstName, string lastName, string email, string password)
        {
            // SQL statement to insert a new user
            string query = "INSERT INTO users (firstname, lastname, email, password) VALUES (@FirstName, @LastName, @Email, @Password)";

            try
            {
                using (var connection = new NpgsqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Set up parameters
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);

                        // Execute the insert command
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                throw;
            }
        }
    }
}
