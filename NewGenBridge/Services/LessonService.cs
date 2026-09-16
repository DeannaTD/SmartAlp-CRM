using NewGenBridge.Models;
using NewGenBridge.UoW;

namespace NewGenBridge.Services
{
    public class LessonService
    {
        UnitOfWork _unitOfWork;
        public LessonService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Lesson> GetLessonByIdAsync(int lessonId)
        {
            //ToDo: прописать если урок не найден
            return await _unitOfWork.Lessons.GetByIdAsync(lessonId);
        }

        public async Task<bool> CreateLessonAsync(Lesson lesson)
        {
            bool isSuccessful = false;
            if (lesson is not null)
            {
                await _unitOfWork.Lessons.CreateAsync(lesson);
                int numberOfChanges = await _unitOfWork.Complete();
                isSuccessful = numberOfChanges > 0;
            }
            return isSuccessful;
        }

        public async Task<IEnumerable<Lesson>> GetLessonsAsync(int levelId = -1)
        {
            if(levelId == -1)
                return await _unitOfWork.Lessons.GetCollectionAsync();
            else
            {
                var lessons = await _unitOfWork.Lessons.GetCollectionAsync();
                return lessons.Where(lesson => lesson.LevelId == levelId);
            }
        }

        public async Task<int> GetLevelId(int lessonId)
        {
            Lesson lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId);
            Level level = await _unitOfWork.Levels.GetByIdAsync(lesson.LevelId);
            return level.Id;
        }

        public async Task<Level> GetLevelByLessonId(int lessonId)
        {
            Lesson lesson = await _unitOfWork.Lessons.GetByIdAsync(lessonId);
            return await _unitOfWork.Levels.GetByIdAsync(lesson.LevelId);
        }

        public async Task<bool> OpenLessonToStudent(string studentId, int lessonId, int duration = 1, bool priority = false)
        {
            int result = 0;
            DateTime closeAt = DateTime.Now.Date.AddDays(duration);
            if (!_unitOfWork.UserLessons.IsRecordExist(studentId, lessonId))
            {
                UserLesson record = new UserLesson()
                {
                    UserId = studentId,
                    LessonId = lessonId,
                    IsOpened = true,
                    CloseAt = closeAt
                };
                await _unitOfWork.UserLessons.CreateAsync(record);
                result = await _unitOfWork.Complete();
            }
            else
            {
                var userLessons = await _unitOfWork.UserLessons.GetCollectionAsync();
                var userLesson = userLessons.Find(ul => ul.UserId == studentId && ul.LessonId == lessonId);
                if(userLesson.CloseAt is not null)
                {
                    if(!userLesson.IsOpened || priority)
                    {
                        userLesson.CloseAt = closeAt;
                    }
                }
                else
                {
                    userLesson.CloseAt = closeAt;
                }
                userLesson.IsOpened = true;
                _unitOfWork.UserLessons.Update(userLesson);
                result = await _unitOfWork.Complete();
            }
            return result > 0;
        }
        public async Task<bool> CloseLessonForStudent(string studentId, int lessonId)
        {
            var allUserLessons = await _unitOfWork.UserLessons.GetByUserIdAsync(studentId);
            var userLesson = allUserLessons.Find(ul => ul.LessonId == lessonId);
            if (userLesson != null)
            {
                userLesson.IsOpened = false;
                _unitOfWork.UserLessons.Update(userLesson);
                return await _unitOfWork.Complete() > 0;
            }
            return false;
        }

        public async Task<List<UserLesson>> GetStudentLessonHistory(string studentId)
        {
            return await _unitOfWork.UserLessons.GetByUserIdAsync(studentId);
        }

        public bool IsLessonOpened(string studentId, int lessonId)
        {
            return _unitOfWork.UserLessons.IsRecordExist(studentId, lessonId);
        }

        public async Task<bool> Rename(Lesson lesson, string newName)
        {
            lesson.Name = newName;
            _unitOfWork.Lessons.Update(lesson);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<bool> UpdatePath(Lesson lesson, string newPath)
        {
            lesson.FileLink = newPath;
            _unitOfWork.Lessons.Update(lesson);
            return await _unitOfWork.Complete() > 0;
        }

        public async Task<bool> DeleteLessonAsync(Lesson lesson)
        {
            bool isSuccessful = false;
            if (lesson is not null)
            {
                _unitOfWork.Lessons.Delete(lesson);
                int numberOfChanges = await _unitOfWork.Complete();
                isSuccessful = numberOfChanges > 0;
            }
            return isSuccessful;
        }
    }
}
