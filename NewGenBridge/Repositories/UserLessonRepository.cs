using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Repositories
{
    public class UserLessonRepository : BaseRepository<UserLesson>
    {
        public UserLessonRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<UserLesson>> GetByUserIdAsync(string userId)
        {
            return await _context.UserLessons.Where(ul => ul.UserId == userId).ToListAsync();
        }

        public bool IsRecordExist(string userId, int lessonId)
        {
            return _context.UserLessons.Any(ul => ul.UserId == userId && ul.LessonId == lessonId);
        }
    }
}
