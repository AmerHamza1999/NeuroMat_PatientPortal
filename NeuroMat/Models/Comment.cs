using System.ComponentModel.DataAnnotations;

namespace NeuroMat.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PressureFrameId { get; set; }
        public PressureFrame PressureFrame { get; set; }
        public string AuthorId { get; set; }
        public User Author { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}
