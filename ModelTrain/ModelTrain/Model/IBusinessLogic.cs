namespace ModelTrain.Model
{
    public interface IBusinessLogic
    {
        public User GetUserFromEmail(string email);
        public bool SaveProject(PersonalProject project);
    }
}
