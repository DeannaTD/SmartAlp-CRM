using NewGenBridge.Models;
using NewGenBridge.UoW;

namespace NewGenBridge.Services
{
    public class LevelService
    {
        UnitOfWork _unitOfWork;

        public LevelService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateLevelAsync(Level model)
        {
            bool isSuccessful = false;
            if (model is not null)
            {
                await _unitOfWork.Levels.CreateAsync(model);
                int numberOfChanges = await _unitOfWork.Complete();
                isSuccessful = numberOfChanges > 0;
            }
            return isSuccessful;
        }

        public async Task<bool> EditLevelAsync(Level model)
        {
            bool isSuccessful = false;
            if (model is not null)
            {
                _unitOfWork.Levels.Update(model);
                int numberOfChanges = await _unitOfWork.Complete();
                isSuccessful = numberOfChanges > 0;
            }
            return isSuccessful;
        }


        public async Task<List<Level>> GetLevelsAsync()
        {
            return await _unitOfWork.Levels.GetCollectionAsync();
        }

        public List<Level> GetLevelsOnly()
        {
            return _unitOfWork.Levels.GetCleanCollection();
        }

        public async Task<Level> GetLevelByIdAsync(int id)
        {
            return await _unitOfWork.Levels.GetByIdAsync(id);
        }
        public async Task<bool> DeleteLevelAsync(Level level)
        {
            bool isSuccessful = false;
            if (level is not null)
            {
                var lessons = await _unitOfWork.Lessons.GetCollectionAsync();
                lessons = [.. lessons.Where(lesson => lesson.LevelId == level.Id)];

                var userLessons = await _unitOfWork.UserLessons.GetCollectionAsync();
                userLessons = [.. userLessons.Where(ul => lessons.Any(lesson => lesson.Id == ul.LessonId))];

                foreach(var ul in userLessons)
                {
                    _unitOfWork.UserLessons.Delete(ul);
                }

                var students = await _unitOfWork.Users.GetCollectionAsync();
                students = [.. students.Where(student => lessons.Any(lesson => lesson.Id == student.CurrentLessonId))];

                foreach(var student in students)
                {
                    student.CurrentLessonId = null;
                }

                foreach(var lesson in lessons)
                {
                    _unitOfWork.Lessons.Delete(lesson);
                }

                _unitOfWork.Levels.Delete(level);
                int numberOfChanges = await _unitOfWork.Complete();
                isSuccessful = numberOfChanges > 0;
            }
            return isSuccessful;
        }
    }
}
