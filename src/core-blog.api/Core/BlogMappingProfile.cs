using AutoMapper;
using Domain;
using System.Linq;

namespace core_blog.api.Core
{
    public class BlogMappingProfile : Profile
    {
        public BlogMappingProfile()
        {
            CreateMap<Post, Dto.Post>()
                .ForMember(p => p.Status, options => options.MapFrom(newPost => newPost.DatePublished == null ? Dto.PostStatus.Draft : Dto.PostStatus.Published))
                .ForMember(p => p.Tags, options => options.MapFrom(newPost => newPost.PostTags.Select(pt => pt.Tag.Name)));
            CreateMap<Comment, Dto.Comment>();
            CreateMap<Tag, Dto.Tag>();
        }
    }
}
