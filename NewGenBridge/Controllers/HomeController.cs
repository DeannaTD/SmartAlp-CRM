using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.Services;
using NewGenBridge.ViewModels;
using System.Diagnostics;

namespace NewGenBridge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        LessonService _lessonService;
        UserManager<NPUser> _userManager;

        public HomeController(ILogger<HomeController> logger, LessonService lessonService, UserManager<NPUser> userManager)
        {
            _logger = logger;
            _lessonService = lessonService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            NPUser user = await _userManager.GetUserAsync(User);
            if (User.IsInRole(NPUR.Student))
            {
                //ToDo: если у пользователя не назначен урок
                Lesson currentLesson = await _lessonService.GetLessonByIdAsync(user.CurrentLessonId ?? 2);
                StudentHomeViewModel model = new StudentHomeViewModel()
                {
                    CurrentLessonId = user.CurrentLessonId ?? 1,
                    CurrentLessonName = currentLesson.Name,
                    CurrentLessonLink = currentLesson.FileLink
                };
                ViewBag.Status = user.StatusId;
                return View("Student", model);
            }
            if(User.IsInRole(NPUR.Mentor))
            {
                if(user.StatusId != UserStatus.Active)
                {

                }
                return RedirectToAction("Index", "Branch", new { isPersonal = true, mentorId = user.Id });
            }
            if (User.IsInRole(NPUR.Admin) || User.IsInRole(NPUR.Supervisor) || User.IsInRole(NPUR.Manager))
            {
                return View("~/Views/Supervision/AdminControlPanel.cshtml");
            }
            return View();
        }

        public IActionResult SetLanguage(string language)
        {
            Response.Cookies.Append("lang", language, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            return Ok("Language set to " + language);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
