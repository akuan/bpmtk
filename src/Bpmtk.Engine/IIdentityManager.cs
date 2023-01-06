using Bpmtk.Engine.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bpmtk.Engine
{
    public interface IIdentityManager
    {
        IList<User> GetUsers(params string[] userIds);

        IList<Group> GetGroups(params string[] groupIds);

        void CreateUser(User user);

        User FindUserById(string userId);

        void CreateGroup(Group group);

        Group FindGroupById(string groupId);

        void UpdateGroup(Group group);

        void UpdateUser(User user);

        Task DeleteGroupAsync(Group group);

        Task DeleteUserAsync(User user);
    }
}
