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

        // Public method to get the single instance of BusinessLogic (allows for locally stored email upon login)
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

        // Get user with email
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

        // Save changes made to a project
        public bool SaveProject(PersonalProject project)
        {
            // TODO: save project to database; allow for shared projects too
            return new Random().Next() % 2 == 0;
        }

        // Make sure login input exists in the database and is correct
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

        // Create a new user account
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

        // Takes a project Id and deletes the project from the users profile and the projects db table
        public async Task<bool> DeleteProjectById(String projectId)
        {
            // If project is deleted from both tables, return true
            if (await Database.DeletePersonalProject(projectId) && await Database.RemoveProjectFromUsersAsync(projectId))
            {
                return true;
            }
            return false;
        }

        // Fetches a list of user projects ids
        public async Task<List<Guid>> GetUserProjects()
        {
            // Create a new list of Guids to hold the users project ids
            List<Guid> userProjects = new List<Guid>();
            // Fetch list from the db and return
            userProjects = await Database.GetUserProjectIdsAsync(this.email);
            return userProjects;
        }

        // Fetches a list of user projects
        public async Task<List<PersonalProject>> GetProjectsByIds(List<Guid> projectIds)
        {
            // Create a new list of personal projects
            List<PersonalProject> userProjects = new List<PersonalProject>();
            // Fetch list of personal projects from the db and return
            userProjects = await Database.GetProjectsByIdsAsync(projectIds);
            return userProjects;
        }

        // Creates a unique Guid for a new project
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

        // Adds a new project to the users profile and the projects db table
        public async Task<bool> AddProjectToDB(PersonalProject newProject)
        {
            // Add new project to both db tables, if no errors, return true
            if (await Database.AddProjectToProjects(this.email, newProject) && await Database.AddProjectToUser(this.email, newProject.ProjectID))
            {
                return true;
            }
            return false;
        }

    }
}
