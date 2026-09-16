using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class AttendanceRepository : BaseRepository<Attendance>
    {
        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public List<Attendance> GetStudentAttendances(string studentId)
        {
            return _dbSet.Where(a => a.UserId == studentId).ToList();
        }
    }
}
