using System;

namespace Dto
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
