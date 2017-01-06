using System;
using System.Linq;
using Domain;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class CommentService : ICommentService
    {
        private readonly BloggingContext _context;

        public CommentService(BloggingContext context)
        {
            _context = context;
        }

        public Dto.Comment Get(Guid id)
        {
            var comment = _context.Comments.SingleOrDefault(p => p.Id == id);
            if (comment == null)
                throw new CommentNotFoundException(id);

            return new Dto.Comment
            {
                Id = comment.Id,
                Name = comment.Name,
                Message = comment.Message,
                DateCreated = comment.DateCreated
            };
        }

        public Dto.Comment Create(string postId, Dto.Comment comment)
        {
            var post = _context.Posts.SingleOrDefault(p => p.Slug == postId);
            if (post == null)
                throw new PostNotFoundException(postId);

            var newComment = new Comment
            {
                Id = Guid.NewGuid(),
                Name = comment.Name,
                Message = comment.Message,
                DateCreated = DateTime.UtcNow,
                Email = comment.Email,
                Post = post
            };

            _context.Comments.Add(newComment);
            _context.SaveChanges();

            return new Dto.Comment
            {
                Id = newComment.Id,
                Name = newComment.Name,
                Message = newComment.Message,
                Email = newComment.Email,
                DateCreated = newComment.DateCreated
            };
        }

        public Dto.Comment Update(Guid commentId, Dto.Comment comment)
        {
            var existingComment = _context.Comments.SingleOrDefault(p => p.Id == commentId);
            if (existingComment == null)
                throw new CommentNotFoundException(commentId);

            existingComment.Message = comment.Message;

            _context.SaveChanges();

            return new Dto.Comment
            {
                Id = existingComment.Id,
                Name = existingComment.Name,
                Message = existingComment.Message,
                DateCreated = existingComment.DateCreated
            };
        }

        public Dto.Comment[] GetAll(string postSlug)
        {
            var post = _context.Posts
                .Include(p => p.Comments)
                .SingleOrDefault(p => p.Slug == postSlug);
            if (post == null)
                throw new PostNotFoundException(postSlug);

            return post.Comments.Select(comment => new Dto.Comment
            {
                Id = comment.Id,
                Name = comment.Name,
                Message = comment.Message,
                DateCreated = comment.DateCreated
            }).OrderBy(p => p.DateCreated).ToArray();
        }

        public void Remove(Guid id)
        {
            var comment = _context.Comments.SingleOrDefault(p => p.Id == id);
            if (comment == null)
                throw new CommentNotFoundException(id);

            _context.Remove(comment);
            _context.SaveChanges();
        }
    }
}
