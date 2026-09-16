using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.UoW;
using NewGenBridge.ViewModels;
using QuestPDF.Infrastructure;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace NewGenBridge.Services
{
    public class BranchService
    {
        UnitOfWork _unitOfWork;
        IWebHostEnvironment _webHost;
        public BranchService(UnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _webHost = webHost;
        }

        public async Task<bool> CreateAsync(Branch model)
        {
            bool isSuccessful = false;
            if (model is not null)
            {
                await _unitOfWork.Branches.CreateAsync(model);
                int numberOfChanges = await _unitOfWork.Complete();
                isSuccessful = numberOfChanges > 0;

                await _unitOfWork.Companies.CreateAsync(new Company()
                {
                    BranchId = model.Id,
                    Name = model.Name,
                    ExecutiveName = "",
                    PhoneNumber = "",
                    Email = "",
                    TaxId = "",
                    BackAccount = "",
                    BankName = "",
                    BankId = ""
                });
                numberOfChanges = await _unitOfWork.Complete();
                isSuccessful = numberOfChanges > 0;
            }
            return isSuccessful;
        }

        public async Task<bool> CreateGroupAsync(GroupCreationViewModel model)
        {
            bool isSuccessful = false;
            Group newGroup = new Group
            {
                Day = model.Day,
                Time = model.Time,
                BranchId = model.BranchId,
                MentorId = model.MentorId,
                Name = model.Name ?? ""
            };
            await _unitOfWork.Groups.CreateAsync(newGroup);
            int numberOfChanges = await _unitOfWork.Complete();
            isSuccessful = numberOfChanges > 0;

            return isSuccessful;
        }

        public async Task<bool> CreateDivision(Division division)
        {
            var divisions = await _unitOfWork.Divisions.GetCollectionAsync();
            if (divisions.Any(d => d.Name == division.Name))
                return false;
            await _unitOfWork.Divisions.CreateAsync(division);
            int numberOfChanges = await _unitOfWork.Complete();

            return numberOfChanges > 0;
        }

        public async Task<bool> UpdateBranch(Branch branch)
        {
            _unitOfWork.Branches.Update(branch);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<bool> UpdateGroup(Group group)
        {
            _unitOfWork.Groups.Update(group);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<bool> DeleteBranch(Branch branch)
        {
            _unitOfWork.Branches.Delete(branch);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<Branch> GetBranchByIdAsync(int branchId)
        {
            return await _unitOfWork.Branches.GetByIdAsync(branchId);
        }
        public async Task<List<Branch>> GetBranchesAsync()
        {
            return await _unitOfWork.Branches.GetCollectionAsync();
        }

        public async Task<List<Branch>> GetBranchesAndGroupsByMentorAsync(string mentorId)
        {
            List<Branch> branches = await _unitOfWork.Branches.GetCollectionAsync();
            List<Branch> result = [];
            foreach (Branch branch in branches)
            {
                if (branch.Groups.Any(group => group.MentorId == mentorId))
                {
                    result.Add(new Branch()
                    {
                        Id = branch.Id,
                        Name = branch.Name,
                        Groups = branch.Groups.Where(group => group.MentorId == mentorId).ToList()
                    });
                }
            }
            return result;
        }

        public async Task<List<Branch>> GetBranchesWithStudents()
        {
            return await _unitOfWork.Branches.GetFullCollectionAsync();
        }

        public async Task<List<Branch>> GetBranchesWithStudents(int divisionId)
        {
            return await _unitOfWork.Branches.GetFullCollectionAsync(divisionId);
        }

        public async Task<List<Branch>> GetBranchesWithStudents(int[] branchIds)
        {
            return await _unitOfWork.Branches.GetFullCollectionAsync(branchIds);
        }

        public async Task<List<Branch>> GetBranchesByDivisionIdAsync(int divisionId)
        {
            return await _unitOfWork.Branches.GetCollectionAsync(divisionId);
        }
        public async Task<Group> GetGroupByIdAsync(int groupId)
        {
            return await _unitOfWork.Groups.GetByIdAsync(groupId);
        }

        public async Task<List<Division>> GetDivisionsAsync()
        {
            return await _unitOfWork.Divisions.GetCollectionAsync();
        }

        public async Task<Division> GetDivisionByIdAsync(int divisionId)
        {
            return await _unitOfWork.Divisions.GetByIdAsync(divisionId);
        }

        public async Task<Division> GetDivisionByIdWidthBranchesAsync(int divisionId)
        {
            return await _unitOfWork.Divisions.GetByIdWithBranches(divisionId);
        }

        public async Task<Division> GetDivisionByBranchId(int branchId)
        {
            var disisions = await _unitOfWork.Divisions.GetCollectionAsync();
            return disisions.FirstOrDefault(d => d.Branches.Any(b => b.Id == branchId));
        }
        public async Task<bool> UpdateDivision(Division division)
        {
            _unitOfWork.Divisions.Update(division);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<bool> DeleteDivision(Division division)
        {
            _unitOfWork.Divisions.Delete(division);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<List<Division>> GetDivisionsOnlyAsync()
        {
            return await _unitOfWork.Divisions.GetCleanDivisionsAsync();
        }

        public async Task<UserData> GetGroupMentorData(int groupId)
        {
            Group group = await _unitOfWork.Groups.GetByIdAsync(groupId);
            NPUser mentor = await _unitOfWork.Users.GetByIdAsync(group.MentorId);
            return new UserData
            {
                Id = mentor.Id,
                UserName = mentor.UserName,
                FirstName = mentor.FirstName,
                LastName = mentor.LastName
            };
        }

        public List<Group> GetGroupsByBranchId(int branchId)
        {
            return _unitOfWork.Groups.GetByBranchId(branchId).ToList();
        }

        public async Task<List<Company>> GetCompaniesByBranches(List<int> branchIds)
        {
            var companies = await _unitOfWork.Companies.GetCollectionAsync();
            bool isAnyBranchWithoutCompany = branchIds.Any(bid => companies.All(c => c.BranchId != bid));
            if (isAnyBranchWithoutCompany)
            {
                foreach (var bid in branchIds)
                {
                    if (companies.All(c => c.BranchId != bid))
                    {
                        //create a company for this branch
                        Branch branch = await _unitOfWork.Branches.GetByIdAsync(bid);
                        await _unitOfWork.Companies.CreateAsync(new Company()
                        {
                            BranchId = bid,
                            Name = branch.Name,
                            ExecutiveName = "",
                            PhoneNumber = "",
                            Email = "",
                            TaxId = "",
                            BackAccount = "",
                            BankName = "",
                            BankId = ""
                        });
                    }
                }
                await _unitOfWork.Complete();
            }
            companies = await _unitOfWork.Companies.GetCollectionAsync();
            return companies.Where(c => branchIds.Contains(c.BranchId)).ToList();
        }

        public async Task<bool> UpdateCompany(Company company)
        {
            _unitOfWork.Companies.Update(company);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<List<KitRequest>> GetKitRequestsByCompanies(List<int> companyIds)
        {
            var kitRequests = await _unitOfWork.KitRequests.GetCollectionAsync();
            return kitRequests.Where(kr => companyIds.Contains(kr.CompanyId)).ToList();
        }

        public async Task<Company> GetCompanyByIdAsync(int companyId)
        {
            var companies = await _unitOfWork.Companies.GetCollectionAsync();
            return companies.FirstOrDefault(c => c.Id == companyId);
        }

        public async Task<bool> CreateKitRequest(KitRequest kitRequest)
        {
            await _unitOfWork.KitRequests.CreateAsync(kitRequest);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<KitRequest> GetKitRequestByIdAsync(int kitRequestId)
        {
            return await _unitOfWork.KitRequests.GetByIdAsync(kitRequestId);
        }

        public async Task<bool> UpdateKitRequest(KitRequest kitRequest)
        {
            _unitOfWork.KitRequests.Update(kitRequest);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        //move to mail service
        public async Task<string> SendNotification(KitRequest request)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("info@jorgosmart.com", "tssquibivflqlghg"),
                EnableSsl = true,
            };
            string path = Path.Combine(_webHost.WebRootPath, "Helpers", "LeadEmailTemplate.html");
            string body = File.ReadAllText(path);
            body = body.Replace("{{COMPANY_NAME}}", request.Company.Name);
            body = body.Replace("{{EXECUTIVE_NAME}}", request.Company.ExecutiveName);
            body = body.Replace("{{PHONE_NUMBER}}", request.Company.PhoneNumber);
            body = body.Replace("{{EMAIL}}", request.Company.Email);
            body = body.Replace("{{TAX_ID}}", request.Company.TaxId);
            body = body.Replace("{{BANK_ACCOUNT}}", request.Company.BackAccount);
            body = body.Replace("{{BANK_NAME}}", request.Company.BankName);
            body = body.Replace("{{BANK_ID}}", request.Company.BankId);
            body = body.Replace("{{NUMBER_OF_KITS}}", request.Amount.ToString());
            body = body.Replace("{{CREATED_BY}}", request.CreatedById);
            body = body.Replace("{{CREATED_AT}}", request.Created.ToString("g"));
            var message = new MailMessage
            {
                From = new MailAddress("info@jorgosmart.com"),
                Subject = "New request for kits",
                Body = body,
                IsBodyHtml = true
            };
            message.To.Add("azamat.r@gmail.com");
            message.To.Add("damira.marketing.player@gmail.com");
            message.To.Add("snehal@obotz.com");
            //message.To.Add("tdboodman409@gmail.com");
            try
            {
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                return $"Error sending email: {ex.Message}";
            }
            return "Email sent successfully";
        }

        private void TempAgreement(Agreement agreement, Company company)
        {
            string path = Path.Combine(_webHost.WebRootPath, "Helpers", "agreement_temp.docx");
            var doc = DocX.Load(path);
            var replacements = new Dictionary<string, string>
            {
                { "{{AGI}}", agreement.Id },
                { "{{CMN}}", company.Name },
                { "{{CRD}}", DateTime.Now.ToString(@"dd/MM/yyyy") },
                { "{{CME}}", company.ExecutiveName },
                { "{{CMA}}", company.Address ?? "" },
                { "{{CMM}}", company.Email },
                { "{{CMP}}", company.PhoneNumber },
                { "{{CRF}}", DateTime.Now.AddYears(5).ToString(@"dd/MM/yyyy") }
            };
            foreach (var item in replacements)
            {
                doc.ReplaceText(new StringReplaceTextOptions
                {
                    SearchValue = item.Key,
                    NewValue = item.Value
                });
            }

            doc.SaveAs(Path.Combine(_webHost.WebRootPath, "Contracts", $"agreement_{agreement.Id}.docx"));
        }

        public async Task<bool> CreateAgreement(Agreement agreement)
        {
            Company company = await _unitOfWork.Companies.GetByIdAsync(agreement.CompanyId);
            try
            {
                TempAgreement(agreement, company);
            }
            catch (Exception ex)
            {
                return false;
            }
            await _unitOfWork.Agreements.CreateAsync(agreement);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<int> GetAgreementsCountByLabel(string label)
        {
            var agreements = await _unitOfWork.Agreements.GetCollectionAsync();
            int count = agreements.Count(a => a.Id.StartsWith(label));
            return count;
        }

        public string GetAgreementFilePath(string agreementId)
        {
            string fileName = $"agreement_{agreementId}.docx";
            string path = Path.Combine(_webHost.WebRootPath, "Contracts", fileName);
            if (System.IO.File.Exists(path))
                return path;
            return "";
        }
    }
            
}