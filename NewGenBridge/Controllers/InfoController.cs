using Microsoft.AspNetCore.Mvc;
using NewGenBridge.Data;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NewGenBridge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InfoController : Controller
    {
        IWebHostEnvironment _webHost;
        ApplicationDbContext _context;
        public InfoController(IWebHostEnvironment webHost, ApplicationDbContext context)
        {
            _webHost = webHost;
            _context = context;
        }

        public async Task<IActionResult> GetInfo()
        {
            var filePath = Path.Combine(_webHost.ContentRootPath, "Data", "api_branches.json");
            var jsonData = await System.IO.File.ReadAllTextAsync(filePath);
            var apiData = JsonSerializer.Deserialize<Helpers.ApiData>(jsonData);
            RawApiData response = new RawApiData();
            response.Countries = new List<RawCountry>();
            foreach(var country in apiData.Countries)
            {
                RawCountry item = new RawCountry();
                item.Code = country.Code;
                item.Info = new List<RawCountryInfo>();
                foreach(var info in country.Info)
                {
                    Models.Group group = _context.Groups.FirstOrDefault(g => g.Id == info.GroupId);
                    Branch branch = _context.Branches.FirstOrDefault(b => b.Id == group.BranchId);
                    Models.Level level = _context.Levels.FirstOrDefault(l => l.Id == info.LevelId);
                    RawCountryInfo infoItem = new RawCountryInfo();
                    infoItem.Address = branch.Address;
                    infoItem.Day = $"{group.Day.ToString().ToLower()}";
                    infoItem.Time = group.Time.ToString(@"hh\:mm");
                    infoItem.LevelCode = level.Id.ToString();
                    item.Info.Add(infoItem);
                }
                response.Countries.Add(item);
            }
            return Ok(response);
        }
    }
}
