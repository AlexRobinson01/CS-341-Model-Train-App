﻿namespace ModelTrain.Model
{
    public interface IBusinessLogic
    {
        public User GetUserFromEmail(String email);
        public bool SaveProject(PersonalProject project);
        public bool ValidateLoginInput(String email, String password);
        public bool CreateAccount(String firstName, String lastName, String email, String password);
    }
}
