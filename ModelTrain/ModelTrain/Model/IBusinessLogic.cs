namespace ModelTrain.Model
{
    public interface IBusinessLogic
    {
        public User GetUserFromEmail(string email);
        public bool SaveProject(PersonalProject project);
        public bool ValidateLoginInput(String email, String password);
    }
}
