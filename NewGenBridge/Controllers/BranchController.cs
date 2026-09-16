using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.Services;
using NewGenBridge.ViewModels;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NewGenBridge.Controllers
{
    public class BranchController(BranchService branchService, UserService userService, AccountService accountService, SimpleLoggerService logger, ContractService contractService) : Controller
    {
        BranchService _branchService = branchService;
        UserService _userService = userService;
        AccountService _accountService = accountService;
        SimpleLoggerService _logger = logger;
        ContractService _contractService = contractService;

        [HttpGet]
        public IActionResult Create(int divisionId)
        {
            List<SelectListItem> TimeZones = TimeZoneInfo.GetSystemTimeZones().Select(tz => new SelectListItem
            {
                Value = tz.Id,
                Text = tz.DisplayName
            }).ToList();
            ViewBag.TimeZones = TimeZones;
            return View("CreateBranch", new Branch() { DivisionId = divisionId });
        }

        [HttpPost]
        public async Task<string> Create(Branch branch)
        {
            string message;
            if (ModelState.IsValid)
            {
                bool Result = await _branchService.CreateAsync(branch);

                //TODO: TEST TEST TEST
                Result &= await _contractService.AddPricesToNewBranchAsync(branch.Id);
                message = Result ? "Created successfully" : "Error code: #SCBRN17";
            }
            else
            {
                message = "Error code: #MVBRN17";
            }
            //if (!message.Contains("#"))
                //await _logger.Log(LogLabel.CreateBranch(User?.Identity?.Name, branch.Name, branch.DivisionId));
            return message;
        }

        [HttpGet]
        public async Task<IActionResult> CreateGroup(int branchId)
        {
            Division division = await _branchService.GetDivisionByBranchId(branchId);
            GroupCreationViewModel model = new()
            {
                BranchId = branchId
            };
            List<NPUser> mentors = await _userService.GetAllUsersInRoleAsync("Mentor");

            model.Mentors = new SelectList(from mentor in mentors
                                           where mentor.MentorDivisionId == division.Id
                                           select new
                                           {
                                               Id = mentor.Id,
                                               Name = mentor.FirstName + " " + mentor.LastName
                                           }, "Id", "Name");
            return PartialView("CreateGroup", model);
        }

        [HttpPost]
        public async Task<string> CreateGroup(GroupCreationViewModel group)
        {
            string message;
            if (ModelState.IsValid)
            {
                bool Result = await _branchService.CreateGroupAsync(group);
                message = Result ? "Created successfully" : "Error code: #SCGCW19";
            }
            else
            {
                message = "Invalid data, try again (error code: #MVGCW19)";
            }
            //if (!message.Contains("#"))
                //await _logger.Log(LogLabel.CreateGroup(User?.Identity?.Name, $"{group.Day} {group.Time.Hours}:{group.Time.Minutes}", group.BranchId));
            return message;
        }


        public async Task<IActionResult> EditGroup(int groupId)
        {
            Group group = await _branchService.GetGroupByIdAsync(groupId);
            Division division = await _branchService.GetDivisionByBranchId(group.BranchId);
            GroupEditionViewModel model = new(group);
            List<NPUser> mentors = await _userService.GetAllUsersInRoleAsync("Mentor");
            model.Mentors = new SelectList(from mentor in mentors where mentor.MentorDivisionId == division.Id select new
            {
               Id = mentor.Id,
               Name = mentor.FirstName + " " + mentor.LastName
            }, "Id", "Name", model.MentorId);
            return View("EditGroup", model);
        }

        public async Task<string> UpdateGroup(GroupEditionViewModel model)
        {
            string message;
            string changes = "";
            if(ModelState.IsValid)
            {
                Group group = await _branchService.GetGroupByIdAsync(model.GroupId);
                if(model.MentorId != group.MentorId)
                {
                    changes += $"Mentor changed from {group.MentorId} to {model.MentorId}; ";
                }
                if(model.Time != group.Time)
                {
                    changes += $"Time changed from {group.Time} to {model.Time}; ";
                }
                if(model.Day != group.Day)
                {
                    changes += $"Day changed from {group.Day} to {model.Day}; ";
                }
                if(model.Name != group.Name)
                {
                    changes += $"Name changed from {group.Name} to {model.Name}; ";
                }
                group.MentorId = model.MentorId;
                group.Time = model.Time;
                group.Day = model.Day;
                group.Name = model.Name ?? "";
                bool Result = await _branchService.UpdateGroup(group);
                

                //get user by model.MentorId
                var mentor = await _userService.GetUserByIdAsync(model.MentorId);
                mentor.MentorLastGroupBranchId = group.BranchId;
                await _userService.UpdateUser(mentor);
                message = Result ? "Updated successfully" : "Error code: #SUGEW53";
            }
            else
            {
                message = "Invalid data, try again (error code: #MVGEW53)";
            }
            //if(changes != "")
                //await _logger.Log($"GroupUpdate: {model.GroupId} changed: {changes}");
            return message;
        }

        public async Task<IActionResult> Edit(int branchId)
        {
            List<Branch> branches = await _branchService.GetBranchesAsync();
            Branch branch = branches.Find(branches => branches.Id == branchId);
            List<SelectListItem> TimeZones = TimeZoneInfo.GetSystemTimeZones().Select(tz => new SelectListItem
            {
                Value = tz.Id,
                Text = tz.DisplayName
            }).ToList();
            ViewBag.TimeZones = TimeZones;
            return View(branch);
        }

        public async Task<string> Update(Branch branch)
        {
            string message;
            bool result = await _branchService.UpdateBranch(branch);
            message = result ? "Updated successfully" : "Error code: #SUBRN45";
            if (result)
            {
                //await _logger.Log($"BranchUpdate: {branch.Id} changed");
            }
            return message;
        }

        public async Task<string> Delete(int branchId)
        {
            string message;
            List<Branch> branches = await _branchService.GetBranchesAsync();
            Branch branch = branches.Find(b => b.Id == branchId);
            string branchInfo = branch.Name + " " + branch.DivisionId;
            if (branch.Groups.Count != 0)
            {
                message = "Branch is not empty (error code: #CDBRN46)";
            }
            else
            {
                bool result = await _branchService.DeleteBranch(branch);
                message = result ? "Deleted successfully" : "Error code: #SDBRN46";
            }
            //if(!message.Contains("#"))
                //await _logger.Log($"BranchDelete: {branchId} deleted ({branchInfo})");
            return message;
        }

        public async Task<IActionResult> Index(bool isPersonal = false, string mentorId = "")
        {
            List<Branch> branches;
            if (isPersonal && mentorId != "")
                branches = await _branchService.GetBranchesAndGroupsByMentorAsync(mentorId);
            else
                branches = await _branchService.GetBranchesAsync();
            ViewBag.MentorId = mentorId;
            ViewBag.IsPersonal = isPersonal;
            return View(branches);
        }

        public async Task<IActionResult> BranchControlPanel()
        {
            List<BranchPanelViewModel> model = [];
            List<Branch> branches = new();
            var userAccesses = await _accountService.GetUserAccesses(User.Identity.Name);
            if (User.IsInRole(NPUR.Supervisor))
            {
                var divisionsIds = userAccesses.Select(ua => ua.DivisionId);
                foreach (int divisionId in divisionsIds)
                {
                    branches.AddRange(await _branchService.GetBranchesWithStudents(divisionId));
                }
            }
            else if (User.IsInRole(NPUR.Manager))
            {
                IEnumerable<int> branchesIds = userAccesses.Select(ua => ua.BranchId ?? 0);
                branches = await _branchService.GetBranchesWithStudents(branchesIds.ToArray());
            }
            else if(User.IsInRole(NPUR.Admin))
            {
                branches = await _branchService.GetBranchesWithStudents();
            }
            foreach(Branch branch in branches)
            {
                //Division division = await _branchService.GetDivisionByBranchId(branch.Id);
                model.Add(new BranchPanelViewModel()
                {
                    BranchId = branch.Id,
                    BranchName = branch.Name,
                    GroupsInfo = branch.Groups.Select(group => new GroupInBranchViewModel()
                    {
                        GroupId = group.Id,
                        TimeString = group.Time.ToString("hh\\:mm"),
                        Day = group.Day,
                        MentorName = _accountService.GetUserShortNameById(group.MentorId),
                        StudentsCount = group.Users.Count,
                        GroupName = group.Name ?? ""
                    }).ToList()
                });
            }
            return View("ControlPanel", model);
        }

        public async Task<IActionResult> Group(int groupId)
        { //ToDo: если группа не найдена
            List<NPUser> students = await _accountService.GetUsersInRoleAsync(NPUR.Student);
            return View(students.Where(student => student.GroupId == groupId));
        }

        #region Division

        [HttpGet]
        public IActionResult CreateDivision()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<string> CreateDivision(Division division)
        {
            string message;
            if (ModelState.IsValid)
            {
                bool Result = await _branchService.CreateDivision(division);
                message = Result ? "Created successfully" : "Error: code #SCDIV24";
            }
            else
            {
                message = "Error code: #MVDIV24";
            }
            //if(!message.Contains("#"))
                //await _logger.Log(LogLabel.CreateDivision(User?.Identity?.Name, division.Name, division.Id));
            return message;
        }

        public async Task<IActionResult> StructureControl()
        {
            List<Division> model = await _branchService.GetDivisionsAsync();
            if(User.IsInRole(NPUR.Supervisor))
            {
                var userAccess = await _accountService.GetUserAccesses(User.Identity.Name);
                model = model.Where(d => userAccess.Find(ua => ua.DivisionId == d.Id) != null).ToList();
            }
            model.Sort((a, b) => b.Branches.Count.CompareTo(a.Branches.Count));
            return View(model);
        }

        public async Task<IActionResult> EditDivision(int divisionId)
        {
            Division division = await _branchService.GetDivisionByIdAsync(divisionId);
            return View(division);
        }

        public async Task<string> UpdateDivision(Division division)
        {
            string message;
            if (ModelState.IsValid)
            {
                bool Result = await _branchService.UpdateDivision(division);
                message = Result ? "Updated successfully" : "Error code: #SUDIV42";
            }
            else
            {
                message = "Error code: #MVDIV42";
            }
            //if(!message.Contains("#"))
                //await _logger.Log($"DivisionUpdate: {division.Id} changed");
            return message;
        }

        public async Task<string> DeleteDivision(int divisionId)
        {
            string message;
            Division division = await _branchService.GetDivisionByIdWidthBranchesAsync(divisionId);
            if (division.Branches.Count == 0)
            {
                bool Result = await _branchService.DeleteDivision(division);
                message = Result ? "Deleted successfully" : "Error code: #SDDIV43";
            }
            else
            {
                message = "Division is not empty (error code: #CDDIV43)";
            }
            return message;
        }
        #endregion


        #region UserAccessToDivisions

        [HttpGet]
        public async Task<IActionResult> ManageAccess()
        {
            if(User.IsInRole(NPUR.Supervisor)) return RedirectToAction("ManageAccessRestricted");
            List<ManagerAccessViewModel> model = [];
            List<UserAccess> userAccess = await _accountService.GetUserAccesses();
            List<Division> divisions = await _branchService.GetDivisionsAsync();
            foreach (Division division in divisions)
            {
                List<string> ids = userAccess.Where(ua => ua.DivisionId == division.Id && ua.BranchId == null).Select(ua => ua.UserId).ToList();
                List<(string, string)> values = [];
                ids.ForEach(id => values.Add((id, _userService.GetUserFullNameById(id))));
                model.Add(new ManagerAccessViewModel()
                {
                    DivisionId = division.Id,
                    BranchId = -1,
                    StructureName = division.Name,
                    ManagersIds = values
                });

                foreach (Branch branch in division.Branches)
                {
                    ids = userAccess.Where(ua => ua.BranchId == branch.Id).Select(ua => ua.UserId).ToList();
                    values = [];
                    ids.ForEach(id => values.Add((id, _userService.GetUserFullNameById(id))));
                    model.Add(new ManagerAccessViewModel()
                    {
                        DivisionId = division.Id,
                        BranchId = branch.Id,
                        StructureName = branch.Name,
                        ManagersIds = values
                    });
                }
            }
            
            return View("~/Views/Supervision/ManageAccess.cshtml", model);
        }

        [HttpGet]
        public IActionResult AddAccess(int divisionId, int branchId)
        {
            string Name = "";
            Division division = _branchService.GetDivisionByIdAsync(divisionId).Result;
            if (branchId != -1)
            {
                Branch branch = _branchService.GetBranchByIdAsync(branchId).Result;
                Name = division.Name + " - " + branch.Name;
            }
            else Name = division.Name;
            AddAccessViewModel model = new()
            {
                DivisionId = divisionId,
                BranchId = branchId,
                StructureName = Name
            };
            return View("~/Views/Supervision/AddAccessMenu.cshtml", model);
        }

        [HttpPost]
        public async Task<string> AddAccessAsync(AddAccessViewModel model)
        {
            string message;
            model.BranchId = model.BranchId == -1 ? null : model.BranchId;
            if (ModelState.IsValid)
            {
                UserAccess? userAccess = await _accountService.GetUserAccess(model.UserId, model.DivisionId, model.BranchId);
                if (userAccess is null)
                {
                    bool Result = await _accountService.AddUserAccessAsync(model);
                    message = Result ? "Access granted successfully" : "Error code: #SCAAW49";

                }
                else message = "Access already has been granted (error code: #DEAAW49)";
            }
            else
            {
                message = "Error code: #MVAAW49";
            }
            //if (!message.Contains("#"))
                //await _logger.Log(LogLabel.AccessGrant(User.Identity.Name, model.UserId, model.DivisionId, model.BranchId));
            return message;
        }

        public async Task<string> RemoveAccess(string userId, int divisionId, int branchId)
        {
            int? bId = branchId == -1 ? null : branchId;
            int? dId = divisionId == -1 ? null : divisionId;
            bool result = await _accountService.DeleteUserAccess(userId, dId, bId);
            if (result)
                await _logger.Log($"AccessRemove: by{User.Identity.Name} to {userId} in {divisionId} : {branchId}");
            return result ? "Access removed successfully" : "Error code: #SCUAM50";
        }

        public async Task<IActionResult> ManageAccessRestricted()
        {
            NPUser user = await _accountService.GetNPUserAsync(User);
            string userId = user.Id;
            List<ManagerAccessViewModel> model = [];
            List<UserAccess> userAccess = await _accountService.GetUserAccesses();
            List<Division> divisions = await _branchService.GetDivisionsAsync();
            foreach (Division division in divisions)
            {
                if(userAccess.Any(ua => ua.UserId == userId && ua.DivisionId == division.Id))
                    foreach (Branch branch in division.Branches)
                    {
                        List<string> ids = userAccess.Where(ua => ua.BranchId == branch.Id).Select(ua => ua.UserId).ToList();
                        List<(string, string)> values = [];
                        ids.ForEach(id => values.Add((id, _userService.GetUserFullNameById(id))));
                        model.Add(new ManagerAccessViewModel()
                        {
                            DivisionId = division.Id,
                            BranchId = branch.Id,
                            StructureName = branch.Name,
                            ManagersIds = values
                        });
                    }
            }

            return View("~/Views/Supervision/ManageAccess.cshtml", model);
        }
        #endregion

        public async Task<JsonResult> GetBranchesSelectList(int divisionId)
        {
            var branches = await _branchService.GetBranchesByDivisionIdAsync(divisionId);
            var selectListItems = branches.Select(branch => new
            {
                branch.Id,
                branch.Name
            }).ToList();

            return Json(new SelectList(selectListItems, "Id", "Name"));
        }


        #region Company

        public async Task<IActionResult> Company()
        {
            List<BranchPanelViewModel> model = [];
            List<Branch> branches = new();
            var userAccesses = await _accountService.GetUserAccesses(User.Identity.Name);
            if (User.IsInRole(NPUR.Supervisor))
            {
                var divisionsIds = userAccesses.Select(ua => ua.DivisionId);
                foreach (int divisionId in divisionsIds)
                {
                    branches.AddRange(await _branchService.GetBranchesWithStudents(divisionId));
                }
            }
            else if(User.IsInRole(NPUR.Manager))
            {
                IEnumerable<int> branchesIds = userAccesses.Select(ua => ua.BranchId ?? 0);
                branches = await _branchService.GetBranchesWithStudents(branchesIds.ToArray());
            }
            else if (User.IsInRole(NPUR.Admin))
            {
                branches = await _branchService.GetBranchesWithStudents();
            }
            else
            {
                return Forbid();
            }
            var companies = await _branchService.GetCompaniesByBranches(branches.Select(b => b.Id).ToList());
            return View("Companies", companies);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCompany(Company company)
        {
            string message;
            if (ModelState.IsValid)
            {
                bool Result = await _branchService.UpdateCompany(company);
                message = Result ? "Updated successfully" : "Error code: #SUCPN54";
            }
            else
            {
                message = "Error code: #MVCPN54";
            }
            return RedirectToAction("Company");
        }
        #endregion

        #region KitRequest
        public async Task<IActionResult> KitRequests()
        {
            List<KitRequest> model = [];
            List<Branch> branches = new();
            var userAccesses = await _accountService.GetUserAccesses(User.Identity.Name);
            if (User.IsInRole(NPUR.Supervisor))
            {
                var divisionsIds = userAccesses.Select(ua => ua.DivisionId);
                foreach (int divisionId in divisionsIds)
                {
                    branches.AddRange(await _branchService.GetBranchesWithStudents(divisionId));
                }
            }
            else if (User.IsInRole(NPUR.Admin))
            {
                branches = await _branchService.GetBranchesWithStudents();
            }
            else if (User.IsInRole(NPUR.Manager))
            {
                IEnumerable<int> branchesIds = userAccesses.Select(ua => ua.BranchId ?? 0);
                branches = await _branchService.GetBranchesWithStudents(branchesIds.ToArray());
            }
            else
            {
                return Forbid();
            }
            var companies = await _branchService.GetCompaniesByBranches(branches.Select(b => b.Id).ToList());
            var requests = await _branchService.GetKitRequestsByCompanies(companies.Select(c => c.Id).ToList());
            ViewBag.Companies = companies;
            return View("KitRequests", requests);
        }

        [HttpGet]
        public async Task<IActionResult> CreateKitRequest(int companyId)
        {
            Company company = await _branchService.GetCompanyByIdAsync(companyId);
            ViewBag.CompanyId = companyId;
            ViewBag.CompanyName = company.Name;
            return PartialView();
        }

        [HttpPost]
        public async Task<string> CreateKitRequest(KitRequest kitRequest)
        {
            string message;
            if (ModelState.IsValid)
            {
                NPUser cuser = await _accountService.GetNPUserAsync(User);
                kitRequest.Created = DateTime.Now;
                kitRequest.Status = RequestStatus.New;
                bool result = await _branchService.CreateKitRequest(kitRequest);
                if (result)
                {
                    kitRequest.Company = await _branchService.GetCompanyByIdAsync(kitRequest.CompanyId);
                    await _branchService.SendNotification(kitRequest);
                }
                message = result ? "Created successfully" : "Error code: #SCKR17";
            }
            else
            {
                message = "Error code: #MVKR17";
            }
            return message;
        }

        public async Task<string> UpdateKitRequestStatus(int requestId, int newStatus)
        {
            string message;
            KitRequest? request = await _branchService.GetKitRequestByIdAsync(requestId);
            if (request != null)
            {
                request.Status = newStatus;
                bool Result = await _branchService.UpdateKitRequest(request);
                message = Result ? "Updated successfully" : "Error code: #SUKR18";
            }
            else
            {
                message = "Error code: #NFKR18";
            }
            return message;
        }
        #endregion

        public async Task<IActionResult> GenerateAgreement(int companyId)
        {
            Company company = await _branchService.GetCompanyByIdAsync(companyId);
            string label = LabelCreator.CreateAgreementLabel(company.Name);
            //check how many agreements with the same label exist in db
            int agreementsCount = await _branchService.GetAgreementsCountByLabel(label);
            Agreement agreement = new()
            {
                Id = LabelCreator.CreateAgreementLabel(company.Name, agreementsCount),
                CompanyId = companyId,
                Created = DateTime.Now
            };
            //save agreement to db
            bool result = await _branchService.CreateAgreement(agreement);
            //return result ? "Created successfully" : "Error code: #SAG17";
            return RedirectToAction("Company");
        }

        public async Task<IActionResult> DownloadAgreement(string aid)
        {
            string fileName = $"agreement_{aid}.docx";
            string path = _branchService.GetAgreementFilePath(aid);
            if (!System.IO.File.Exists(path))
                return NotFound();
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileName
            );
        }
    }
}