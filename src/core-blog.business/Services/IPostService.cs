using System.Collections.Generic;

namespace Business.Services
{
    public interface IPostService
    {
        Domain.Post Get(string slug);
        Domain.Post Create(Dto.Post post);
        Domain.Post Update(string slug, Dto.Post post);
        IEnumerable<Domain.Post> GetAll();
        void Delete(string slug);
        void DeleteAll();
    }
}
