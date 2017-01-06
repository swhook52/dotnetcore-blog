using Dto;
using System;

namespace Business.Services
{
    public interface ICommentService
    {
        Comment Get(Guid id);
        Comment Create(string postId, Comment comment);
        Comment Update(Guid commentId, Comment comment);
        Comment[] GetAll(string postSlug);
        void Remove(Guid id);
    }
}
