using NewGenBridge.Models;

namespace NewGenBridge.ViewModels
{
    public class MentorLessonAccessViewModel
    {
        public List<Level> LevelsWithLessons { get; set; }
        public string MentorId { get; set; }
        public string MentorName { get; set; }
        public List<LevelLessonsMentorViewModel> LevelsLessons { get; set; }
    }

    public class LevelLessonsMentorViewModel
    {
        public int LevelId { get; set; }
        public List<UserLesson> Lessons { get; set; }
    }
}
