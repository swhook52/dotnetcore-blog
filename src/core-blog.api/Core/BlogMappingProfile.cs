using AutoMapper;
using Domain;

namespace core_blog.api.Core
{
    public class BlogMappingProfile : Profile
    {
        public BlogMappingProfile()
        {
            CreateMap<Post, Dto.Post>()
                .ForMember(p => p.Status, options => options.MapFrom(newPost => newPost.DatePublished == null ? Dto.PostStatus.Draft : Dto.PostStatus.Published));
            CreateMap<Comment, Dto.Comment>();
            CreateMap<Tag, Dto.Tag>();
        }
    }
}
