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
        public Task<bool> IsCorrectPassword(string email, string password);
        public Task<bool> DeletePersonalProject(string projectId);
        public Task<bool> RemoveProjectFromUsersAsync(string projectId);
        public Task<List<Guid>> GetUserProjectIdsAsync(string email);
        public Task<List<PersonalProject>> GetProjectsByIdsAsync(List<Guid> projectIds);
        public Task<bool> IsGuidUnique(Guid id);
        public Task<bool> AddProjectToUser(string email, string projectId);
        public Task<bool> AddProjectToProjects(string email, PersonalProject newProject);

        Task<bool> UpdateProject(PersonalProject project);
    }
}
