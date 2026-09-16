using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.Services;
using NewGenBridge.ViewModels;
using QuestPDF.Helpers;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NewGenBridge.Controllers
{
    public class ContractController : Controller
    {
        ContractService _contractService;
        AccountService _accountService;
        BranchService _branchService;
        readonly IWebHostEnvironment _env;
        public ContractController(ContractService contractService, AccountService accountService, BranchService branchService, IWebHostEnvironment env)
        {
            _contractService = contractService;
            _accountService = accountService;
            _branchService = branchService;
            _env = env;
        }

        #region Price
        public async Task<IActionResult> PriceTable()
        {
            //Optimize later (branch to branchId)
            List<Branch> branches = new List<Branch>();
            if (User.IsInRole(NPUR.Admin))
            {
                branches = await _branchService.GetBranchesAsync();
            }
            else if (User.IsInRole(NPUR.Supervisor))
            {
                List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
                foreach (UserAccess access in accesses)
                {
                    if (access.DivisionId != null)
                    {
                        var br = await _branchService.GetBranchesByDivisionIdAsync(access.DivisionId.Value);
                        branches.AddRange(br);
                    }
                }
            }

            List<Price> prices = await _contractService.GetPriceCollectionAsync();
            List<PriceRecordViewModel> model = new List<PriceRecordViewModel>();
            foreach (Branch branch in branches)
            {
                var division = await _branchService.GetDivisionByBranchId(branch.Id);
                var temp = prices.Where(p => p.BranchId == branch.Id);
                var range = temp.Select(p => new PriceRecordViewModel()
                {
                    PriceId = p.Id,
                    BranchId = branch.Id,
                    BranchName = branch.Name,
                    DivisionName = division?.Name,
                    LevelId = p.LevelId,
                    Sum = p.Sum,
                    MaterialPrice = p.MaterialsPrice ?? 0,
                    Duration = p.DurationInMonths
                });
                model.AddRange(range);
            }

            return View(model);
        }

        public async Task<IActionResult> UpdatePrice(List<PriceRecordViewModel> model)
        {
            if (ModelState.IsValid)
            {
                foreach (var record in model)
                {
                    Price price = await _contractService.GetPriceAsync(record.PriceId);
                    price.Sum = record.Sum;
                    price.MaterialsPrice = record.MaterialPrice;
                    price.DurationInMonths = record.Duration;
                    await _contractService.UpdatePriceAsync(price);
                }
            }
            return RedirectToAction("PriceTable");
        }
        #endregion


        #region Contract

        [HttpGet]
        public async Task<IActionResult> Sign()
        {
            List<Division> allDivisions = await _branchService.GetDivisionsAsync();
            List<SelectListItem> branchSelectList = new List<SelectListItem>();
            foreach (var division in allDivisions)
            {
                SelectListGroup group = new SelectListGroup() { Name = division.Name };
                foreach (var branch in division.Branches)
                {
                    branchSelectList.Add(new SelectListItem
                    {
                        Text = $"{division.Name} {branch.Name}",
                        Value = branch.Id.ToString(),
                        Group = group
                    });
                }
            }

            ViewBag.Branches = branchSelectList;
            ViewBag.Prices = await _contractService.GetPriceCollectionAsync();
            return View("ContractSign");
        }

        public string GetContractText(int branchId = 0)
        {
            var text = _contractService.ContractText(branchId);
            var match = Regex.Match(text, @"MAINSTART\{([\s\S]*?)\}MAINEND");
            string extracted = match.Success ? match.Groups[1].Value : "Ошибка: не найден блок MAINSTART{...}MAIN END";
            return extracted;
        }

        [HttpPost]
        public async Task<IActionResult> Sign(ContractSignViewModel model, int DurationInMonth)
        {
            int contractId = 0;
            if (ModelState.IsValid)
            {
                Price price = await _contractService.GetPriceByBranchAndLevel(model.BranchId, model.LevelId);
                string number = $"{model.BranchId}{DateTime.Now.ToString("ddMMyy")}";
                int count = await _contractService.FindContractCount(number);
                number += (count + 1).ToString();
                Contract contract = new Contract
                {
                    BranchId = model.BranchId,
                    SignDate = DateTime.Now,
                    StudentName = $"{model.FirstName} {model.LastName}",
                    ParentName = model.ParentName,
                    PriceId = price.Id,
                    Number = number,
                    PriceAmount = price.Sum
                };
                await _contractService.CreateContractAsync(contract);
                await _contractService.CreateBills(contract, DurationInMonth);
                contractId = contract.Id;
                await _contractService.SaveContractPDF(model, contractId, price.Sum, price.MaterialsPrice ?? 0, DurationInMonth);
                Lead lead = new Lead()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    ParentName = contract.ParentName,
                    PhoneNumber = model.PhoneNumber,
                    LevelId = model.LevelId,
                    BranchId = model.BranchId,
                    ContractId = contractId
                };
                await _contractService.CreateLead(lead);
                return Content(contract.Id.ToString());
            }

            return BadRequest();
        }

        public IActionResult DownloadContract(string id)
        {
            var filePath = Path.Combine(_env.WebRootPath, "Contracts", $"contract_{id}.pdf");
            if (!System.IO.File.Exists(filePath))
                return BadRequest();

            var fileName = Path.GetFileName(filePath);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }

        public async Task<IActionResult> ContractTable(int page = 1, string name = "")
        {
            int pagesize = 20;
            //Optimize later (branch to branchId)
            List<Branch> branches = new List<Branch>();
            if (User.IsInRole(NPUR.Admin))
            {
                branches = await _branchService.GetBranchesAsync();
            }
            else if (User.IsInRole(NPUR.Supervisor))
            {
                List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
                foreach (UserAccess access in accesses)
                {
                    if (access.DivisionId != null)
                    {
                        var br = await _branchService.GetBranchesByDivisionIdAsync(access.DivisionId.Value);
                        branches.AddRange(br);
                    }
                }
            }
            else if (User.IsInRole(NPUR.Manager))
            {
                List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
                foreach (UserAccess access in accesses)
                {
                    if (access.BranchId != null)
                    {
                        var br = await _branchService.GetBranchByIdAsync(access.BranchId.Value);
                        branches.Add(br);
                    }
                }
            }
            ViewBag.BranchIds = branches.Select(b => b.Id).ToList();
            var contracts = await _contractService.GetContractCollectionAsync();

            List<Contract> model = new List<Contract>();
            foreach (var branch in branches)
            {
                var branchContracts = contracts.Where(c => c.BranchId == branch.Id).ToList();
                model.AddRange(branchContracts);
            }
            ViewBag.NamesList = model.Select(c => new { c.StudentName }).DistinctBy(c => c.StudentName).ToList();
            if (name != "")
            {
                model = model.Where(c => c.StudentName == name).ToList();
                ViewBag.Name = name;
            }
            else
            {
                ViewBag.Name = "";
            }

            ViewBag.TotalPages = (int)Math.Ceiling((double)model.Count / pagesize);
            model = model.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            ViewBag.CurrentPage = page;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ContractTablePartial", model);
            }
            return View(model);
        }
        #endregion

        #region Bills

        public async Task<IActionResult> BillTable(int page = 1, string name = "")
        {
            int pageSize = 20;
            //Optimize later (branch to branchId)
            List<Branch> branches = new List<Branch>();
            if (User.IsInRole(NPUR.Admin))
            {
                branches = await _branchService.GetBranchesAsync();
            }
            else if (User.IsInRole(NPUR.Supervisor))
            {
                List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
                foreach (UserAccess access in accesses)
                {
                    if (access.DivisionId != null)
                    {
                        var br = await _branchService.GetBranchesByDivisionIdAsync(access.DivisionId.Value);
                        branches.AddRange(br);
                    }
                }
            }
            else if (User.IsInRole(NPUR.Manager))
            {
                List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
                foreach (UserAccess access in accesses)
                {
                    if (access.BranchId != null)
                    {
                        var br = await _branchService.GetBranchByIdAsync(access.BranchId.Value);
                        branches.Add(br);
                    }
                }
            }
            //TODO: NEEDS A LOT OF OPTIMIZATION!!!
            var contracts = await _contractService.GetContractCollectionAsync();
            contracts = contracts.Where(c => branches.Any(b => b.Id == c.BranchId)).ToList();

            ViewBag.NamesList = contracts.Select(c => new { c.StudentName }).DistinctBy(c => c.StudentName).ToList();

            List<BillViewModel> model = new List<BillViewModel>();
            foreach (var contract in contracts)
            {
                var bills = await _contractService.GetBillsByContractIdAsync(contract.Id);
                model.AddRange(bills.Select(b => new BillViewModel
                {
                    PaymentNumber = b.BillNumber,
                    ContractNumber = contract.Number,
                    StudentName = contract.StudentName,
                    Sum = b.Sum,
                    PaymentDate = b.DueDate ?? DateTime.MinValue,
                    Status = b.Status,
                    BillId = b.Id,
                    HasFile = Directory.EnumerateFiles(Path.Combine(_env.WebRootPath, "Bills"), b.BillNumber + ".*").Any()
                }));
            }

            if (name != "")
            {
                model = model.Where(c => c.StudentName == name).ToList();
                ViewBag.Name = name;
            }
            else
            {
                ViewBag.Name = "";
            }

            ViewBag.TotalPages = (int)Math.Ceiling((double)model.Count / pageSize);
            model = model.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_BillTablePartial", model);
            }

            return View(model);
        }

        public async Task<IActionResult> UpdateBillStatus(int billId, bool status)
        {
            Bill bill = await _contractService.GetBillByIdAsync(billId);
            if (bill is not null)
            {
                bill.Status = status;
                await _contractService.UpdateBillAsync(bill);
            }
            return Ok();
        }
        #endregion

        public IActionResult ContractEdit(int branchId)
        {
            string contractName = $"contract_{branchId}.txt";
            var filePath = System.IO.Path.Combine(_env.WebRootPath, "Contracts", contractName);
            if (!System.IO.File.Exists(filePath))
            {
                _contractService.CreateContractText(branchId);
            }
            string text = _contractService.ContractText(branchId);
            ContractEditViewModel model = new ContractEditViewModel
            {
                BranchId = branchId,
                HeadSegment = Regex.Match(text, @"HEADSTART\{([\s\S]*?)\}HEADEND").Groups[1].Value,
                MainSegment = Regex.Match(text, @"MAINSTART\{([\s\S]*?)\}MAINEND").Groups[1].Value,
                FooterSegment = Regex.Match(text, @"FOOTERSTART\{([\s\S]*?)\}FOOTEREND").Groups[1].Value
            };
            return View(model);
        }

        public IActionResult SaveContractEdit(ContractEditViewModel model)
        {
            string contractName = $"contract_{model.BranchId}.txt";
            var filePath = System.IO.Path.Combine(_env.WebRootPath, "Contracts", contractName);
            string text = $"HEADSTART{{{model.HeadSegment}}}HEADEND\nMAINSTART{{{model.MainSegment}}}MAINEND\nFOOTERSTART{{{model.FooterSegment}}}FOOTEREND";
            System.IO.File.WriteAllText(filePath, text);
            return RedirectToAction("ContractEdit", new { branchId = model.BranchId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await _contractService.DeleteContractById(id);
            return RedirectToAction("ContractTable");
        }

        public async Task<IActionResult> BillDownload(string paymentNumber)
        {
            string folderPath = Path.Combine(_env.WebRootPath, "Bills");
            string path = Directory.GetFiles(folderPath, paymentNumber + ".*").FirstOrDefault();
            if (!System.IO.File.Exists(path)) return NotFound();
            string contentType = GetContentType(path);
            if (contentType == "NOTVALID") return BadRequest();
            string fullFileName = $"{paymentNumber}{Path.GetExtension(path)}";
            Response.Headers.ContentDisposition = $"inline; filename=\"{fullFileName}\"";
            var fileBytes = await System.IO.File.ReadAllBytesAsync(path);
            return File(fileBytes, contentType);
        }

        public async Task<IActionResult> BillUpload(IFormFile file, string paymentNumber)
        {
            if (file is null || file.Length == 0)
                return BadRequest("No file selected.");
            string uploadsFolder = Path.Combine(_env.WebRootPath, "Bills");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string fileExtension = Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, $"{paymentNumber}{fileExtension}");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return Ok();
        }


        #region ContentTypes
        private static readonly Dictionary<string, string> _mimeTypes = new()
        {
            { ".pdf", "application/pdf" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" }
        };

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return _mimeTypes.TryGetValue(ext, out var type)
                ? type
                : "NOTVALID";
        }

        #endregion


        public async Task<IActionResult> NewStudents(int page = 1, string name = "")
        {
            int pageSize = 20;
            List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
            IEnumerable<int?> userAccesses = accesses.Select(a => a.BranchId).Where(a => a is not null);
            List<Lead> model = await _contractService.GetLeadsByBranchIds(userAccesses.ToList());
            if (name != "")
            {
                model = model.Where(s => $"{s.FirstName} {s.LastName}".ToLower().Contains(name.ToLower())).ToList();
                ViewBag.Name = name;
            }
            List<SelectListItem> groupList = new List<SelectListItem>();
            foreach (int? branchId in userAccesses)
            {
                Branch branch = await _branchService.GetBranchByIdAsync(branchId ?? 0);
                List<Models.Group> groups = _branchService.GetGroupsByBranchId(branchId ?? 0);
                groupList.AddRange(groups.Select(
                    g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = branch.Name + ", " + g.Name + " " + g.Day + " " + g.Time.ToString("hh\\:mm")
                    }));
            }

            ViewBag.GroupList = groupList;
            ViewBag.BranchesId = userAccesses;
            ViewBag.TotalPages = (int)Math.Ceiling((double)model.Count / pageSize);
            ViewBag.NamesList = model.Select(c => new { FullName = $"{c.FirstName} {c.LastName}" }).Distinct().ToList();
            model = model.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_NewStudentsPartial", model);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewStudentFromContractAsync(StudentFromContractViewModel model)
        {
            if (ModelState.IsValid)
            {
                Contract contract = _contractService.GetContractById(model.ContractId);
                StudentRegistrationViewModel student = new StudentRegistrationViewModel()
                {
                    ParentName = model.ParentName,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
                    StudentPhoneNumber = model.PhoneNumber,
                    DateStarted = DateOnly.FromDateTime(DateTime.Now),
                    PaymentAmount = contract.Price.Sum,
                    Note = "",
                    GroupId = model.GroupId
                };
                await _accountService.RegisterAsync(student);
                //remove lead
                Lead lead = await _contractService.GetLeadById(model.LeadId);
                if (lead != null)
                {
                    await _contractService.DeleteLeadAsync(lead);
                }
                return Ok();
            }

            return BadRequest();
        }

        public async Task<IActionResult> DeleteLead(int leadId)
        {
            Lead lead = await _contractService.GetLeadById(leadId);
            if (lead != null)
            {
                await _contractService.DeleteLeadAsync(lead);
                return Ok();
            }
            return NotFound();
        }

        public async Task<IActionResult> RoyaltyTable(int page = 1)
        {
            int pageSize = 20;
            //Optimize later (branch to branchId)
            List<Branch> branches = new List<Branch>();
            if (User.IsInRole(NPUR.Admin))
            {
                branches = await _branchService.GetBranchesAsync();
            }
            else if (User.IsInRole(NPUR.Supervisor))
            {
                List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
                foreach (UserAccess access in accesses)
                {
                    if (access.DivisionId != null)
                    {
                        var br = await _branchService.GetBranchesByDivisionIdAsync(access.DivisionId.Value);
                        branches.AddRange(br);
                    }
                }
            }
            else if (User.IsInRole(NPUR.Manager))
            {
                List<UserAccess> accesses = await _accountService.GetUserAccesses(User.Identity.Name);
                foreach (UserAccess access in accesses)
                {
                    if (access.BranchId != null)
                    {
                        var br = await _branchService.GetBranchByIdAsync(access.BranchId.Value);
                        branches.Add(br);
                    }
                }
            }
            //TODO: NEEDS A LOT OF OPTIMIZATION!!!
            var contracts = await _contractService.GetContractCollectionAsync(true);
            List<Royalty> royalties = [];
            //get all bills and group them by branch and period, sum them and multiply by percentage, then create royalty records
            foreach (var branch in branches)
            {
                var branchContracts = contracts.Where(c => c.BranchId == branch.Id).ToList();
                var bills = branchContracts.SelectMany(c => c.Bills);
                //sum all the bills by month of DueDate
                var rtmp = bills.GroupBy(b => new { b.DueDate.Value.Year, b.DueDate.Value.Month })
                .Select(g => new Royalty
                {
                    Total = g.Sum(b => b.Sum) * 0.05m,
                    BranchId = branch.Id,
                    Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                }).ToList();
                royalties.AddRange(rtmp);
            }

            ViewBag.TotalPages = (int)Math.Ceiling((double)royalties.Count / pageSize);
            royalties = royalties.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.Branches = branches;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_RoyaltyTablePartial", royalties);
            }
            return View(royalties);
        }
    }
}