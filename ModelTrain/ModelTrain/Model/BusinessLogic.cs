namespace ModelTrain.Model
{
    public class BusinessLogic : IBusinessLogic
    {
        private IDatabase Database { get; set; }

        public BusinessLogic()
        {
            Database = new Database();
        }

        // Get user with email
        public User GetUserFromEmail(String email)
        {
            User userToGet = Database.GetUser(email);
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

    }
}
