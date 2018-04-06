using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Models
{
    public class CommentLinkViewModel
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string LinkId { get; set; }
    }
}
