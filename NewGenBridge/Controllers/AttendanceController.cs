using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.Services;
using NewGenBridge.ViewModels;
using System.Globalization;
using System.Text.Json;

namespace NewGenBridge.Controllers
{
    public class AttendanceController : Controller
    {
        AttendanceService _attendanceService;
        UserManager<NPUser> _userManager;

        public AttendanceController(AttendanceService attendanceService, UserManager<NPUser> userManager)
        {
            _attendanceService = attendanceService;
            _userManager = userManager;
        }

        public IActionResult GroupAttendance(int groupId, DateTime date)
        {
            date = DateHelper.GetStartDate(date);
            ViewBag.StartDate = date;
            ViewBag.GroupId = groupId;
            List<NPUser> students = _attendanceService.GetGroupAttendances(groupId, date);
            List<AttendanceViewModel> model = [];
            foreach (NPUser student in students)
            {
                List<Attendance> attendances = [];
                for(int i = 0; i<7; i++)
                {
                    attendances.Add(student.Attendances.FirstOrDefault(a => a.Date == date.AddDays(i)) ?? new Attendance() { Id = "unchecked"});
                }    
                model.Add(new AttendanceViewModel() 
                { 
                    StudentId = student.Id,
                    StudentName = student.FirstName + " " + student.LastName,
                    StudentUserName = student.UserName,
                    LessonId = student.CurrentLessonId ?? 1,
                    IsBlocked = student.StatusId == UserStatus.Blocked,
                    Attendances = attendances
                });
            }
            return PartialView("~/Views/Branch/GroupAttendance.cshtml", model);
        }

        [HttpPost]
        public async Task<bool> UpdateAttendance([FromBody] object data)
        {
            int result = 0;
            if(data is not null)
            {
                List<AttendanceTool> tools = JsonSerializer.Deserialize<List<AttendanceTool>>(data.ToString());
                foreach(AttendanceTool tool in tools)
                {
                    if (tool.Id == "unchecked")
                    {
                        if (tool.State != "0")
                        {
                            result += await _attendanceService.CreateAttendance(new Attendance()
                            {
                                UserId = tool.StudentId,
                                AttendanceStatus = Convert.ToInt32(tool.State),
                                Date = DateTime.ParseExact(tool.Stamp, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                            });
                        }
                    }
                    else
                    {
                        if (tool.State == "0")
                        {
                            result += await _attendanceService.DeleteAttendanceAsync(tool.Id);
                        }
                        else
                        {
                            result += await _attendanceService.UpdateAttendanceAsync(tool.Id, Convert.ToInt32(tool.State));
                        }
                    }
                }
            }
            return result > 0;
        }
        
        public IActionResult StudentAttendance()
        {
            NPUser student = _userManager.GetUserAsync(User).Result;
            List<Attendance> attendances = _attendanceService.GetAttendances(student.Id);
            var groupedAttendances = attendances.GroupBy(a => new { a.Date.Year, a.Date.Month })
                                    .Select(g => new StudentAttendanceViewModel()
                                    {
                                        Year = g.Key.Year,
                                        Month = g.Key.Month,
                                        Attendances = g.ToList()
                                    });
            return PartialView("~/Views/Home/Attendance.cshtml", groupedAttendances);
        }
    }

    public struct AttendanceTool
    {
        public string StudentId { get; set; }
        public string State { get; set; }
        public string Id { get; set; }
        public string Stamp { get; set; }
    }
}