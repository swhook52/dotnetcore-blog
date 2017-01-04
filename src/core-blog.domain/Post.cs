using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsStatic { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DatePublished { get; set; }
    }
}
