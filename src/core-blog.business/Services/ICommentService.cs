using System;

namespace Business.Services
{
    public interface ICommentService
    {
        Domain.Comment Get(Guid id);
        Domain.Comment Create(string postId, Dto.Comment comment);
        Domain.Comment Update(Guid commentId, Dto.Comment comment);
        Domain.Comment[] GetAll(string postSlug);
        void Remove(Guid id);
    }
}
