using System.Data;
using System.Text.Json;
using Npgsql;
using NpgsqlTypes;

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

        public async Task<bool> IsCorrectPassword(string email, string password)
        {
            // SQL query to get the stored password for the provided email
            string query = "SELECT password FROM users WHERE email = @Email";

            try
            {
                using (var connection = new NpgsqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Add parameter
                        command.Parameters.AddWithValue("@Email", email);

                        // Execute query and retrieve the stored password
                        var result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            string storedPassword = (string)result;
                            // Check if the stored password matches the provided password
                            return storedPassword == password;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }

            // Return false if email not found or password doesn't match
            return false;
        }

        public async Task<bool> DeletePersonalProject(string projectId)
        {
            const string query = "DELETE FROM projects WHERE projectid = @ProjectId;";

            try
            {
                await using (var connection = new NpgsqlConnection(connString))
                {
                    await connection.OpenAsync();

                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Set the parameter type to DbType.Guid for UUID compatibility
                        command.Parameters.Add(new NpgsqlParameter("@ProjectId", DbType.Guid)
                        {
                            Value = Guid.Parse(projectId) // Convert the string to a Guid
                        });

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0; // True if a row was deleted
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while deleting the project: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> RemoveProjectFromUsersAsync(string projectId)
        {
            const string query = @"
        UPDATE users
        SET projects = array_remove(projects, @ProjectId)
        WHERE @ProjectId = ANY(projects);";

            try
            {
                await using (var connection = new NpgsqlConnection(connString))
                {
                    await connection.OpenAsync();

                    await using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Set the parameter type to DbType.Guid for UUID compatibility
                        command.Parameters.Add(new NpgsqlParameter("@ProjectId", DbType.Guid)
                        {
                            Value = Guid.Parse(projectId) // Convert the string to a Guid
                        });

                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0; // Returns true if any rows were updated
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while removing the project from users: {ex.Message}");
                throw;
            }
        }

        // Method to get the user's list of project IDs from the users table
        public async Task<List<Guid>> GetUserProjectIdsAsync(string email)
        {
            const string query = "SELECT projects FROM users WHERE email = @Email;";

            try
            {
                using (var connection = new NpgsqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.Add(new NpgsqlParameter("@Email", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = email });

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var projects = reader["projects"] as Guid[];
                                return projects?.ToList() ?? new List<Guid>();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching the user's projects: {ex.Message}");
                throw;
            }

            return new List<Guid>();
        }

        // Method to get project details based on the list of project IDs
        public async Task<List<PersonalProject>> GetProjectsByIdsAsync(List<Guid> projectIds)
        {
            var projects = new List<PersonalProject>();

            if (projectIds.Count == 0)
                return projects;

            var query = "SELECT projectid, projectname, datecreated FROM projects WHERE projectid = ANY(@ProjectIds);";

            try
            {
                using (var connection = new NpgsqlConnection(connString))
                {
                    await connection.OpenAsync();

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        // Use NpgsqlDbType.Array for PostgreSQL arrays
                        command.Parameters.Add(new NpgsqlParameter("@ProjectIds", NpgsqlDbType.Uuid | NpgsqlDbType.Array)
                        {
                            Value = projectIds.ToArray()
                        });

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var project = new PersonalProject
                                {
                                    ProjectID = reader.GetGuid(reader.GetOrdinal("projectid")).ToString(),
                                    ProjectName = reader.GetString(reader.GetOrdinal("projectname")),
                                    DateCreated = reader.GetDateTime(reader.GetOrdinal("datecreated")).ToString("MM/dd/yyyy"),
                                    Track = new Model.Track.TrackBase() // Fetch track data if necessary
                                };

                                projects.Add(project);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching the projects: {ex.Message}");
                throw;
            }

            return projects;
        }

        public async Task<bool> IsGuidUnique(Guid id)
        {
            // SQL query to check if project id exists
            string query = "SELECT COUNT(*) FROM projects WHERE projectid = @Id";

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    await conn.OpenAsync();

                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Id", id);

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

        public async Task<bool> AddProjectToUser(string email, string projectId)
        {
            try
            {
                // Convert the projectId string to a Guid
                Guid projectGuid = Guid.Parse(projectId);

                using var connection = new Npgsql.NpgsqlConnection(connString);
                await connection.OpenAsync();

                // SQL query to append the project ID to the `projects` array
                string query = @"
                    UPDATE users
                    SET projects = array_append(projects, @ProjectGuid)
                    WHERE email = @Email
                    RETURNING projects;
                    ";

                using var command = new Npgsql.NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProjectGuid", projectGuid);
                command.Parameters.AddWithValue("@Email", email);

                // Execute the query and check if any rows were updated
                var result = await command.ExecuteScalarAsync();

                // If the result is not null, the project was successfully added
                return result != null;
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid projectId format. Ensure it is a valid GUID string.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding project to user: {ex.Message}");
                return false;
            }
        }

        // Add the newly created project to the projects table in the database
        public async Task<bool> AddProjectToProjects(string email, PersonalProject newProject)
        {
            // Catch possible errors that may arrise
            try
            {
                // Establish a connection with the database
                using var connection = new Npgsql.NpgsqlConnection(connString);
                await connection.OpenAsync();

                // SQL query to insert a new project id, name, owner, date, and empty track fields
                string query = @"
                    INSERT INTO public.projects (projectid, projectname, projectowner, datecreated, trackdata)
                    VALUES (@ProjectID, @ProjectName, @ProjectOwner, @DateCreated, @TrackData);
                    ";

                using var command = new Npgsql.NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProjectID", new Guid(newProject.ProjectID)); // Ensure project ID is a GUID
                command.Parameters.AddWithValue("@ProjectName", newProject.ProjectName); // Take projectName from project and add to database row
                command.Parameters.AddWithValue("@ProjectOwner", email);    // Take locally stored email and set as projectOwner
                command.Parameters.AddWithValue("@DateCreated", newProject.DateCreated);    // This is set in a previous method as DateTime.Now()
                command.Parameters.AddWithValue("@TrackData", ""); // Set track data to an empty string

                // Execute the query
                int rowsAffected = await command.ExecuteNonQueryAsync();

                // Return true if rows were affected, this indicates the insertion was successful
                return rowsAffected > 0;
            }
            catch (Exception ex) // Throw an error otherwise
            {
                Console.WriteLine($"Error adding project to projects table: {ex.Message}");
                return false;
            }
        }


    }
}
