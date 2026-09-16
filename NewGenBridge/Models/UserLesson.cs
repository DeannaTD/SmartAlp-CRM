using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class UserLesson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public int LessonId { get; set; }
        public bool IsOpened { get; set; } //is lesson opened currently or not (used for closing access to old projects)
        public DateTime? CloseAt { get; set; }
        public virtual NPUser User { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}
