
using System;

namespace Domain.Exceptions
{
    public class DuplicatePostException : Exception
    {
        private readonly object _slug;

        public override string Message => $"A post with slug '{_slug}' already exists.";

        public DuplicatePostException(string slug)
        {
            _slug = slug;
        }
    }
}
