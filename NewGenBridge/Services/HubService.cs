using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.UoW;
using NewGenBridge.ViewModels;
using System.ComponentModel;

namespace NewGenBridge.Services
{
    public class HubService
    {
        UnitOfWork _unitOfWork;
        IWebHostEnvironment _webHost;

        public HubService(UnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _webHost = webHost;
        }

        public async Task<bool> UploadFile(HubFile file, IFormFile formFile, List<string> roles)
        {
            //file name in system is 
            await _unitOfWork.HubFiles.CreateAsync(file);
            await _unitOfWork.Complete();
            // Save the file to the server
            using (var stream = new FileStream(Path.Combine(_webHost.WebRootPath, "Uploads", file.FilePath), FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }
            await SaveFileAccess(file.Id, roles);
            return true;
        }

        public async Task<bool> UploadKBaseNoFile(HubFile file, List<string> roles)
        {
            //file name in system is 
            await _unitOfWork.HubFiles.CreateAsync(file);
            await _unitOfWork.Complete();
            await SaveFileAccess(file.Id, roles);
            return true;
        }

        private async Task<bool> SaveFileAccess(int fileId, List<string> roles)
        {
            foreach (string role in roles)
            {
                HubFileRole access = new HubFileRole
                {
                    FileId = fileId,
                    RoleName = role
                };
                await _unitOfWork.HubFileRoles.CreateAsync(access);
            }
            await _unitOfWork.Complete();
            return true;
        }

        public async Task<IEnumerable<HubFile>> GetAllFilesByRolesAsync(string roleName, bool isKBase = false)
        {
            var hubFiles = await _unitOfWork.HubFiles.GetAllFilesByRoleAsync(roleName);
            return hubFiles.Where(f => (f.IsKBase ?? false) == isKBase);
        }

        public async Task<IEnumerable<HubFile>> GetFilesAsync(bool isKBase = false)
        {
            var hubFiles = await _unitOfWork.HubFiles.GetCollectionAsync();
            return hubFiles.Where(f => (f.IsKBase ?? false) == isKBase);
        }

        public async Task<HubFile> GetByIdAsync(int id)
        {
            return await _unitOfWork.HubFiles.GetByIdAsync(id);
        }

        public async Task<List<string>> GetRolesByFileIdAsync(int fileId)
        {
            var roles = await _unitOfWork.HubFileRoles.GetRolesByFileIdAsync(fileId);
            return [.. roles.Select(r => r.RoleName)];
        }

        public async Task Update(HubFile file, List<string> roles, IFormFile? formFile)
        {
            var existingRoles = await _unitOfWork.HubFileRoles.GetRolesByFileIdAsync(file.Id);
            //if role from roles not in existingRoles, add it
            //if role from existingRoles not in roles, remove it
            foreach (string role in roles)
            {
                if (!existingRoles.Any(r => r.RoleName == role))
                {
                    HubFileRole access = new HubFileRole
                    {
                        FileId = file.Id,
                        RoleName = role
                    };
                    await _unitOfWork.HubFileRoles.CreateAsync(access);
                }
            }
            foreach (var existingRole in existingRoles)
            {
                if (!roles.Contains(existingRole.RoleName))
                {
                    _unitOfWork.HubFileRoles.Delete(existingRole);
                }
            }

            if (formFile != null)
            {
                string newExtension = Path.GetExtension(formFile.FileName);
                string oldExtension = Path.GetExtension(file.FilePath);
                if(newExtension != oldExtension)
                {
                    string newFilePath = Path.GetFileNameWithoutExtension(file.FilePath) + newExtension;
                    file.FilePath = newFilePath;
                }
                using (var stream = new FileStream(Path.Combine(_webHost.WebRootPath, "Uploads", file.FilePath), FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }
            }
            _unitOfWork.HubFiles.Update(file);
            await _unitOfWork.Complete();
        }

        public async Task UpdateNoFile(HubFile file, List<string> roles, string videoUrl)
        {
            var existingRoles = await _unitOfWork.HubFileRoles.GetRolesByFileIdAsync(file.Id);
            //if role from roles not in existingRoles, add it
            //if role from existingRoles not in roles, remove it
            foreach (string role in roles)
            {
                if (!existingRoles.Any(r => r.RoleName == role))
                {
                    HubFileRole access = new HubFileRole
                    {
                        FileId = file.Id,
                        RoleName = role
                    };
                    await _unitOfWork.HubFileRoles.CreateAsync(access);
                }
            }
            foreach (var existingRole in existingRoles)
            {
                if (!roles.Contains(existingRole.RoleName))
                {
                    _unitOfWork.HubFileRoles.Delete(existingRole);
                }
            }

            file.FilePath = videoUrl;
            _unitOfWork.HubFiles.Update(file);
            await _unitOfWork.Complete();
        }

        public async Task DeleteFileAsync(HubFile file)
        {
            var roles = await _unitOfWork.HubFileRoles.GetRolesByFileIdAsync(file.Id);
            foreach (var role in roles)
            {
                _unitOfWork.HubFileRoles.Delete(role);
            }
            _unitOfWork.HubFiles.Delete(file);
            // Delete the file from the server
            string path = Path.Combine(_webHost.WebRootPath, "Uploads", file.FilePath);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            await _unitOfWork.Complete();
        }
    }
}
