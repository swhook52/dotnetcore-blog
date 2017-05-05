using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Tag
    {
        [Key]
        public string Name { get; set; }

        public List<PostTag> Posts { get; set; } = new List<PostTag>();
    }
}