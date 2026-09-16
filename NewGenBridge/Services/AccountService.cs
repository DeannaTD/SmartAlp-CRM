using Microsoft.AspNetCore.Identity;
using NewGenBridge.Helpers;
using NewGenBridge.UoW;
using NewGenBridge.ViewModels;
using NewGenBridge.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NewGenBridge.Services
{
    public class AccountService
    {
        UserManager<NPUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        SignInManager<NPUser> _signInManager;
        UnitOfWork _unitOfWork;

        public AccountService(UserManager<NPUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<NPUser> signInManager, UnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        #region User Object Creation
        private NPUser CreateNewUserObject(StaffRegistrationViewModel model) => new()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = LabelCreator.CreateUserName(model.FirstName[0] + model.LastName),
            Email = model.Email ?? "",
            EmailConfirmed = true,
            DateOfBirth = model.DateOfBirth.ToDateTime(TimeOnly.MinValue),
            StatusId = UserStatus.Active,
            ParentName = "",
            PaymentAmount = 0,
            MentorDivisionId = model.Role == NPUR.Mentor ? model.DivisionId : null
        };

        private NPUser CreateNewUserObject(StudentRegistrationViewModel model) => new NPUser
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            UserName = LabelCreator.CreateUserName(model.FirstName[0] + model.LastName),
            PhoneNumber = model.PhoneNumber,
            StudentPhoneNumber = model.StudentPhoneNumber,
            DateOfBirth = model.DateOfBirth.ToDateTime(TimeOnly.MinValue),
            DateStarted = model.DateStarted.ToDateTime(TimeOnly.MinValue),
            StatusId = UserStatus.Active,
            ParentName = model.ParentName,
            PaymentAmount = model.PaymentAmount,
            GroupId = model.GroupId,
            Note = model.Note,
            CurrentLessonId = 2 //here the first id of lesson
        };

        #endregion

        #region Registration
        public async Task<(bool, string)> RegisterAsync(StaffRegistrationViewModel model)
        {
            bool isSuccessful = false;
            NPUser newUser = CreateNewUserObject(model);
            string userName = newUser.UserName;
            NPUser? duplicateUser = await _userManager.FindByNameAsync(userName);
            int accountNumber = 1;
            while (duplicateUser is not null)
            {
                userName = newUser.UserName;
                userName += (accountNumber++);
                duplicateUser = await _userManager.FindByNameAsync(userName);
            }
            newUser.UserName = userName;
            string password = "";
            if (newUser.UserName.Length >= 5) password = newUser.UserName;
            else password = newUser.UserName + "1234";
            IdentityResult result = await _userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, model.Role);
                //await _signInManager.SignInAsync(newUser, isPersistent: true);
                isSuccessful = true;
            }
            return (isSuccessful, newUser.UserName);
        }

        public async Task<(bool, string)> RegisterAsync(StudentRegistrationViewModel model)
        {
            bool isSuccessful = false;
            NPUser newUser = CreateNewUserObject(model);
            string userName = newUser.UserName;
            NPUser? duplicateUser = await _userManager.FindByNameAsync(userName);
            int accountNumber = 1;
            while (duplicateUser is not null)
            {
                userName = newUser.UserName;
                userName += (accountNumber++);
                duplicateUser = await _userManager.FindByNameAsync(userName);
            }
            newUser.UserName = userName;
            string password = "";
            if (newUser.UserName.Length >= 5) password = newUser.UserName;
            else password = newUser.UserName + "1234";
            IdentityResult result = await _userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Student");
                var lessons = await _unitOfWork.Lessons.GetCollectionAsync();
                if (lessons.Count == 0)
                    return (false, "");
                UserLesson userLesson = new UserLesson
                {
                    UserId = newUser.Id,
                    LessonId = lessons[0].Id,
                    IsOpened = true
                };
                await _unitOfWork.UserLessons.CreateAsync(userLesson);
                int numberOfChanges = await _unitOfWork.Complete();
                isSuccessful = numberOfChanges > 0;

            }
            return (isSuccessful, newUser.UserName);
        }
        #endregion

        #region Login and logout

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> SignPassword(LoginViewModel model)
        {
            bool lockoutOnFailure = false;
            Microsoft.AspNetCore.Identity.SignInResult result;
            bool loginByEmail = model.UserName.Contains('@');
            if (loginByEmail)
            {
                NPUser? user = await _userManager.FindByEmailAsync(model.UserName);
                if (user is null)
                {
                    return Microsoft.AspNetCore.Identity.SignInResult.Failed;
                }
                result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure);
            }
            else
            {
                result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure);
            }
            if (result.Succeeded)
            {
                if (loginByEmail)
                {
                    NPUser? user = await _userManager.FindByEmailAsync(model.UserName);
                    return user.StatusId != UserStatus.Active ? Microsoft.AspNetCore.Identity.SignInResult.NotAllowed : Microsoft.AspNetCore.Identity.SignInResult.Success;
                }
                else
                {
                    NPUser? user = await _userManager.FindByNameAsync(model.UserName);
                    return user.StatusId != UserStatus.Active ? Microsoft.AspNetCore.Identity.SignInResult.NotAllowed : Microsoft.AspNetCore.Identity.SignInResult.Success;
                }
            }
            else return result;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        #endregion

        public async Task<List<NPUser>> GetUsersInRoleAsync(string role)
        {
            //ToDo: проверка на null
            var users = await _userManager.GetUsersInRoleAsync(role);
            return users.ToList();
        }

        public string GetUserShortNameById(string userId)
        {
            NPUser? user = _userManager.FindByIdAsync(userId).Result;
            if (user is null) return "";
            return user.FirstName[0] + ". " + user.LastName;
        }

        #region UserAccess Management

        public async Task<List<UserAccess>> GetUserAccesses()
        {
            return await _unitOfWork.UserAccesses.GetCollectionAsync();
        }

        public async Task<bool> AddUserAccessAsync(AddAccessViewModel model)
        {
            UserAccess userAccess = new UserAccess
            {
                UserId = model.UserId,
                DivisionId = model.DivisionId,
                BranchId = model.BranchId == -1 ? null : model.BranchId
            };
            await _unitOfWork.UserAccesses.CreateAsync(userAccess);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<bool> DeleteUserAccess(string uId, int? dId, int? bId)
        {
            var uas = await _unitOfWork.UserAccesses.GetCollectionAsync();
            UserAccess? access = uas.Find(ua => ua.UserId == uId && ua.DivisionId == dId && ua.BranchId == bId);
            if (access is null) return false;
            _unitOfWork.UserAccesses.Delete(access);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<UserAccess?> GetUserAccess(string uId, int? dId, int? bId)
        {
            var uas = await _unitOfWork.UserAccesses.GetCollectionAsync();
            return uas.Find(ua => ua.UserId == uId && ua.DivisionId == dId && ua.BranchId == bId);
        }

        public async Task<NPUser> GetNPUserAsync(ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
        }

        public async Task<List<UserAccess>> GetUserAccesses(string userName)
        {
            NPUser user = await _userManager.FindByNameAsync(userName);
            var userAccesses = await _unitOfWork.UserAccesses.GetCollectionAsync();
            return userAccesses.Where(ua => ua.UserId == user.Id).ToList();
        }

        public async Task<List<NPUser>> GetStudentsByBranchId(int branchId, int statusId = 0)
        {
            var groups = _unitOfWork.Groups.GetByBranchId(branchId);
            if (statusId == 0) return [.. groups.SelectMany(g => g.Users)];
            else return [.. groups.SelectMany(g => g.Users.Where(u => u.StatusId == statusId))];
        }

        public async Task<List<NPUser>> GetStudentsByGroupId(int groupId, int statusId = 0)
        {
            var group = await _unitOfWork.Groups.GetByIdAsync(groupId);
            if (statusId == 0) return [.. group.Users];
            else return [.. group.Users.Where(u => u.StatusId == statusId)];
        }
        public List<NPUser> GetArchivedStudentsByBranchId(int branchId)
        {
            return _unitOfWork.Users.GetArchivedStudents(branchId);
        }

        #endregion

        public List<MentorLevel> GetMentorLevels(string mentorId)
        {
            return _unitOfWork.MentorLevels.GetLevelsByMentor(mentorId);
        }

        public async Task RemoveMentorLevel(string mentorId, int levelId)
        {
            var mentorLevels = _unitOfWork.MentorLevels.GetLevelsByMentor(mentorId);
            var mentorLevel = mentorLevels.Find(ml => ml.LevelId == levelId);
            if (mentorLevel is not null)
            {
                _unitOfWork.MentorLevels.Delete(mentorLevel);
                await _unitOfWork.Complete();
            }
        }

        public async Task AddMentorLevel(string mentorId, int levelId)
        {
            MentorLevel mentorLevel = new()
            {
                MentorId = mentorId,
                LevelId = levelId
            };
            await _unitOfWork.MentorLevels.CreateAsync(mentorLevel);
            await _unitOfWork.Complete();
        }

    }
}
