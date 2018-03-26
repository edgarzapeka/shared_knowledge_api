using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Data
{
    public class LinkState
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string LinkURL { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
}
