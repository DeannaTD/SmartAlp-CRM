using Microsoft.CodeAnalysis.Operations;
using NewGenBridge.Models;
using NewGenBridge.UoW;
using NewGenBridge.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SixLabors.ImageSharp.Drawing;
using System.Globalization;

namespace NewGenBridge.Services
{
    public class ContractService
    {
        UnitOfWork _unitOfWork;
        readonly IWebHostEnvironment _env;

        public ContractService(UnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public Contract GetContractById(int contractId)
        {
            return _unitOfWork.Contracts.GetContractById(contractId);
        }

        public async Task<List<Price>> GetPriceCollectionAsync()
        {
            return await _unitOfWork.Prices.GetCollectionAsync();
        }

        public async Task<Price> GetPriceAsync(int priceId)
        {
            return await _unitOfWork.Prices.GetByIdAsync(priceId);
        }

        public async Task<decimal> GetPriceSum(int priceId)
        {
            var price = await _unitOfWork.Prices.GetByIdAsync(priceId);
            return price.Sum;
        }
        //todo: add try-catch
        public async Task<bool> AddPricesToNewBranchAsync(int branchId)
        {
            var levels = await _unitOfWork.Levels.GetCollectionAsync();
            foreach (var level in levels)
            {
                Price price = new Price
                {
                    BranchId = branchId,
                    LevelId = level.Id,
                    Sum = 0
                };
                await _unitOfWork.Prices.CreateAsync(price);
            }
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> AddPricesOfNewLevel(int levelId)
        {
            var branches = await _unitOfWork.Branches.GetCollectionAsync();
            foreach (var branch in branches)
            {
                Price price = new Price
                {
                    BranchId = branch.Id,
                    LevelId = levelId,
                    Sum = 0
                };
                await _unitOfWork.Prices.CreateAsync(price);
            }
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<bool> UpdatePriceAsync(Price price)
        {
            _unitOfWork.Prices.Update(price);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<Price> GetPriceByBranchAndLevel(int branchId, int levelId)
        {
            var prices = await _unitOfWork.Prices.GetCollectionAsync();
            return prices.FirstOrDefault(p => p.BranchId == branchId && p.LevelId == levelId);
        }

        public async Task<List<Contract>> GetContractCollectionAsync(bool includeBills = false)
        {
            if (includeBills)
            {
                return _unitOfWork.Contracts.GetContractsWithBills();
            }
            return await _unitOfWork.Contracts.GetCollectionAsync();
        }

        public async Task<int> CreateContractAsync(Contract contract)
        {
            await _unitOfWork.Contracts.CreateAsync(contract);
            return await _unitOfWork.Complete();
        }

        public async Task<int> FindContractCount(string number)
        {
            var contracts = await _unitOfWork.Contracts.GetCollectionAsync();
            return contracts.Count(c => c.Number.StartsWith(number));
        }

        public async Task<int> CreateBills(int contractId, int count = 6)
        {
            Contract contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
            for (int i = 0; i < count; i++)
            {
                Bill bill = new Bill
                {
                    ContractId = contractId,
                    Status = false,
                    BillNumber = $"{contract.Number}-{i + 1}"
                };
                await _unitOfWork.Bills.CreateAsync(bill);
            }
            return await _unitOfWork.Complete();
        }

        public async Task<int> CreateBills(Contract contract, int count = 6)
        {
            for (int i = 0; i < count; i++)
            {
                Bill bill = new Bill
                {
                    ContractId = contract.Id,
                    Status = false,
                    BillNumber = $"{contract.Number}-{i + 1}",
                    Sum = contract.Price.Sum,
                    DueDate = DateTime.Now.AddMonths(i)
                };
                await _unitOfWork.Bills.CreateAsync(bill);
            }

            if (contract.Price.MaterialsPrice is > 0)
            {
                Bill materialBill = new Bill
                {
                    ContractId = contract.Id,
                    Status = false,
                    BillNumber = $"{contract.Number}-M",
                    Sum = contract.Price.MaterialsPrice ?? 0,
                    DueDate = DateTime.Now
                };
                await _unitOfWork.Bills.CreateAsync(materialBill);
            }
            return await _unitOfWork.Complete();
        }

        public async Task<List<Bill>> GetBillsByContractIdAsync(int contractId)
        {
            var bills = await _unitOfWork.Bills.GetCollectionAsync();
            return bills.Where(b => b.ContractId == contractId).ToList();
        }

        public async Task<Bill> GetBillByIdAsync(int billId)
        {
            return await _unitOfWork.Bills.GetByIdAsync(billId);
        }

        public async Task<bool> UpdateBillAsync(Bill bill)
        {
            _unitOfWork.Bills.Update(bill);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public string ContractText(int branchId = 0)
        {
            string filePath = System.IO.Path.Combine(_env.WebRootPath, "Contracts", branchId == 0 ? "contract.txt" : $"contract_{branchId}.txt");
            if (!System.IO.File.Exists(filePath))
            {
                filePath = System.IO.Path.Combine(_env.WebRootPath, "Contracts", "contract.txt");
            }
            string text = System.IO.File.ReadAllText(filePath);
            return text;
        }

        public bool CreateContractText(int branchId)
        {
            var filePath = System.IO.Path.Combine(_env.WebRootPath, "Contracts", $"contract_{branchId}.txt");
            if (!System.IO.File.Exists(filePath))
            {
                var defaultFilePath = System.IO.Path.Combine(_env.WebRootPath, "Contracts", "contract.txt");
                System.IO.File.Copy(defaultFilePath, filePath);
                return true;
            }
            else return false;
        }

        //TODO: dry
        public async Task<string> GetBranchNameById(int? branchId)
        {
            if (branchId is null)
            {
                return string.Empty;
            }
            var branch = await _unitOfWork.Branches.GetByIdAsync(branchId);
            return branch.Name;
        }

        public async Task SaveContractPDF(ContractSignViewModel data, int contractId, decimal sum, decimal materials, int months = 6)
        {
            string contractTemplate = ContractText(data.BranchId);
            string branchName = await GetBranchNameById(data.BranchId);
            string contractText = contractTemplate
                .Replace("{ParentName}", data.ParentName)
                .Replace("{FirstName}", data.FirstName)
                .Replace("{LastName}", data.LastName)
                .Replace("{DateOfBirth}", data.DateOfBirth.ToString("dd.MM.yyyy"))
                .Replace("{PhoneNumber}", data.PhoneNumber)
                .Replace("{Address}", data.Address)
                .Replace("{SignDate}", DateTime.Now.ToString("dd.MM.yyyy"))
                .Replace("{MonthlyPrice}", sum.ToString("F2", CultureInfo.InvariantCulture))
                .Replace("{MaterialsPrice}", materials.ToString("F2", CultureInfo.InvariantCulture))
                .Replace("{BranchName}", branchName)
                .Replace("{ContractDateBegin}", DateTime.Now.ToString("dd.MM.yyyy"))
                .Replace("{ContractDateEnd}", DateTime.Now.AddMonths(months).ToString("dd.MM.yyyy"))
                .Replace("HEADSTART{", "")
                .Replace("}HEADEND", "")
                .Replace("MAINSTART{", "")
                .Replace("}MAINEND", "")
                .Replace("FOOTERSTART{", "")
                .Replace("}FOOTEREND", "");

            string fileName = $"contract_{contractId}.pdf";
            string filePath = System.IO.Path.Combine(_env.WebRootPath, "Contracts");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            string outputPath = System.IO.Path.Combine(filePath, fileName);
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);
                    page.Content().Text(contractText).FontSize(12);
                });
            });
            try
            {
                pdf.GeneratePdf(outputPath);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log the error)
                string b = ex.Message;
            }

