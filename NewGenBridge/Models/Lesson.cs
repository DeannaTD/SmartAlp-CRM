using System.ComponentModel.DataAnnotations;

namespace NewGenBridge.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int LevelId { get; set; }
        public string? FileLink { get; set; }

        public int Index { get; set; }
        public virtual ICollection<UserLesson> UserLessons { get; set; }
        public virtual Level Level { get; set; }

    }
}
