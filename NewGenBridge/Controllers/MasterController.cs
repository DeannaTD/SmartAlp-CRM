using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NewGenBridge.Data;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.Services;
using System.Net.WebSockets;
using System.Text.Json;
using System.Net;
using System.Net.Mail;
//ToDo: объединить в SeedData
namespace NewGenBridge.Controllers
{
    public class MasterController : Controller
    {
        ApplicationDbContext _context;
        RoleManager<IdentityRole> _roleManager;
        UserManager<NPUser> _userManager;
        IWebHostEnvironment _webHost;
        LessonService _lessonService;
        public MasterController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHost, UserManager<NPUser> userManager, LessonService lessonService)
        {
            _context = context;
            _roleManager = roleManager;
            _webHost = webHost;
            _userManager = userManager;
            _lessonService = lessonService;
        }

        //required to be an admin to access this method
        //[Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            if (User.Identity.Name == "dsotc")
                return View();
            else return BadRequest();
        }

        [Authorize(Policy = "DsotcOnly")]
        public async Task<IActionResult> InitializeRoles()
        {
            List<IdentityRole> roles = _roleManager.Roles.ToList();
            string[] message;
            if (roles.Count == 0)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" });
                await _roleManager.CreateAsync(new IdentityRole { Name = "Manager", NormalizedName = "MANAGER" });
                await _roleManager.CreateAsync(new IdentityRole { Name = "Mentor", NormalizedName = "MENTOR" });
                await _roleManager.CreateAsync(new IdentityRole { Name = "Student", NormalizedName = "STUDENT" });
                await _roleManager.CreateAsync(new IdentityRole { Name = "Supervisor", NormalizedName = "SUPERVISOR" });
                message = ["Таблица AspNetRoles обновлена"];
            }
            else
            {
                message = ["Таблица AspNetRoles уже заполнена"];
            }
            return View("Message", message);
        }

        [Authorize(Policy = "DsotcOnly")]
        public async Task AddRole()
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = "Surepvisor", NormalizedName = "SUPERVISOR" });
        }

        [Authorize(Policy = "DsotcOnly")]
        public async Task<IActionResult> InitializeStatus() //todo: make status creating with certain ids
        {
            List<Status> statuses = _context.Statuses.ToList();
            string[] message;
            if (statuses.Count == 0)
            {
                _context.Statuses.Add(new Status { Name = "Active" });
                _context.Statuses.Add(new Status { Name = "Blocked" });
                _context.Statuses.Add(new Status { Name = "Archived" });
                await _context.SaveChangesAsync();
                message = ["Таблица Status обновлена"];
            }
            else
            {
                message = ["Таблица Status уже заполнена"];
            }
            return View("Message", message);
        }


        [Authorize(Policy = "DsotcOnly")]
        public IActionResult CreateFolders()
        {
            List<string> message = [];
            string[] folders = ["Projects", "Profile"];
            foreach (string folder in folders)
            {
                string path = Path.Combine(_webHost.WebRootPath, folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    message.Add($"Папка {folder} создана");
                }
                else
                {
                    message.Add($"Папка {folder} уже существует");
                }
            }
            return View("~/Views/Master/Message.cshtml", message.ToArray());
        }


        [Authorize(Policy = "DsotcOnly")]
        public IActionResult ShowAllUsersWithRoles()
        {
            List<string> userStrings = [];
            foreach (var user in _context.Users)
            {
                string role = _userManager.GetRolesAsync(user).Result[0];
                userStrings.Add($"{user.UserName} - {role} - {user.Id}");
            }
            return View("~/Views/Master/Message.cshtml", userStrings.ToArray());
        }

        [Authorize(Policy = "DsotcOnly")]
        public IActionResult ShowManagers()
        {
            List<string> userStrings = [];
            foreach (var user in _context.Users)
            {
                string role = _userManager.GetRolesAsync(user).Result[0];
                if (role == NPUR.Manager || role == NPUR.Supervisor)
                    userStrings.Add($"{user.UserName} - {role}");
            }
            return View("~/Views/Master/Message.cshtml", userStrings.ToArray());
        }

        [Authorize(Policy = "DsotcOnly")]
        public async Task DropPassword(string id)
        {
            NPUser user = await _userManager.FindByIdAsync(id);
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, user.UserName);
            var k = result.Errors;
        }

        [Authorize(Policy = "DsotcOnly")]
        public async Task DropPasswordToNew(string username, string newPassword)
        {
            NPUser user = await _userManager.FindByNameAsync(username);
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            var k = result.Errors;
        }

        [Authorize(Policy = "DsotcOnly")]
        public void DeleteAttendancesOfUser(string userId)
        {
            List<Attendance> attendances = _context.Attendances.Where(a => a.UserId == userId).ToList();
            _context.Attendances.RemoveRange(attendances);
            _context.SaveChanges();
        }

        [Authorize(Policy = "DsotcOnly")]
        public void DeleteLessonsOfUser(string userId)
        {
            List<UserLesson> lessons = _context.UserLessons.Where(a => a.UserId == userId).ToList();
            _context.UserLessons.RemoveRange(lessons);
            _context.SaveChanges();
        }

        [Authorize(Policy = "DsotcOnly")]
        public async Task DeleteUser(string userId)
        {
            NPUser user = _context.Users.Find(userId);
            await _userManager.DeleteAsync(user);
            _context.SaveChanges();
        }
        [Authorize(Roles = "Admin,Supervisor")]
        public void DeleteGroup(int groupId)
        {
            Group group = _context.Groups.Find(groupId);
            _context.Groups.Remove(group);
            _context.SaveChanges();
        }
        [Authorize(Roles = "Admin,Supervisor")]
        public void DeleteBranch(int branchId)
        {
            Branch branch = _context.Branches.Find(branchId);
            _context.Branches.Remove(branch);
            _context.SaveChanges();
        }
        [Authorize(Policy = "DsotcOnly")]
        public void DeleteDivision(int divisionId)
        {
            Division division = _context.Divisions.Find(divisionId);
            _context.Divisions.Remove(division);
            _context.SaveChanges();
        }
        [Authorize(Roles = "Admin,Supervisor")]
        public string DeleteDivisionCascade(int divisionId)
        {
            Division division = _context.Divisions.Find(divisionId);
            List<Branch> branches = _context.Branches.Where(b => b.DivisionId == divisionId).ToList();
            foreach (Branch branch in branches)
            {
                List<Group> groups = _context.Groups.Where(g => g.BranchId == branch.Id).ToList();
                foreach (Group group in groups)
                {
                    List<NPUser> users = _context.Users.Where(u => u.GroupId == group.Id).ToList();
                    foreach (NPUser user in users)
                    {
                        DeleteAttendancesOfUser(user.Id);
                        DeleteLessonsOfUser(user.Id);
                    }
                    _context.Users.RemoveRange(users);
                    _context.SaveChanges();
                }
                _context.Groups.RemoveRange(groups);
                _context.SaveChanges();
                List<UserAccess> userAccesses = _context.UserAccesses.Where(ua => ua.BranchId == branch.Id).ToList();
                _context.UserAccesses.RemoveRange(userAccesses);
                _context.SaveChanges();
            }
            _context.Branches.RemoveRange(branches);
            _context.SaveChanges();
            List<UserAccess> userAccessesD = _context.UserAccesses.Where(ua => ua.DivisionId == division.Id).ToList();
            _context.UserAccesses.RemoveRange(userAccessesD);
            _context.SaveChanges();
            _context.Divisions.Remove(division);
            _context.SaveChanges();
            return "Deleted successfully";
        }

        [Authorize(Roles = "Admin,Supervisor")]
        public string DeleteBranchCascade(int branchId)
        {
            Branch branch = _context.Branches.Find(branchId);
            List<Group> groups = _context.Groups.Where(g => g.BranchId == branch.Id).ToList();
            foreach (Group group in groups)
            {
                List<NPUser> users = _context.Users.Where(u => u.GroupId == group.Id).ToList();
                foreach (NPUser user in users)
                {
                    DeleteAttendancesOfUser(user.Id);
                    DeleteLessonsOfUser(user.Id);
                }
                _context.Users.RemoveRange(users);
                _context.SaveChanges();
            }
            _context.Groups.RemoveRange(groups);
            _context.SaveChanges();
            List<UserAccess> userAccesses = _context.UserAccesses.Where(ua => ua.BranchId == branch.Id).ToList();
            _context.UserAccesses.RemoveRange(userAccesses);
            _context.SaveChanges();
            _context.Branches.Remove(branch);
            _context.SaveChanges();
            return "Deleted successfully";
        }

        [Authorize(Policy = "DsotcOnly")]
        public async Task<bool> CreatePricesTest()
        {
            var branches = _context.Branches.ToList();
            var levels = _context.Levels.ToList();
            foreach (var branch in branches)
            {
                foreach (var level in levels)
                {
                    Price price = new Price
                    {
                        BranchId = branch.Id,
                        LevelId = level.Id,
                        Sum = 0
                    };
                    _context.Prices.Add(price);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        [Authorize(Roles = "Admin,Supervisor")]
        public IActionResult TranslationPage()
        {
            List<Translation> translations = _context.Translations.ToList();
            return View("Translations", translations);
        }
        [Authorize(Roles = "Admin,Supervisor")]
        public IActionResult AddTranslation(string key, string en, string fr, string it, string tr, string de, string ru)
        {
            Translation translation = new Translation
            {
                NPKey = key,
                EnValue = en,
                FrValue = fr,
                ItValue = it,
                TrValue = tr,
                DeValue = de,
                RuValue = ru
            };
            _context.Translations.Add(translation);
            _context.SaveChanges();
            return RedirectToAction("TranslationPage");
        }

        [Authorize(Roles = "Admin,Supervisor")]
        public IActionResult ChangeTranslation()
        {
            var translation = _context.Translations.First(tr => tr.NPKey == "number_of_kits");
            //translation.NPKey = "amount_of_kits";
            Translation translations = new Translation()
            {
                NPKey = "amount_of_kits",
                EnValue = translation.EnValue,
                FrValue = translation.FrValue,
                ItValue = translation.ItValue,
                TrValue = translation.TrValue,
                DeValue = translation.DeValue,
                RuValue = translation.RuValue
            };
            _context.Translations.Add(translations);
            _context.SaveChanges();
            return Ok("Done");
        }


        [HttpGet]
        public IActionResult SetLanguage(string lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
                lang = "en";

            Response.Cookies.Append("lang", lang, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });

            var referer = Request.Headers["Referer"].ToString();
            return Redirect(string.IsNullOrEmpty(referer) ? "/" : referer);
        }

        [HttpGet]
        public async Task<IActionResult> ApiSettings()
        {
            if(User.Identity.Name != "dsotc")
                return BadRequest();
            string path = Path.Combine(_webHost.ContentRootPath, "Data", "api_branches.json");
            using (var reader = new StreamReader(path))
            {
                var json = await reader.ReadToEndAsync();
                var apiBranches = JsonSerializer.Deserialize<ApiData>(json);
                return View("ApiCountry", apiBranches);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApiSettingsSave(ApiData apiData)
        {
            if (User.Identity.Name != "dsotc")
                return BadRequest();
            string path = Path.Combine(_webHost.ContentRootPath, "Data", "api_branches.json");
            var json = JsonSerializer.Serialize(apiData, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(path, json);
            return RedirectToAction("ApiSettings");
        }

        [Authorize(Policy = "DsotcOnly")]
        public async Task<IActionResult> ApiSettingsAdd()
        {
            return PartialView("_AddApiCountryPartial");
        }

        [Authorize(Roles = "Admin,Supervisor")]
        [HttpPost]
        public async Task<IActionResult> ApiSettingsAdd(Country country)
        {
            if (User.Identity.Name != "dsotc")
                return BadRequest();
            //county.Code is always a string
            //country List<CountryInfo> Info is null and should be empty list
            country.Info = [];
            string path = Path.Combine(_webHost.ContentRootPath, "Data", "api_branches.json");
            var json = await System.IO.File.ReadAllTextAsync(path);
            var apiData = JsonSerializer.Deserialize<ApiData>(json);

            // Add new country to the list
            apiData.Countries.Add(country);

            // Save back to file (file is now closed from reading)
            var updatedJson = JsonSerializer.Serialize(apiData, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(path, updatedJson);

            return RedirectToAction("ApiSettings");
        }

        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> ApiSettingsNewGroup(string countryCode)
        {
            if (User.Identity.Name != "dsotc")
                return BadRequest();
            // Get first available group and level from database
            var firstGroup = _context.Groups.FirstOrDefault();
            var firstLevel = _context.Levels.FirstOrDefault();
            if (firstGroup == null || firstLevel == null)
                return BadRequest("No groups or levels available in the database");

            string path = Path.Combine(_webHost.ContentRootPath, "Data", "api_branches.json");
            var json = await System.IO.File.ReadAllTextAsync(path);
            var apiData = JsonSerializer.Deserialize<ApiData>(json);
            var country = apiData.Countries.FirstOrDefault(c => c.Code == countryCode);
            if (country == null)
                return BadRequest($"Country with code '{countryCode}' not found");
            var newCountryInfo = new CountryInfo
            {
                GroupId = firstGroup.Id,
                LevelId = firstLevel.Id
            };
            
            country.Info.Add(newCountryInfo);

            var updatedJson = JsonSerializer.Serialize(apiData, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(path, updatedJson);

            return RedirectToAction("ApiSettings");

        }
        
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> ApiSettingsDelete(string countryCode)
        {
            if (User.Identity.Name != "dsotc")
                return BadRequest();
            string path = Path.Combine(_webHost.ContentRootPath, "Data", "api_branches.json");
            var json = await System.IO.File.ReadAllTextAsync(path);
            var apiData = JsonSerializer.Deserialize<ApiData>(json);
            var country = apiData.Countries.FirstOrDefault(c => c.Code == countryCode);

            if (country == null)
                return BadRequest($"Country with code '{countryCode}' not found");

            apiData.Countries.Remove(country);

            var updatedJson = JsonSerializer.Serialize(apiData, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(path, updatedJson);

            return RedirectToAction("ApiSettings");

        }

        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<IActionResult> FixAmountContract()
        {
            var contracts = _context.Contracts.Where(c => c.PriceAmount == null).ToList();
            foreach (var contract in contracts)
            {
                var price = _context.Prices.Find(contract.PriceId);
                if (price != null)
                {
                    contract.PriceAmount = price.Sum;
                }
            }
            await _context.SaveChangesAsync();
            return Ok("Done");
        }

        [Authorize(Policy = "DsotcOnly")]
        public async Task<string> TempDeleteAgreements()
        {
            string[] agreementsids = [
                "#TO0505"];
            foreach (string agreementId in agreementsids)
            {
                var agreement = _context.Agreements.Find(agreementId);
                if (agreement != null)
                {
                    _context.Agreements.Remove(agreement);
                }
            }
            await _context.SaveChangesAsync();
            return "Done";
        }

        [Authorize(Policy = "DsotcOnly")]
        public string SetDueDateToBills()
        {
            var bills = _context.Bills.Where(b => b.DueDate == null).ToList();
            foreach (var bill in bills)
            {
                var contract = _context.Contracts.Find(bill.ContractId);
                bill.DueDate = contract.SignDate.AddMonths(
                    char.IsDigit(bill.BillNumber.Last())
                    ? int.Parse(bill.BillNumber.Last().ToString()) - 1
                    : 0);
            }
            _context.SaveChanges();
            return "Done";
        }
    }
}