            return;
        }

        public async Task<bool> DeleteContractById(int contractId)
        {
            Contract contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
            if (contract is null)
            {
                return false;
            }
            var bills = await _unitOfWork.Bills.GetCollectionAsync();
            bills = bills.Where(b => b.ContractId == contractId).ToList();
            foreach (var bill in bills)
            {
                _unitOfWork.Bills.Delete(bill);
            }
            _unitOfWork.Contracts.Delete(contract);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task CreateLead(Lead lead)
        {
            await _unitOfWork.Leads.CreateAsync(lead);
            await _unitOfWork.Complete();
        }

        public async Task<List<Lead>> GetLeadsByBranchIds(List<int?> branchIds)
        {
            var leads = await _unitOfWork.Leads.GetCollectionAsync();
            return leads.Where(l => branchIds.Contains(l.BranchId)).ToList();
        }

        public async Task<Lead> GetLeadById(int leadId)
        {
            return await _unitOfWork.Leads.GetByIdAsync(leadId);
        }

        public async Task<bool> DeleteLeadAsync(Lead lead)
        {
            _unitOfWork.Leads.Delete(lead);
            int numberOfChanges = await _unitOfWork.Complete();
            return numberOfChanges > 0;
        }

        public async Task<List<Bill>> GetBillWithContracts()
        {
            var bills = await _unitOfWork.Bills.GetFullBillsAsync();
            return bills;
        }
    }
}
