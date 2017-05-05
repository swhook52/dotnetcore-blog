using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class PostTag
    {
        [Key]
        public int Id { get; set; }

        public Post Post { get; set; }
        public Tag Tag { get; set; }
    }
}