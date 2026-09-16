using Microsoft.AspNetCore.Identity;
using NewGenBridge.UoW;
using NewGenBridge.Models;

namespace NewGenBridge.Services
{
    public class UserService
    {
        UnitOfWork _unitOfWork;
        UserManager<NPUser> _userManager;
        RoleManager<IdentityRole> _roleManager;

        public UserService(UnitOfWork unitOfWork, UserManager<NPUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //ToDo: изменить roleName на enum или что-то
        public async Task<List<NPUser>> GetAllUsersInRoleAsync(string roleName)
        {
            IList<NPUser> usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            return usersInRole.ToList();
        }

        public string GetUserFullNameById(string userId)
        {
            NPUser user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            return user.FirstName + " " + user.LastName;
        }

        public async Task<NPUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> UpdateUser(NPUser user)
        {
            try
            {
                _unitOfWork.Users.Update(user);
                await _unitOfWork.Complete();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
