namespace ModelTrain.Model
{
    public interface IBusinessLogic
    {
        public User GetUserFromEmail();
        public bool SaveProject(PersonalProject project);
        public Task<bool> ValidateLoginInput(String email, String password);
        public Task<bool> CreateAccount(String firstName, String lastName, String email, String password);
    }
}
