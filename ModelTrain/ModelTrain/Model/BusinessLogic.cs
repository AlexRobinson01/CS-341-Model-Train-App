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
        private String email = "";

        public BusinessLogic()
        {
            Database = new Database();
        }

        // Public method to get the single instance of BusinessLogic
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
            User userToGet = Database.GetUser(this.email);
            if (userToGet != null)
            {
                return userToGet;
            }
            else
            {
                return null; // Throw exception
            }
        }

        public bool SaveProject(PersonalProject project)
        {
            // TODO: save project to database; allow for shared projects too
            return new Random().Next() % 2 == 0;
        }

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
            this.email = email;
            return true;
        }

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

        public async Task<bool> DeleteProjectById(String projectId)
        {
            if (await Database.DeletePersonalProject(projectId) && await Database.RemoveProjectFromUsersAsync(projectId))
            {
                return true;
            }
            return false;
        }

        public async Task<List<Guid>> GetUserProjects()
        {
            List<Guid> userProjects = new List<Guid>();
            userProjects = await Database.GetUserProjectIdsAsync(this.email);
            return userProjects;
        }

        public async Task<List<PersonalProject>> GetProjectsByIds(List<Guid> projectIds)
        {
            List<PersonalProject> userProjects = new List<PersonalProject>();
            userProjects = await Database.GetProjectsByIdsAsync(projectIds);
            return userProjects;
        }

        public async Task<string> GetUniqueGuid(int maxRetries = 10)
        {
            Guid newGuid = Guid.NewGuid();
            int retryCount = 0;

            while (!(await Database.IsGuidUnique(newGuid)))
            {
                retryCount++;
                if (retryCount > maxRetries)
                {
                    throw new InvalidOperationException("Failed to generate a unique GUID after multiple attempts.");
                }

                newGuid = Guid.NewGuid();
            }

            return newGuid.ToString();
        }

        public async Task<bool> AddProjectToDB(PersonalProject newProject)
        {
            if (await Database.AddProjectToProjects(this.email, newProject) && await Database.AddProjectToUser(this.email, newProject.ProjectID))
            {
                return true;
            }
            return false;
        }

    }
}
