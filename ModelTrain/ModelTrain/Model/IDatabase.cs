using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelTrain.Model
{
    public interface IDatabase
    {
        public User GetUser(string email);
        public Task<bool> IsNewEmail(string email);
        public Task CreateAccount(string firstName, string lastName, string email, string password);
    }
}
