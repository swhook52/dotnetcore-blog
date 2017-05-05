using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class PostService : IPostService
    {
        private readonly BloggingContext _context;

        public PostService(BloggingContext context)
        {
            _context = context;
        }

        public Post Get(string slug)
        {
            var post = _context.Posts
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .SingleOrDefault(p => p.Slug.ToLower() == slug.ToLower());

            if (post == null)
                throw new PostNotFoundException(slug);

            return post;
        }

        public Post Create(Dto.Post post)
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

            foreach (var tagName in post.Tags)
            {
                newPost.PostTags.Add(new PostTag
                {
                    Post = newPost,
                    Tag = GetOrCreateTag(tagName)
                });
            }

            _context.Posts.Add(newPost);
            _context.SaveChanges();

            return newPost;
        }

        private Tag GetOrCreateTag(string tagName)
        {
            var tag = _context.Tags.SingleOrDefault(p => p.Name.ToLower() == tagName.ToLower());
            if (tag != null)
                return tag;

            return new Tag { Name = tagName };
        }

        public Post Update(string existingSlug, Dto.Post post)
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

            return existingPost;
        }

        public IEnumerable<Post> GetAll()
        {
            return _context.Posts
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag);
        }

        public void Delete(string slug)
        {
            var existingPost = _context.Posts.SingleOrDefault(p => p.Slug == slug);
            if (existingPost == null)
                throw new PostNotFoundException(slug);

            _context.Posts.Remove(existingPost);
            _context.SaveChanges();
        }

        public void DeleteAll()
        {
            var posts = _context.Posts;
            foreach (var post in posts)
            {
                _context.Posts.Remove(post);
            }

            _context.SaveChanges();
        }
    }
}
