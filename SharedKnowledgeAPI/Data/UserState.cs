using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Data
{
    public class UserState
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Karma { get; set; }
        public string Token { get; set; }
        public string Secret { get; set; }
        public string UserRole { get; set; }
    }
}
