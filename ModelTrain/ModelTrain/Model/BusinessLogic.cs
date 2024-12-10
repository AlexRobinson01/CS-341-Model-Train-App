using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ModelTrain.Model
{
    public class BusinessLogic : IBusinessLogic
    {
        private IDatabase Database { get; set; }
        // Static instance of BusinessLogic
        private static BusinessLogic _instance;
        // Lock object for thread safety
        private static readonly object _lock = new object();
        // This variable will hold the signed in users email upon login
        private String email = "";

        public BusinessLogic()
        {
            Database = new Database();
        }

        /// <summary>
        /// Public method to get the single instance of BusinessLogic (allows for locally stored email upon login)
        /// </summary>
        public static BusinessLogic Instance
        {
            get
            {
                // Use double-checked locking to ensure thread safety
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BusinessLogic();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Get user with email
        /// </summary>
        /// <returns>
        /// Respective user
        /// </returns>
        public User GetUserFromEmail()
        {
            //Grab the user from their email
            User userToGet = Database.GetUser(this.email);
            // If user exists, return them
            if (userToGet != null)
            {
                return userToGet;
            }
            else
            {
                return null; // Throw exception
            }
        }

        /// <summary>
        /// Saves a project by updating the existing record in the database
        /// with the same ProjectID.
        /// </summary>
        /// <param name="project">The project to save, identified by its ProjectID.</param>
        /// <returns>
        /// True if the project is successfully saved; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when the project does not have a valid ProjectID.</exception>
        /// <summary>
        /// Saves a project by updating the existing record in the database with the same ProjectID.
        /// </summary>
        /// <param name="project">The project to save.</param>
        /// <returns>True if the project is successfully saved; false otherwise.</returns>
        public async Task<bool> SaveProject(PersonalProject project)
        {
            if (string.IsNullOrEmpty(project.ProjectID))
                throw new ArgumentException("ProjectID is required to save the project.");

            try
            {
                Console.WriteLine($"Saving project with ProjectID={project.ProjectID}");
                bool updated = await Database.UpdateProject(project);

                if (!updated)
                {
                    Console.WriteLine("Failed to update project. Project may not exist.");
                }

                return updated;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving project: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Make sure login input exists in the database and is correct
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>
        /// True if login is valid, false otherwise
        /// </returns>
        public async Task<bool> ValidateLoginInput(String email, String password)
        {
            // Check if email is not in database
            bool emailDoesNotExist = await Database.IsNewEmail(email);
            // Check if password is correct
            bool correctPassword = await Database.IsCorrectPassword(email, password);

            if (emailDoesNotExist || !correctPassword)  // If no email match or incorrect password return false
            {
                return false;
            }
            // Locally store email of signed in user for other functionality
            this.email = email;
            return true;
        }

        /// <summary>
        /// Create a new user account
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>
        /// True if account is created, false otherwise
        /// </returns>
        public async Task<bool> CreateAccount(String firstName, String lastName, String email, String password)
        {
            bool uniqueEmail = await Database.IsNewEmail(email);        // Check if database doesn't have this email
            if (uniqueEmail)                                            // If so, create the account
            {
                await Database.CreateAccount(firstName, lastName, email, password);
                return true;
            }
            return false;

        }

        /// <summary>
        /// Takes a project Id and deletes the project from the users profile and the projects db table
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>
        /// True if deleted, false otherwise
        /// </returns>
        public async Task<bool> DeleteProjectById(String projectId)
        {
            // If project is deleted from both tables, return true
            if (await Database.DeletePersonalProject(projectId) && await Database.RemoveProjectFromUsersAsync(projectId))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Fetches a list of user projects ids
        /// </summary>
        /// <returns>
        /// List of ids of users projects
        /// </returns>
        public async Task<List<Guid>> GetUserProjects()
        {
            // Create a new list of Guids to hold the users project ids
            List<Guid> userProjects = new List<Guid>();
            // Fetch list from the db and return
            userProjects = await Database.GetUserProjectIdsAsync(this.email);
            return userProjects;
        }

        /// <summary>
        /// Fetches a list of user projects
        /// </summary>
        /// <param name="projectIds"></param>
        /// <returns>
        /// List of users projects
        /// </returns>
        public async Task<List<PersonalProject>> GetProjectsByIds(List<Guid> projectIds)
        {
            // Create a new list of personal projects
            List<PersonalProject> userProjects = new List<PersonalProject>();
            // Fetch list of personal projects from the db and return
            userProjects = await Database.GetProjectsByIdsAsync(projectIds);
            return userProjects;
        }

        /// <summary>
        /// Creates a unique Guid for a new project
        /// </summary>
        /// <param name="maxRetries"></param>
        /// <returns>
        /// String of new guid
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<string> GetUniqueGuid(int maxRetries = 10)
        {
            // Create new Guid
            Guid newGuid = Guid.NewGuid();
            // Set try count to zero
            int retryCount = 0;

            // If projectid is unique in the database, return
            while (!(await Database.IsGuidUnique(newGuid)))
            {
                // If not unique, try again and increment count
                retryCount++;
                // If reaches max attempts, relay that it cannot generate unique GUID
                if (retryCount > maxRetries)
                {
                    throw new InvalidOperationException("Failed to generate a unique GUID after multiple attempts.");
                }

                newGuid = Guid.NewGuid();
            }

            return newGuid.ToString();
        }

        /// <summary>
        /// Adds a new project to the users profile and the projects db table
        /// </summary>
        /// <param name="newProject"></param>
        /// <returns>
        /// True if added to tables, false otherwise
        /// </returns>
        public async Task<bool> AddProjectToDB(PersonalProject newProject)
        {
            // Add new project to both db tables, if no errors, return true
            if (await Database.AddProjectToProjects(this.email, newProject) && await Database.AddProjectToUser(this.email, newProject.ProjectID))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if password is correct
        /// </summary>
        /// <param name="password"></param>
        /// <returns>True if correct password, false otherwise</returns>
        public async Task<bool> IsCorrectPassword(String password)
        {
            // Check if password is correct
            bool correctPassword = await Database.IsCorrectPassword(this.email, password);

            if (!correctPassword)  // If incorrect password return false
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="password"></param>
        /// <returns>True if password changed, false otherwise</returns>
        public async Task<bool> ChangePassword(String password)
        {
             return await Database.ChangePassword(this.email, password);

        }
    }
}
