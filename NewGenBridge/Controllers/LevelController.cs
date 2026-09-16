using IronSoftware.Drawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.Services;
using NewGenBridge.ViewModels;
using System.IO;
using System.IO.Compression;
//ToDo: filelink пустой
namespace NewGenBridge.Controllers
{
    public class LevelController : Controller
    {
        LessonService _lessonService;
        LevelService _levelService;
        AccountService _accountService;
        UserManager<NPUser> _userManager;
        BranchService _branchService;
        ContractService _contractService;
        IWebHostEnvironment _webHost;


        public LevelController(LessonService lessonService, LevelService levelService, IWebHostEnvironment webHost, UserManager<NPUser> userManager, BranchService branchService, ContractService contractService, AccountService accountService)
        {
            _levelService = levelService;
            _lessonService = lessonService;
            _userManager = userManager;
            _webHost = webHost;
            _branchService = branchService;
            _contractService = contractService;
            _accountService = accountService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            List<Level> levels = await _levelService.GetLevelsAsync();
            return View(levels);
        }

        [HttpGet]
        public IActionResult CreateLevel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLevelAsync(Level level, IFormFile? picture)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                result = await _levelService.CreateLevelAsync(level);
                result &= await _contractService.AddPricesOfNewLevel(level.Id);
                if (picture is not null)
                {
                    try
                    {
                        string path = Path.Combine(_webHost.WebRootPath, "Projects", "Levels");
                        string fileName = level.Id.ToString() + ".png";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //put picture to the path
                        string filePath = Path.Combine(path, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await picture.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return result ? RedirectToAction("LevelControlMenu") : BadRequest("Something went wrong");
        }

        public IActionResult CreateLesson(int levelId = -1)
        {
            ViewBag.LevelId = levelId;
            return View();
        }

        //!---------------------------------------------------------------------------
        //use only for master page
        //use CreateNewLesson for admin page
        [HttpPost]
        public async Task<IActionResult> CreateLessonAsync(LessonCreationViewModel model)
        {
            string[] message;
            if (ModelState.IsValid)
            {
                Lesson lesson = new Lesson
                {
                    Name = model.Name,
                    LevelId = model.LevelId
                };
                bool result = await _lessonService.CreateLessonAsync(lesson);
                if (!result)
                {
                    message = ["Ошибка при создании урока"];
                    return View("~/Views/Master/Message.cshtml", message);
                }
                if (model.File is not null)
                {
                    try
                    {
                        string fileName = LabelCreator.CreateProjectLabel(model.LevelId, lesson.Id);
                        string pathToZip = Path.Combine(_webHost.WebRootPath, "Projects", fileName + ".zip");
                        using FileStream stream = new FileStream(pathToZip, FileMode.Create);
                        model.File.CopyTo(stream);
                        stream.Close();
                        string extractPath = Path.Combine(_webHost.WebRootPath, "Projects", fileName);
                        ZipFile.ExtractToDirectory(pathToZip, extractPath, true);
                        message = ["Проект создан успешно. Файл создан успешно"];
                    }
                    catch (Exception ex)
                    {
                        message = ["Ошибка при загрузке файла: " + ex.Message];
                        return View("~/Views/Master/Message.cshtml", message);
                    }
                }
                else
                {
                    message = ["Проект создан успешно. Файл проекта не был предоставлен"];
                    return View("~/Views/Master/Message.cshtml", message);
                }
            }
            else message = ["Ошибка валидации модели урока"];
            return View("~/Views/Master/Message.cshtml", message);
        }
        //!---------------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> StudentLessonHandle(string studentId, string info = "")
        {
            //ToDo: сделать так, чтобы загрузка всех проектов и уровней была один раз
            //ToDo: сделать так, чтобы уровень и урок были выбраны по умолчанию по LessonId
            NPUser student = await _userManager.FindByIdAsync(studentId);
            List<Level> levels = await _levelService.GetLevelsAsync();
            int lessonId = student.CurrentLessonId ?? 1;
            int levelId = await _lessonService.GetLevelId(lessonId);
            ViewBag.Levels = new SelectList(levels, "Id", "Name", levelId);
            StudentLessonViewModel model = new StudentLessonViewModel
            {
                StudentId = studentId,
                StudentNameInfo = info,
                LessonId = lessonId
            };

            return PartialView("StudentLessonMenu", model);
        }

        public async Task<JsonResult> GetLessonSelectList(int levelId, string studentId, int lessonId = -1)
        {
            List<UserLesson> userLessons = await _lessonService.GetStudentLessonHistory(studentId);
            IEnumerable<Lesson> allLessons = await _lessonService.GetLessonsAsync(levelId);
            // Create a HashSet for quick lookup of opened lessons
            var openedLessonIds = userLessons.Where(ul => ul.IsOpened).Select(ul => ul.LessonId).ToHashSet();

            // Project lessons with "(+)" for opened ones
            var selectListItems = allLessons.Select(lesson => new
            {
                Id = lesson.Id,
                Name = openedLessonIds.Contains(lesson.Id) ? lesson.Name + " (✓)" : lesson.Name
            }).ToList();

            return Json(new SelectList(selectListItems, "Id", "Name"));
        }

        public async Task<JsonResult> GetLessonSelectListGroup(int levelId)
        {
            IEnumerable<Lesson> allLessons = await _lessonService.GetLessonsAsync(levelId);
            
            var selectListItems = allLessons.Select(lesson => new
            {
                Id = lesson.Id,
                Name = lesson.Name
            }).ToList();

            return Json(new SelectList(selectListItems, "Id", "Name"));
        }

        public async Task<IActionResult> Lesson(int lessonId, string lessonLink = "")
        {
            int levelId = await _lessonService.GetLevelId(lessonId);
            if (levelId == 2) //if scratch
            {
                ViewBag.isHTML = true;
                ViewBag.lessonLink = "../Projects/" + LabelCreator.CreateProjectLabel(levelId, lessonId) + "/index.html";
                return View();
            }
            else ViewBag.isHTML = false;

            if (lessonLink != "")
            {
                //ToDo: переместить все в LabelCreator или PathCreator
                ViewBag.lessonLink = "../Projects/" + lessonLink + "/lesson.pdf";
                return View();
            }
            if (lessonId == 0)
            {
                NPUser user = _userManager.GetUserAsync(User).Result;
                lessonId = user.CurrentLessonId ?? 1;
            }
            Level level = _lessonService.GetLevelByLessonId(lessonId).Result;
            ViewBag.lessonLink = "../Projects/" + LabelCreator.CreateProjectLabel(level.Id, lessonId) + "/lesson.pdf";
            return View();
        }

        public IActionResult LessonPreview(int lessonId)
        {
            Level level = _lessonService.GetLevelByLessonId(lessonId).Result;
            if(level.Id == 2)
            {
                ViewBag.isHTML = true;
                ViewBag.lessonLink = "../Projects/" + LabelCreator.CreateProjectLabel(level.Id, lessonId) + "/index.html";
                return View("Lesson");
            }
            else ViewBag.isHTML = false;
            ViewBag.lessonLink = "../Projects/" + LabelCreator.CreateProjectLabel(level.Id, lessonId) + "/lesson.pdf";
            return View("Lesson");
        }

        public async Task<IActionResult> StudentProgress()
        {
            NPUser user = await _userManager.GetUserAsync(User);
            Level level = await _lessonService.GetLevelByLessonId(user.CurrentLessonId ?? 2);
            ViewBag.Description = level.Description;
            ViewBag.LevelIconLink = level.Id + ".png";
            ViewBag.Status = user.StatusId;
            if (user.StatusId != UserStatus.Active)
                return View("Progress");
            IEnumerable<Lesson> lessons = await _lessonService.GetLessonsAsync(level.Id);

            List<UserLesson> userLessons = await _lessonService.GetStudentLessonHistory(user.Id);
            
            List<LessonProgressViewModel> model = new List<LessonProgressViewModel>();
            foreach(Lesson lesson in lessons)
            {
                UserLesson? record = userLessons.Find(ul => ul.LessonId == lesson.Id);
                if (record is not null)
                {
                    model.Add(new LessonProgressViewModel()
                    {
                        Name = lesson.Name,
                        IsOpened = record.IsOpened,
                        Link = lesson.FileLink ?? LabelCreator.CreateProjectLabel(level.Id, lesson.Id)
                    });
                }
                else
                {
                    model.Add(new LessonProgressViewModel()
                    {
                        Name = lesson.Name,
                        IsOpened = false,
                        Link = LabelCreator.CreateProjectLabel(level.Id, lesson.Id)
                    });
                }
            }
            return View("Progress", model);
        }

        [HttpGet]
        public async Task<IActionResult> LevelControlMenu()
        {
            List<Level> levels = await _levelService.GetLevelsAsync();
            return View(levels);
        }

        [HttpGet]
        public async Task<IActionResult> LessonsControlPanel(int levelId = -1)
        {
            var lessons = await _lessonService.GetLessonsAsync(levelId);
            ViewBag.LevelId = levelId;
            ViewBag.LevelName = await _levelService.GetLevelByIdAsync(levelId) is Level level ? level.Name : "Level name";
            return View(lessons.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> LessonEdit(int lessonId)
        {
            Lesson? lesson = await _lessonService.GetLessonByIdAsync(lessonId);
            if (lesson is null) return View("~/Views/Master/Message.cshtml", new string[] { "Lesson not found" });
            return View(lesson);
        }

        [HttpPost]
        public async Task<IActionResult> LessonUpdate(int lessonId, string lessonName, IFormFile lessonFile)
        {
            string messages = "";

            Lesson lesson = await _lessonService.GetLessonByIdAsync(lessonId);
            if (lesson is null) return NotFound();
            bool result = true;
            if(lesson.Name != lessonName)
            {
                result = await _lessonService.Rename(lesson, lessonName);
            }
            if(lessonFile is not null)
            {
                try
                {
                    string fileName = LabelCreator.CreateProjectLabel(lesson.LevelId, lesson.Id);
                    string pathToProject = Path.Combine(_webHost.WebRootPath, "Projects", fileName);
                    if(!Directory.Exists(pathToProject))
                    {
                        Directory.CreateDirectory(pathToProject);
                        messages += "Project folder created. ";
                    }
                    //lessonFile это pdf файл. Добавляем его в папку проекта
                    string pathToPdf = Path.Combine(pathToProject, "lesson.pdf");
                    messages += "Path to pdf: " + pathToPdf + ". ";
                    using FileStream stream = new FileStream(pathToPdf, FileMode.Create);
                    lessonFile.CopyTo(stream);
                    messages += "File copied to stream. ";
                    stream.Close();
                    messages += "Stream closed. ";
                    await _lessonService.UpdatePath(lesson, Path.Combine("Projects", fileName));
                    messages += "Path updated. ";
                    result = result && true;
                }
                catch(Exception e)
                {
                    messages += "Exception caught: " + e.Message;
                    result = false;
                }
            }
            Console.WriteLine(messages);
            return result ? Ok() : BadRequest("Something went wrong");
        }

        public async Task<IActionResult> EditLevel(int levelId)
        {
            Level level = await _levelService.GetLevelByIdAsync(levelId);
            return View(level);
        }

        [HttpPost]
        public async Task<IActionResult> EditLevel(Level level, IFormFile? picture)
        {
            bool result = false;
            if (ModelState.IsValid)
            {
                result = await _levelService.EditLevelAsync(level);
                if (picture is not null)
                {
                    try
                    {
                        string path = Path.Combine(_webHost.WebRootPath, "Projects", "Levels");
                        string fileName = level.Id.ToString() + ".png";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //put picture to the path
                        string filePath = Path.Combine(path, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await picture.CopyToAsync(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return result ? RedirectToAction("LevelControlMenu") : BadRequest("Something went wrong");
        }

        public async Task<IActionResult> CreateNewLesson(LessonCreationViewModel model)
        {
            bool result = true;
            if (ModelState.IsValid)
            {
                Lesson lesson = new Lesson
                {
                    Name = model.Name,
                    LevelId = model.LevelId
                };
                result = await _lessonService.CreateLessonAsync(lesson);
                if (!result) return BadRequest("Something went wrong");

                if (model.File is not null)
                {
                    try
                    {
                        string fileName = LabelCreator.CreateProjectLabel(lesson.LevelId, lesson.Id);
                        string pathToProject = Path.Combine(_webHost.WebRootPath, "Projects", fileName);
                        if (!Directory.Exists(pathToProject))
                        {
                            Directory.CreateDirectory(pathToProject);
                        }
                        string pathToPdf = Path.Combine(pathToProject, "lesson.pdf");
                        using FileStream stream = new FileStream(pathToPdf, FileMode.Create);
                        model.File.CopyTo(stream);
                        stream.Close();
                        await _lessonService.UpdatePath(lesson, Path.Combine("Projects", fileName));
                        result = result && true;
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                }
            }
            else return BadRequest("Invalid data, try again");
            return result ? RedirectToAction("LessonsControlPanel", new { levelId = model.LevelId }) : BadRequest("Something went wrong");
        }

        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            Lesson lesson = await _lessonService.GetLessonByIdAsync(lessonId);
            int levelId = lesson.LevelId;
            bool result = await _lessonService.DeleteLessonAsync(lesson);
            return result ? Ok("Deleted") : BadRequest("Something went wrong");
        }

        public async Task<IActionResult> DeleteLevel(int levelId)
        {
            Level level = await _levelService.GetLevelByIdAsync(levelId);
            bool result = await _levelService.DeleteLevelAsync(level);
            return result ? RedirectToAction("LevelControlMenu") : BadRequest("Something went wrong");
        }

        [HttpGet]

        public async Task<IActionResult> SwitchLessonToAll(int groupId)
        {
            Group group = await _branchService.GetGroupByIdAsync(groupId);
            List<Level> levels = await _levelService.GetLevelsAsync();
            if(group.lessonId is null)
            {
                ViewBag.Levels = new SelectList(levels, "Id", "Name");
            }
            else
            {
                int levelId = await _lessonService.GetLevelId((int)group.lessonId);
                ViewBag.Levels = new SelectList(levels, "Id", "Name", levelId);
            }
            SwitchLessonViewModel model = new SwitchLessonViewModel
            {
                GroupId = groupId,
                LevelId = 0,
                LessonId = group.lessonId
            };

            return PartialView("SwitchLessonToAll", model);
        }

        public async Task<IActionResult> MentorIndex()
        {

            NPUser user = await _userManager.GetUserAsync(User);
            List<MentorLevel> mentorLevels = _accountService.GetMentorLevels(user.Id);
            List<UserLesson> userLessons = await _lessonService.GetStudentLessonHistory(user.Id);
            List<Lesson> lessons = new List<Lesson>();
            foreach(var ml in mentorLevels)
            {
                var tml = await _lessonService.GetLessonsAsync(ml.LevelId);
                lessons.AddRange(tml);
            }
            var openedIdsTest = userLessons.Where(ul => ul.IsOpened).Select(ul => ul.LessonId).ToList();
            var openedIds = userLessons.Where(ul => ul.IsOpened).Select(ul => ul.LessonId).ToHashSet();
            foreach (var lesson in lessons)
            {
                if(openedIds.Contains(lesson.Id))
                {
                    lesson.FileLink = "OPEN";
                }
            }
            return View("MentorIndex", lessons);
        }
    }
}