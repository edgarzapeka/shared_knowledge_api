using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SharedKnowledgeAPI.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int Karma { get; set; }
        public string UserRole { get; set; }
        public string CustomUserName { get; set; }

        public virtual ICollection<Link> Links { get; set; }
        public virtual ICollection<CommentLink> Comments { get; set; }
    }
}
