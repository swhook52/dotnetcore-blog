using System;

namespace Dto
{
    public class Post
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Slug { get; set; }
        public bool IsStatic { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DatePublished { get; set; }
        public PostStatus Status { get; set; }
        public string[] Tags { get; set; }
    }
}
