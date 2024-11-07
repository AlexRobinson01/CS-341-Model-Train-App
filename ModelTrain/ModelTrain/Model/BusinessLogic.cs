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

        public bool ValidateLoginInput(String email, String password)
        {
            return true;
        }

    }
}
