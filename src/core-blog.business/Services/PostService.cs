using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Exceptions;

namespace Business.Services
{
    public class PostService : IPostService
    {
        private readonly BloggingContext _context;

        public PostService(BloggingContext context)
        {
            _context = context;
        }

        public Dto.Post Get(string slug)
        {
            var post = _context.Posts.SingleOrDefault(p => p.Slug.ToLower() == slug.ToLower());
            if (post == null)
                throw new PostNotFoundException(slug);

            return new Dto.Post
            {
                Slug = post.Slug,
                Body = post.Body,
                DateCreated = post.DateCreated,
                DatePublished = post.DatePublished,
                IsFeatured = post.IsFeatured,
                IsStatic = post.IsStatic,
                Title = post.Title,
                Status = post.DatePublished == null ? Dto.PostStatus.Draft : Dto.PostStatus.Published
            };
        }

        public Dto.Post Create(Dto.Post post)
        {
            var slug = post.Slug?.ToLowerInvariant().Trim();
            if (string.IsNullOrEmpty(slug))
                throw new Exception("Slug is required");

            if (_context.Posts.Any(p => p.Slug == slug))
                throw new DuplicatePostException(slug);

            var newPost = new Post
            {
                Slug = slug,
                Body = post.Body,
                DateCreated = post.DateCreated ?? DateTime.UtcNow,
                DatePublished = post.DatePublished,
                IsFeatured = post.IsFeatured,
                IsStatic = post.IsStatic,
                Title = post.Title
            };

            _context.Posts.Add(newPost);
            _context.SaveChanges();

            return new Dto.Post
            {
                Slug = newPost.Slug,
                Body = newPost.Body,
                DateCreated = newPost.DateCreated,
                DatePublished = newPost.DatePublished,
                IsFeatured = newPost.IsFeatured,
                IsStatic = newPost.IsStatic,
                Title = newPost.Title,
                Status = newPost.DatePublished == null ? Dto.PostStatus.Draft : Dto.PostStatus.Published
            };
        }

        public Dto.Post Update(string existingSlug, Dto.Post post)
        {
            var oldSlug = existingSlug?.ToLowerInvariant().Trim();
            var newSlug = post.Slug?.ToLowerInvariant().Trim();

            var existingPost = _context.Posts.SingleOrDefault(p => p.Slug == oldSlug);
            if (existingPost == null)
                throw new PostNotFoundException(oldSlug);

            // If the slug is changing, make sure it's still unique
            if (oldSlug != newSlug && _context.Posts.Any(p => p.Slug == newSlug))
                throw new DuplicatePostException(newSlug);

            existingPost.Slug = newSlug;
            existingPost.Body = post.Body;
            existingPost.DatePublished = post.DatePublished;
            existingPost.IsFeatured = post.IsFeatured;
            existingPost.IsStatic = post.IsStatic;
            existingPost.Title = post.Title;

            _context.SaveChanges();

            return new Dto.Post
            {
                Slug = existingPost.Slug,
                Body = existingPost.Body,
                DateCreated = existingPost.DateCreated,
                DatePublished = existingPost.DatePublished,
                IsFeatured = existingPost.IsFeatured,
                IsStatic = existingPost.IsStatic,
                Title = existingPost.Title,
                Status = existingPost.DatePublished == null ? Dto.PostStatus.Draft : Dto.PostStatus.Published
            };
        }

        public IEnumerable<Dto.Post> GetAll()
        {
            return _context.Posts.Select(post => new Dto.Post
            {
                Slug = post.Slug,
                Body = post.Body,
                DateCreated = post.DateCreated,
                DatePublished = post.DatePublished,
                IsFeatured = post.IsFeatured,
                IsStatic = post.IsStatic,
                Title = post.Title,
                Status = post.DatePublished == null ? Dto.PostStatus.Draft : Dto.PostStatus.Published
            });
        }
    }
}
