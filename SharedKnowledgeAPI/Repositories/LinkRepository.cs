using SharedKnowledgeAPI.Data;
using SharedKnowledgeAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Repositories
{
    public class LinkRepository
    {
        ApplicationDbContext _context;

        public LinkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Link> GetAll()
        {
            return _context.Link.ToList();
        }

        public bool AddLink(Link link)
        {
            try
            {
                _context.Link.Add(link);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
