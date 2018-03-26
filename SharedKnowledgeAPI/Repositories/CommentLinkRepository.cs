using SharedKnowledgeAPI.Data;
using SharedKnowledgeAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Repositories
{
    public class CommentLinkRepository
    {
        ApplicationDbContext _context;

        public CommentLinkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public CommentLink AddComment(CommentLink c)
        {
            try
            {
                _context.CommentLink.Add(c);
                _context.SaveChanges();
                return c;
            }
            catch
            {
                return null;
            }
        }

        public CommentLink GetComment(string id)
        {
            return _context.CommentLink.Where(cl => cl.Id.Equals(id)).FirstOrDefault();
        }

        public string Delete(CommentLink comment)
        {
            _context.CommentLink.Remove(comment);
            _context.SaveChanges();
            return comment.Id;
        }

        public IEnumerable<Object> GetAll()
        {
            return _context.CommentLink.Select(cl => new
            {
                id = cl.Id,
                body = cl.Body,
                date = cl.Date,
                rating = cl.Rate,
                authorId = cl.AuthorId,
                linkId = cl.LinkId,
                authorName = cl.ApplicationUser.UserName
            }).ToList();
        }
    }
}
