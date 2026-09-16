using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Helpers;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class UserRepository : BaseRepository<NPUser>
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public NPUser GetStudentWithAttendances(string userId)
        {
            return _dbSet.Include(u => u.Attendances).First(s => s.Id == userId);
        }

        public NPUser GetStudentWithAttendances(string userId, DateTime startDate)
        {
            DateTime endDate = startDate.AddDays(6);
            return _dbSet.Include(u => u.Attendances.Where(a => DateHelper.DateIsBetween(a.Date, startDate, endDate))).First(s => s.Id == userId);
        }

        public List<NPUser> GetStudentsWithAttendances(int groupId)
        {
            return _dbSet.Include(u => u.Attendances)
                         .Where(u => u.GroupId == groupId && u.StatusId != UserStatus.Archived)
                         .ToList();
        }

        public List<NPUser> GetArchivedStudents(int branchId = 0)
        {
            if (branchId == 0)
                return _dbSet.Include(u => u.Access)
                             .Where(u => u.StatusId == UserStatus.Archived && u.Access.Name == NPUR.Student)
                             .ToList();
            else
                return _dbSet.Include(u => u.Access)
                             .Where(u => u.StatusId == UserStatus.Archived
                                        && u.Access.Name == NPUR.Student
                                        && u.MentorDivisionId == branchId)
                             .ToList();

        }
    }
}
