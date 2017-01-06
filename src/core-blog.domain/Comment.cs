using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
