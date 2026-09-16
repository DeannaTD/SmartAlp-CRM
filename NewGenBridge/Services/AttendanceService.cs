using NewGenBridge.Helpers;
using NewGenBridge.Models;
using NewGenBridge.UoW;

namespace NewGenBridge.Services
{
    public class AttendanceService
    {
        UnitOfWork _unitOfWork;

        public AttendanceService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateAttendance(Attendance attendance)
        {
            await _unitOfWork.Attendances.CreateAsync(attendance);
            return await _unitOfWork.Complete();
        }

        public async Task<int> DeleteAttendanceAsync(string id)
        {
            Attendance attendance = await _unitOfWork.Attendances.GetByIdAsync(id);
            _unitOfWork.Attendances.Delete(attendance);
            return await _unitOfWork.Complete();
        }

        public async Task<int> UpdateAttendanceAsync(string id, int status)
        {
            Attendance attendance = await _unitOfWork.Attendances.GetByIdAsync(id);
            attendance.AttendanceStatus = status;
            _unitOfWork.Attendances.Update(attendance);
            return await _unitOfWork.Complete();
        }

        public List<NPUser> GetGroupAttendances(int groupId, DateTime date)
        {
            DateTime startDate = DateHelper.GetStartDate(date);
            List<NPUser> students = _unitOfWork.Users.GetStudentsWithAttendances(groupId);
            foreach (NPUser student in students)
            {
                student.Attendances = student.Attendances.Where(a => DateHelper.DateIsBetween(date, startDate, startDate.AddDays(6))).ToList();
            }
            return students;
        }

        public List<Attendance> GetAttendances(string userId)
        {
            return _unitOfWork.Attendances.GetStudentAttendances(userId);
        }

    }
}
