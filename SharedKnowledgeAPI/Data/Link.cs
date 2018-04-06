using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Models
{
    public class Link
    {
        [Key]
        public string Id { get; set; }
        public string Title { get; set; }
        public string LinkURL { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
        public string CategoryName { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }        
        public virtual ICollection<CommentLink> Comments { get; set; }
    }
}
