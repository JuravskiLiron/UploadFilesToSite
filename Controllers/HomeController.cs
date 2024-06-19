using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Upload_Files.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost("/upload")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            try
            {
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileType = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (IsAllowedFileType(fileType) && IsFileSizeAllowed(file.Length))
                        {
                            var uniqueFileName = GetUniqueFileName(file.FileName);
                            var fileSavePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                            using (var stream = new FileStream(fileSavePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            ViewBag.UploadSuccess = $"File '{uniqueFileName}' uploaded successfully.";
                        }
                        else
                        {
                            ViewBag.UploadError = $"File '{file.FileName}' has an invalid format or is too large.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.UploadError = $"An error occurred: {ex.Message}";
                return View("Index");
            }

            return View("Index");
        }

        public IActionResult Index()
        {
            return View();
        }

        private bool IsAllowedFileType(string fileType)
        {
            return fileType == ".png" || fileType == ".jpg" || fileType == ".jpeg";
        }

        private bool IsFileSizeAllowed(long fileSize)
        {
            return fileSize <= 10485760; // 10MB
        }

        private string GetUniqueFileName(string fileName)
        {
            return $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        }
    }
}
