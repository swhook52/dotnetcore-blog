using System;

namespace Domain.Exceptions
{
    public class CommentNotFoundException : NotFoundException
    {
        public CommentNotFoundException(Guid id) : base("Comment", "Id", id) { }
    }
}
