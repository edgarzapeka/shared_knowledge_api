using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Models
{
    public class CommentLink
    {
        [Key]
        public string Id { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public int Rate { get; set; }

        public string LinkId { get; set; }
        public string AuthorId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Link Link { get; set; }
    }
}
