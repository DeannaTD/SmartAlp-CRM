using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.Services;
using NewGenBridge.ViewModels;
using System.Security.Policy;
using System.Threading.Tasks;

namespace NewGenBridge.Controllers
{
    public class HubController : Controller
    {
        UserManager<NPUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        IWebHostEnvironment _webHost;
        HubService _hubService;

        public HubController(UserManager<NPUser> userManager, RoleManager<IdentityRole> roleManager, HubService hubService, IWebHostEnvironment webHost)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _hubService = hubService;
            _webHost = webHost;
        }
        public async Task<IActionResult> Index()
        {
            NPUser? user = await _userManager.GetUserAsync(User);
            if (user is null) return Challenge();
            if (User.IsInRole(NPUR.Student)) return Forbid();
            IEnumerable<HubFile> files = [];
            if (User.IsInRole(NPUR.Admin) || User.IsInRole(NPUR.Supervisor))
            {
                files = await _hubService.GetFilesAsync();
            }
            else
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                files = await _hubService.GetAllFilesByRolesAsync(roles.First());
            }
            return View(files);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            if(User.IsInRole(NPUR.Admin) || User.IsInRole(NPUR.Supervisor))
            {
                ViewBag.Roles = new string[] { NPUR.Manager, NPUR.Mentor };
                return View("AddNewFile");
            }
            return Forbid();
            
        }

        [HttpPost]
        public async Task<IActionResult> Upload(CreateHubFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Find current user with UserManager
                NPUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                if(currentUser == null)
                {
                    return Unauthorized();
                }
                HubFile file = new HubFile
                {
                    FileName = model.Name,
                    FileDescription = model.Description,
                    FilePath = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName),
                    UploadedAt = DateTime.Now,
                    UploadedById = currentUser.Id
                };

                await _hubService.UploadFile(file, model.File, [.. model.SelectedRoles]);

                return RedirectToAction("Index");
            }
            return BadRequest();
        }

        public async Task<IActionResult> Download(int id)
        {
            HubFile file = await _hubService.GetByIdAsync(id);
            if (file is null) return NotFound();
            string path = Path.Combine(_webHost.WebRootPath, "Uploads", file.FilePath);
            if (!System.IO.File.Exists(path)) return NotFound();
            string contentType = GetContentType(path);
            string fullFileName = $"{file.FileName}{Path.GetExtension(file.FilePath)}";
            var disposition = new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("inline");
            disposition.SetHttpFileName(fullFileName);
            Response.GetTypedHeaders().ContentDisposition = disposition;
            var fileBytes = await System.IO.File.ReadAllBytesAsync(path);
            return File(fileBytes, contentType);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int fileid)
        {
            HubFile file = await _hubService.GetByIdAsync(fileid);
            List<string> roles = await _hubService.GetRolesByFileIdAsync(fileid);
            ViewBag.Roles = new List<string> { NPUR.Manager, NPUR.Mentor };
            ViewBag.Available = roles;
            return View(file);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, string FileName, string FileDescription, List<string> SelectedRoles, IFormFile? UploadedFile)
        {
            HubFile file = await _hubService.GetByIdAsync(id);
            file.FileName = FileName;
            file.FileDescription = FileDescription;
            await _hubService.Update(file, SelectedRoles, UploadedFile);
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Delete(int fileid)
        {
            HubFile file = await _hubService.GetByIdAsync(fileid);
            if (file == null) return NotFound();
            await _hubService.DeleteFileAsync(file);
            return RedirectToAction("Index");
        }

        #region ContentTypes
        private static readonly Dictionary<string, string> _mimeTypes = new()
        {
            { ".pdf", "application/pdf" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".webp", "image/webp" },
            { ".txt", "text/plain" },
            { ".json", "application/json" },
            { ".zip", "application/zip" },
            { ".rar", "application/vnd.rar" },
            { ".7z", "application/x-7z-compressed" },
        };

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return _mimeTypes.TryGetValue(ext, out var type)
                ? type
                : "application/octet-stream";
        }

        #endregion

        #region KBASE

        public async Task<IActionResult> KnowledgeBaseIndex()
        {
            NPUser? user = await _userManager.GetUserAsync(User);
            if (user is null) return Challenge();
            if (User.IsInRole(NPUR.Student)) return Forbid();
            IEnumerable<HubFile> files = [];
            if (User.IsInRole(NPUR.Admin) || User.IsInRole(NPUR.Supervisor))
            {
                files = await _hubService.GetFilesAsync(true);
            }
            else
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                files = await _hubService.GetAllFilesByRolesAsync(roles.First(), true);
            }
            return View(files);
        }


        [HttpGet]
        public IActionResult UploadKBase()
        {
            if (User.IsInRole(NPUR.Admin) || User.IsInRole(NPUR.Supervisor))
            {
                ViewBag.Roles = new string[] { NPUR.Manager, NPUR.Mentor };
                return View("KnowledgeBaseNew");
            }
            return Forbid();

        }

        [HttpPost]
        public async Task<IActionResult> UploadKBase(CreateKBaseFileViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Find current user with UserManager
                NPUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                if(model.File == null && model.VideoUrl == null)
                {
                    return BadRequest();
                }
                string filePath = "";

                bool isVideo = model.VideoUrl?.Length > 0;
                if (isVideo)
                {
                    filePath = model.VideoUrl;
                }
                else 
                {
                    filePath = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
                }
                HubFile file = new HubFile
                {
                    FileName = model.Name,
                    FileDescription = model.Description,
                    FilePath = filePath,
                    UploadedAt = DateTime.Now,
                    UploadedById = currentUser.Id,
                    IsKBase = true
                };
                if (isVideo) await _hubService.UploadKBaseNoFile(file, [.. model.SelectedRoles]);
                else await _hubService.UploadFile(file, model.File, [.. model.SelectedRoles]);

                return RedirectToAction("KnowledgeBaseIndex");
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> EditKBase(int fileid)
        {
            HubFile file = await _hubService.GetByIdAsync(fileid);
            List<string> roles = await _hubService.GetRolesByFileIdAsync(fileid);
            ViewBag.Roles = new List<string> { NPUR.Manager, NPUR.Mentor };
            ViewBag.Available = roles;
            ViewBag.IsUrl = file.FilePath.StartsWith("http") || file.FilePath.StartsWith("www");
            return View(file);
        }

        [HttpPost]
        public async Task<IActionResult> EditKBase(int id, string FileName, string FileDescription, List<string> SelectedRoles, IFormFile? UploadedFile, string? videoUrl)
        {
            HubFile file = await _hubService.GetByIdAsync(id);
            file.FileName = FileName;
            file.FileDescription = FileDescription;
            if(videoUrl != null)
            {
                await _hubService.UpdateNoFile(file, SelectedRoles, videoUrl);
            }
            else
            {
                await _hubService.Update(file, SelectedRoles, UploadedFile);
            }
            return RedirectToAction("Index");
        }
        #endregion

    }
}
