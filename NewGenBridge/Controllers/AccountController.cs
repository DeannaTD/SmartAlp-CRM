using IronSoftware.Drawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.Services;
using NewGenBridge.ViewModels;
using QuestPDF.Helpers;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace NewGenBridge.Controllers
{
    public class AccountController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<NPUser> _userManager;
        AccountService _accountService;
        IWebHostEnvironment _webHost;
        LessonService _lessonService;
        BranchService _branchService;
        LevelService _levelService;
        SimpleLoggerService _logger;

        public AccountController(
            RoleManager<IdentityRole> roleManager,
            UserManager<NPUser> userManager,
            AccountService accountService,
            LessonService lessonService,
            IWebHostEnvironment webHost,
            BranchService branchService,
            SimpleLoggerService logger,
            LevelService levelService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _accountService = accountService;
            _lessonService = lessonService;
            _webHost = webHost;
            _branchService = branchService;
            _logger = logger;
            _levelService = levelService;
        }
        #region Login and logout

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var signResult = await _accountService.SignPassword(model);
                if (signResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (signResult.IsNotAllowed)
                    ModelState.AddModelError("", "Your account has been blocked");
                else
                    ModelState.AddModelError("", "Invalid username or password");
            }
            else
            {
                ModelState.AddModelError("", "Invalid data input");
            }
            return View(model);

        }

        public async Task<IActionResult> Logout()
        {
            //await _logger.Log(LogLabel.Logout(User?.Identity?.Name));
            await _accountService.SignOutAsync();
            return RedirectToAction("Login");
        }
        #endregion
        #region Registration of new users

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            List<SelectListItem> roleList = [];
            List<string> enabledRoles = [];
            NPUser user = await _accountService.GetNPUserAsync(User);
            int access = -1;
            List<int?> divisions = [];
            List<UserAccess> userAccess = [];
            if (User.IsInRole(NPUR.Admin)) access = 1;
            if(User.IsInRole(NPUR.Supervisor)) access = 2;
            if(User.IsInRole(NPUR.Manager)) access = 3;
            if(User.Identity.Name == "dsotc") access = 0;
            switch (access)
            {
                case 0: //for me
                    enabledRoles = [NPUR.Mentor, NPUR.Manager, NPUR.Admin, NPUR.Supervisor];
                    divisions = _branchService.GetDivisionsAsync().Result.Select(d => (int?)d.Id).ToList();
                    break;
                case 1: //for admin
                    enabledRoles = [NPUR.Manager, NPUR.Supervisor];
                    divisions = _branchService.GetDivisionsAsync().Result.Select(d => (int?)d.Id).ToList();
                    break;
                case 2: //for supervisor
                    enabledRoles = [NPUR.Mentor, NPUR.Manager];
                    userAccess = await _accountService.GetUserAccesses();
                    divisions = userAccess.Where(userAccess => userAccess.UserId == user.Id).Select(ua => ua.DivisionId).ToList();
                    break;
                case 3: //for manager
                    enabledRoles = [NPUR.Mentor];
                    userAccess = await _accountService.GetUserAccesses();
                    divisions = userAccess.Where(userAccess => userAccess.UserId == user.Id).Select(ua => ua.DivisionId).ToList();
                    break;
            }
            foreach (IdentityRole role in _roleManager.Roles)
            {
                //ToDo: сделать что-то с проверкой через строки (enum или словарь)
                if (enabledRoles.Contains(role.Name))
                    roleList.Add(new SelectListItem { Text = role.Name, Value = role.Name });
            }

            ViewBag.RoleList = roleList;
            ViewBag.DivisionIds = divisions.Distinct();
            return View("UserRegistration");
        }

        [HttpPost]
        public async Task<string> Register(StaffRegistrationViewModel model)
        {
            string message;
            if (ModelState.IsValid)
            {
                (bool, string) result = await _accountService.RegisterAsync(model);
                message = result.Item1 ? "Created successfully\nUsername:" + result.Item2 : "Error code: #SCURW05";
            }
            else message = "Error code: #MVURW05";
            //if(!message.Contains("#"))
                //await _logger.Log(LogLabel.Register(User?.Identity?.Name, $"{model.FirstName} {model.LastName}", model.Role));
            return message;
        }

        [HttpGet]
        public IActionResult RegisterStudent(string groupId)
        {
            ViewBag.GroupId = groupId;
            return View("StudentRegistration");

        }

        [HttpPost]
        public async Task<string> RegisterStudent(StudentRegistrationViewModel model)
        {
            string message;
            if (ModelState.IsValid)
            {
                (bool, string) result = await _accountService.RegisterAsync(model);
                if(result.Item1)
                    message = "Created successfully.\nUsername: " + result.Item2;
                else message = "Error code: #SCSRW07";
            }
            else message = "Invalid data, try again (error code: #MVSRW07)";
            //if (!message.Contains("#"))
                //await _logger.Log(LogLabel.Register(User?.Identity?.Name, $"{model.FirstName} {model.LastName}", NPUR.Student));
            return message;
        }
        #endregion

        #region Settings

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            //ToDo: добавить перевод на страницу ошибки если пользователь не найден или вылетел
            NPUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            SettingsViewModel model = new SettingsViewModel()
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            NPUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            IdentityResult result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(errors);
            }
        }

        [HttpPost]
        public IActionResult ChangeProfilePicture(IFormFile picture)
        {
            try
            {
                string path = Path.Combine(_webHost.WebRootPath, "Profile");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string fileName = User.Identity.Name + ".png";
                AnyBitmap bitmap = AnyBitmap.FromStream(picture.OpenReadStream());
                int koef = bitmap.Width / 100;
                AnyBitmap newBitmap = new AnyBitmap(bitmap, bitmap.Width / koef, bitmap.Height / koef);
                newBitmap.SaveAs(Path.Combine(path, fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Error occured: " + ex.Message);
            }
            return RedirectToAction("Settings");
        }
        #endregion


        #region UserStudent management
        [HttpPost]
        public async Task UpdateStudentLessonAsync(StudentLessonViewModel model)
        {
            NPUser student = await _userManager.FindByIdAsync(model.StudentId);
            student.CurrentLessonId = model.LessonId;
            await _userManager.UpdateAsync(student);
            await _lessonService.OpenLessonToStudent(model.StudentId, model.LessonId ?? 0);
            //await _logger.Log($"LessonOpened: {model.StudentNameInfo} ({model.StudentId}) to {model.LessonId}");
        }
        #endregion

        public async Task<IActionResult> StudentInfoHandle(string studentId) //todo: COMMENT THIS SHIT
        {
            NPUser student = await _userManager.FindByIdAsync(studentId);
            Level level = await _lessonService.GetLevelByLessonId(student.CurrentLessonId ?? 1);
            StudentInfoViewModel model = new()
            {
                StudentId = student.Id,
                StatusId = student.StatusId ?? 0,
                ParentName = student.ParentName,
                PhoneNumber = student.PhoneNumber ?? "",
                FirstName = student.FirstName,
                LastName = student.LastName,
                Started = DateOnly.FromDateTime(student.DateStarted),
                DateOfBirth = DateOnly.FromDateTime(student.DateOfBirth),
                Note = student.Note ?? "",
                GroupId = student.GroupId ?? 0,
                LevelId = level.Id,
                LevelName = level.Name
            };
            Group group = model.GroupId > 0 ? await _branchService.GetGroupByIdAsync(model.GroupId) : null;
            Branch branch = group != null ? await _branchService.GetBranchByIdAsync(group.BranchId)
                : student.MentorDivisionId.HasValue ? await _branchService.GetBranchByIdAsync(student.MentorDivisionId.Value) : null;
            List<Branch> branches = await _branchService.GetBranchesAsync();
            
            SelectList branchList = new(from br in branches where branch != null && br.DivisionId == branch.DivisionId
                                        from gr in br.Groups select new { 
                                            Id = gr.Id,
                                            Name = br.Name + ", " + (gr.Name ?? "") + " " + gr.Day.ToString() + " " + gr.Time.ToString("hh\\:mm") 
                                        }, "Id", "Name");
            List<SelectListItem> mentorNames = [];
            List<string> mentorIds = [];
            foreach(var selectItem in branchList)
            {
                UserData result = await _branchService.GetGroupMentorData(Convert.ToInt32(selectItem.Value));
                mentorNames.Add(new SelectListItem($"{result.FirstName} {result.LastName}", selectItem.Value));
                mentorIds.Add(result.Id);
            }
            ViewBag.BranchList = branchList;
            ViewBag.GroupsMentors = mentorNames;
            return View("StudentInfoMenu", model);
        }

        public async Task<string> UpdateStudent(StudentInfoViewModel model)
        {
            string message;
            if(ModelState.IsValid)
            {
                NPUser student = await _userManager.FindByIdAsync(model.StudentId);
                student.ParentName = model.ParentName;
                student.PhoneNumber = model.PhoneNumber;
                student.FirstName = model.FirstName;
                student.LastName = model.LastName;
                student.DateStarted = model.Started.ToDateTime(TimeOnly.MinValue);
                student.DateOfBirth = model.DateOfBirth.ToDateTime(TimeOnly.MinValue);
                student.Note = model.Note;
                student.GroupId = model.GroupId;
                IdentityResult result = await _userManager.UpdateAsync(student);
                message = result.Succeeded ? "Updated successfully" : "Error code: #UMSIW54";
            }
            else message = "Invalid data, try again (error code: #MVSIW54)";
            return message;
            
        }

        public async Task<IActionResult> Mentors() //todo: think about better naming
        {
            List<NPUser> mentors = await _accountService.GetUsersInRoleAsync(NPUR.Mentor);
            List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
            SortedSet<NPUser> model = new(Comparer<NPUser>.Create((x, y) => x.Id.CompareTo(y.Id)));
            IEnumerable<int?> userAccessibleStructures = accesses.Select(a => a.DivisionId).Distinct();
            foreach (int? divisionId in userAccessibleStructures)
            {
                if (divisionId is null) continue;
                List<NPUser> mentorsOfDivision = mentors.FindAll(mentor => mentor.MentorDivisionId == divisionId);
                model.UnionWith(mentorsOfDivision);
            }
            return View(model.Select(u => new MentorManagerViewModel() { 
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                StatusId = u.StatusId
            }).ToList());
        }

        public async Task<IActionResult> Students(int statusId = 0, int page = 1, string name = "", int branchId = 0, int groupId = 0)
        {

            int pageSize = 20;
            List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
            IEnumerable<int?> userAccesses = accesses.Select(a => a.BranchId).Where(a => a is not null).Distinct().ToList();
            var model = new List<StudentManagerViewModel>();
            foreach(int? access in userAccesses)
            {
                List<NPUser> students = await _accountService.GetStudentsByBranchId(access ?? 0, statusId);
                model.AddRange(students.Select(s => new StudentManagerViewModel() { 
                    Id = s.Id,
                    BranchId = access ?? 0,
                    FullName = s.FirstName + " " + s.LastName,
                    GroupId = s.GroupId ?? 0,
                    StatusId = s.StatusId ?? 0,
                    StartedOn = DateOnly.FromDateTime(s.DateStarted)
                }));
            }
            if (statusId == UserStatus.Archived || statusId == UserStatus.All)
            {
                var detached = (await _userManager.GetUsersInRoleAsync(NPUR.Student))
                    .Where(u => u.GroupId == null && u.StatusId == UserStatus.Archived && userAccesses.Contains(u.MentorDivisionId));
                model.AddRange(detached.Select(u => new StudentManagerViewModel { Id=u.Id, BranchId=u.MentorDivisionId ?? 0, FullName=u.FirstName+" "+u.LastName, GroupId=0, StatusId=UserStatus.Archived, StartedOn=DateOnly.FromDateTime(u.DateStarted) }));
            }
            model = model.DistinctBy(s => s.Id).ToList();
            name = (name ?? "").Trim();
            if(name!="")
            {
                model = model.Where(s => s.FullName.ToLower().Contains(name.ToLower())).ToList();
                ViewBag.Name = name;
            }

            // Build group list for dropdown
            List<SelectListItem> groupList = new List<SelectListItem>();
            var branchList = new List<SelectListItem>();
            var groupBranches = new Dictionary<string, int>();
            foreach (int? allowedBranchId in userAccesses)
            {
                Branch branch = await _branchService.GetBranchByIdAsync(allowedBranchId ?? 0);
                List<Group> groups = _branchService.GetGroupsByBranchId(allowedBranchId ?? 0);
                branchList.Add(new SelectListItem { Value = branch.Id.ToString(), Text = branch.Name });
                foreach (var g in groups) groupBranches[g.Id.ToString()] = branch.Id;
                groupList.AddRange(groups.Select(
                    g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = branch.Name + ", " + g.Name + " " + g.Day + " " + g.Time.ToString("hh\\:mm")
                    }));
            }

            if (branchId != 0) model = model.Where(s => s.BranchId == branchId).ToList();
            if (groupId != 0) model = model.Where(s => s.GroupId == groupId).ToList();
            model = model.OrderBy(s => s.FullName).ThenBy(s => s.Id).ToList();
            ViewBag.BranchList = branchList;
            ViewBag.GroupBranches = groupBranches;
            ViewBag.BranchId = branchId;
            ViewBag.GroupId = groupId;
            ViewBag.ResultCount = model.Count;
            ViewBag.GroupList = groupList;
            ViewBag.BranchesId = userAccesses;
            ViewBag.StatusId = statusId;
            ViewBag.TotalPages = (int)Math.Ceiling((double)model.Count / pageSize);
            ViewBag.NamesList = model.Select(c => new { c.FullName}).DistinctBy(c => c.FullName).ToList();
            page = Math.Clamp(page, 1, Math.Max(1, (int)ViewBag.TotalPages));
            model = model.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_StudentsAllPartial", model);
            }
            return View("StudentsAll", model);
        }

        public async Task<string> ToggleUserStatus(string userId, int statusId, int groupId = -1)
        {
            NPUser user = await _userManager.FindByIdAsync(userId);
            //if user is blocked and we want to block him again, we should activate him
            if (user.StatusId == statusId && statusId != UserStatus.Active)
                statusId = UserStatus.Active;

            user.StatusId = statusId;
            if(groupId != -1) user.GroupId = groupId;
            IdentityResult result = await _userManager.UpdateAsync(user);
            return $"{userId} {statusId}";
        }

        public async Task<string> SwitchUserStatus(string userId, int statusId, int groupId = -1)
        {
            NPUser user = await _userManager.FindByIdAsync(userId);

            user.StatusId = statusId;
            if (groupId != -1) user.GroupId = groupId;
            IdentityResult result = await _userManager.UpdateAsync(user);
            return $"{userId} {statusId}";
        }

        public async Task ArchiveStudent(string studentId)
        {
            NPUser student = await _userManager.FindByIdAsync(studentId);
            student.StatusId = UserStatus.Archived;
            var branch = await _branchService.GetGroupByIdAsync(student.GroupId ?? 0);
            student.GroupId = null;
            student.MentorDivisionId = branch.BranchId;
            await _userManager.UpdateAsync(student);
        }

        public async Task<IActionResult> ArchivedStudents()
        {
            List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
            IEnumerable<int?> userAccesses = accesses.Select(a => a.BranchId).Where(a => a is not null);
            var model = new List<StudentManagerViewModel>();
            foreach (int? access in userAccesses)
            {
                IEnumerable<NPUser> students = _userManager.GetUsersInRoleAsync(NPUR.Student).Result.
                                                Where(u => u.MentorDivisionId == access && u.StatusId == UserStatus.Archived);
                model.AddRange(students.Select(s => new StudentManagerViewModel()
                {
                    Id = s.Id,
                    FullName = s.FirstName + " " + s.LastName,
                    GroupId = -1,
                    StatusId = s.StatusId ?? 3
                }));
            }
            ViewBag.BranchesId = userAccesses;
            ViewBag.StatusId = UserStatus.Archived;
            return View("Students", model);
        }

        public async Task<string> GetAllStudents()
        {
            var users = await _accountService.GetUsersInRoleAsync(NPUR.Student);
            StringBuilder builder = new();
            foreach (NPUser user in users)
            {
                builder.Append(user.FirstName + " " + user.MentorDivisionId + "\n");
            }
            return builder.ToString();
        }

        public async Task<IActionResult> ShowUsers(bool isManager = false)
        {
            List<MentorManagerViewModel> model = [];
            List<string> enabledRoles = [];
            if (isManager && User.IsInRole(NPUR.Admin) || User.IsInRole(NPUR.Supervisor))
            {
                enabledRoles = [NPUR.Manager];
                ViewBag.Title = "Managers";
            }
            else if (!isManager && User.IsInRole(NPUR.Admin))
            {
                enabledRoles = [NPUR.Supervisor];
                ViewBag.Title = "Supervisors";
            }
            foreach(string role in enabledRoles)
            {
                var users = await _userManager.GetUsersInRoleAsync(role);
                model.AddRange(users.Select(u => new MentorManagerViewModel()
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    StatusId = u.StatusId
                }));
            }
            return View("Users", model);

        }

        public async Task<IActionResult> SwitchLessonToAll(SwitchLessonViewModel model)
        {
            if(ModelState.IsValid)
            {
                var students = await _accountService.GetStudentsByGroupId(model.GroupId, UserStatus.Active);
                foreach(var student in students)
                {
                    student.CurrentLessonId = model.LessonId;
                    await _userManager.UpdateAsync(student);
                    await _lessonService.OpenLessonToStudent(student.Id, model.LessonId ?? 0);
                }
                Group group = await _branchService.GetGroupByIdAsync(model.GroupId);
                group.lessonId = model.LessonId ?? 0;
                await _branchService.UpdateGroup(group);

                var mentor = await _userManager.FindByIdAsync(group.MentorId);
                await _lessonService.OpenLessonToStudent(mentor.Id, model.LessonId ?? 0);
                return Ok();
            }
            return BadRequest();
        }

        public async Task<IActionResult> SwitchLessonToMentor(string userId, int lessonId, int oType)
        {
            int duration = 0;
            switch (oType)
            {
                case 0: duration = 0; break;
                case 1: duration = 1; break;
                case 2: duration = 30; break;
                case 3: duration = 36525; break;
            }
            if (duration == 0)
            {
                try
                {
                    await _lessonService.CloseLessonForStudent(userId, lessonId);

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                try
                {
                    await _lessonService.OpenLessonToStudent(userId, lessonId, duration, true);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return Ok();
        }

        public async Task<IActionResult> MentorLevelAccess()
        {
            List<NPUser> mentors = await _accountService.GetUsersInRoleAsync(NPUR.Mentor);
            List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
            SortedSet<NPUser> model = new(Comparer<NPUser>.Create((x, y) => x.Id.CompareTo(y.Id)));
            IEnumerable<int?> userAccessibleStructures = accesses.Select(a => a.DivisionId).Distinct();
            foreach (int? divisionId in userAccessibleStructures)
            {
                if (divisionId is null) continue;
                List<NPUser> mentorsOfDivision = mentors.FindAll(mentor => mentor.MentorDivisionId == divisionId);
                model.UnionWith(mentorsOfDivision);
            }

            List<MentorLevelAccessViewModel> viewModel = [];
            List<Level> levels = _levelService.GetLevelsOnly();
            foreach (NPUser mentor in model)
            {
                List<MentorLevel> mentorLevelAccess = _accountService.GetMentorLevels(mentor.Id);
                List<LevelAccess> la = levels.Select(level => new LevelAccess()
                {
                    LevelId = level.Id,
                    LevelName = level.Name,
                    HasAccess = mentorLevelAccess.Any(ml => ml.LevelId == level.Id)
                }).ToList();
                viewModel.Add(new MentorLevelAccessViewModel()
                {
                    MentorId = mentor.Id,
                    MentorName = mentor.FirstName + " " + mentor.LastName,
                    LevelAccesses = la
                });
            }
            return View(viewModel);
        }

        public async Task ToggleMentorLevelAccess(string userId, int levelId, bool hasAccess)
        {
            if (!hasAccess)
            {
                await _accountService.RemoveMentorLevel(userId, levelId);
            }
            else
            {
                await _accountService.AddMentorLevel(userId, levelId);
            }
        }

        public async Task<IActionResult> MentorLessonAccess(string mentorId)
        {
            NPUser mentor = await _userManager.FindByIdAsync(mentorId);

            MentorLessonAccessViewModel viewModel = new MentorLessonAccessViewModel();
            viewModel.LevelsWithLessons = await _levelService.GetLevelsAsync();
            
            List<MentorLevel> mentorLevelAccess = _accountService.GetMentorLevels(mentor.Id); //all levels assigned to mentor
            var mentorLessons = await _lessonService.GetStudentLessonHistory(mentor.Id); //all lessons assigned to mentor
            mentorLessons = mentorLessons.Where(ml => ml.IsOpened).ToList(); //all lessons opened to mentor

            viewModel.MentorId = mentor.Id;
            viewModel.MentorName = mentor.FirstName + " " + mentor.LastName;
            viewModel.LevelsLessons = [];

            foreach (var lvl in mentorLevelAccess)
            {
                LevelLessonsMentorViewModel tmp = new LevelLessonsMentorViewModel();
                tmp.LevelId = lvl.LevelId;
                var lessonsOfLevelIds = viewModel.LevelsWithLessons.FirstOrDefault(l => l.Id == lvl.LevelId)?.Lessons.Select(les => les.Id);
                tmp.Lessons = mentorLessons.Where(ml => lessonsOfLevelIds != null && lessonsOfLevelIds.Contains(ml.LessonId)).ToList();
                viewModel.LevelsLessons.Add(tmp);
            }

            return View(viewModel);
        }
    }
}