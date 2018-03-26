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

        public Link AddLink(Link link)
        {
            try
            {
                _context.Link.Add(link);
                _context.SaveChanges();
                return link;
            }
            catch
            {
                return null;
            }
        }

        public string Delete(Link link)
        {
            try
            {
                _context.Link.Remove(link);
                _context.SaveChanges();
                return link.Id;
            }
            catch
            {
                return null;
            }
        }

        public Link Update(Link link)
        {
            try
            {
                _context.Link.Update(link);
                _context.SaveChanges();
                return link;
            }
            catch
            {
                return null;
            }
        }

        public Link GetLink(string id)
        {
            return _context.Link.Where(l => l.Id.Equals(id)).FirstOrDefault();
        }
    }
}
