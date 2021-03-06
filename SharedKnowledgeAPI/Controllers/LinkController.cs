﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SharedKnowledgeAPI.Data;
using SharedKnowledgeAPI.Models;
using SharedKnowledgeAPI.Repositories;

namespace SharedKnowledgeAPI.Controllers
{
    public class LinkController : Controller
    {
        ApplicationDbContext _context;
        LinkRepository _linkRepo;
        CommentLinkRepository _commentLinkRepo;

        public LinkController(ApplicationDbContext context)
        {
            _context = context;
            _linkRepo = new LinkRepository(context);
            _commentLinkRepo = new CommentLinkRepository(context);
        }

        public JsonResult GetAll()
        {
            return new JsonResult(new
            {
                links = _linkRepo.GetAll().ToList().Select(l => new LinkViewModel
                {
                    Id = l.Id,
                    Title = l.Title,
                    LinkURL = l.LinkURL,
                    Rating = l.Rating,
                    Date = l.Date,
                    UserName = _context.ApplicationUser.Where(au => au.Id == l.UserId).FirstOrDefault().UserName,
                    UserId = l.UserId,
                    Category = l.CategoryName 
                })
            });
        }

        [HttpPost]
        public IActionResult Add([FromBody]JObject json)
        {
            string email = json.GetValue("email").ToString();
            string title = json.GetValue("title").ToString();
            string url = json.GetValue("url").ToString();
            string category = json.GetValue("category").ToString();

            try
            {
                ApplicationUser user = _context.Users.Where(u => u.Email == email).FirstOrDefault();
                if (user != null)
                {
                    Link link = new Link()
                    {
                        Title = title,
                        LinkURL = url,
                        Date = DateTime.Now,
                        Rating = 0,
                        CategoryName = category,
                        UserId = user.Id,
                    };
                    Link result = _linkRepo.AddLink(link);
                    if (result != null)
                    {
                        return new JsonResult(new LinkViewModel()
                        {
                            Id = result.Id,
                            Title = result.Title,
                            LinkURL = result.LinkURL,
                            Date = result.Date,
                            Rating = result.Rating,
                            UserName = result.ApplicationUser.CustomUserName != " " ? result.ApplicationUser.CustomUserName : result.ApplicationUser.UserName,
                            UserId = result.ApplicationUser.Id,
                            Category = result.CategoryName
                        });
                    }
                }
                throw new Exception();
            }
            catch
            {
                return new NotFoundResult();
            }
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            Link link = _context.Link.Where(l => l.Id.Equals(id)).FirstOrDefault();
            if (link != null)
            {
                return new JsonResult(_linkRepo.Delete(link));
            }

            return null;
        }

        [HttpPut]
        public IActionResult Update([FromBody]JObject body)
        {
            string linkId = body.GetValue("linkId").ToString();
            string title = body.GetValue("title").ToString();
            string linkURL = body.GetValue("linkURL").ToString();

            Link link = _linkRepo.GetLink(linkId);
            if (link != null)
            {
                link.Title = title;
                link.LinkURL = linkURL;
                return new JsonResult(_linkRepo.Update(link));
            }

            return null;
        }

        [HttpPost]
        public IActionResult AddComment([FromBody]JObject json)
        {
            string authorId = json.GetValue("authorId").ToString();
            string linkId = json.GetValue("linkId").ToString();
            string body = json.GetValue("body").ToString();

            Link l = _context.Link.Where(link => link.Id == linkId).FirstOrDefault();
            ApplicationUser user = _context.ApplicationUser.Where(u => u.Id == authorId).FirstOrDefault();

            try
            {
                CommentLink result = _commentLinkRepo.AddComment(new CommentLink()
                {
                    Body = body,
                    Date = DateTime.Now,
                    AuthorId = authorId,
                    LinkId = linkId,
                    Rate = 0
                });
                return new JsonResult(new CommentLinkViewModel
                {
                    Id = result.Id,
                    Body = result.Body,
                    Date = result.Date,
                    Rating = result.Rate,
                    AuthorId = result.AuthorId,
                    LinkId = result.LinkId,
                    AuthorName = _context.ApplicationUser.Where(au => au.Id == result.AuthorId).FirstOrDefault().UserName
                });
            }
            catch
            {
                return new JsonResult(new { error = "Error" });
            }
        }

        [HttpDelete]
        public IActionResult DeleteComment(string id)
        {
            CommentLink comment = _commentLinkRepo.GetComment(id);
            if (comment != null)
            {
                return new JsonResult(_commentLinkRepo.Delete(comment));
            }

            return null;
        }

        public IActionResult GetAllComments()
        {
            return new JsonResult(new { comments = _commentLinkRepo.GetAll() });
        }

        [HttpGet]
        public IActionResult IncreaseLinkRate(string id)
        {
            Link link = _context.Link.Where(l => l.Id == id).FirstOrDefault();
            if (link != null)
            {
                link.Rating += 1;
                _context.SaveChanges();
                return new JsonResult(new LinkViewModel
                {
                    Id = link.Id,
                    Title = link.Title,
                    LinkURL = link.LinkURL,
                    Rating = link.Rating,
                    Date = link.Date,
                    UserName = _context.ApplicationUser.Where(au => au.Id == link.UserId).FirstOrDefault().UserName,
                    UserId = link.UserId,
                    Category = link.CategoryName
                });
            }

            return new JsonResult(new { message = "Error. Link Not found" });
        }

        [HttpGet]
        public IActionResult DecreaseLinkRate(string id)
        {
            Link link = _context.Link.Where(l => l.Id == id).FirstOrDefault();
            if (link != null)
            {
                link.Rating -= 1;
                _context.SaveChanges();

                return new JsonResult(new LinkViewModel
                {
                    Id = link.Id,
                    Title = link.Title,
                    LinkURL = link.LinkURL,
                    Rating = link.Rating,
                    Date = link.Date,
                    UserName = _context.ApplicationUser.Where(au => au.Id == link.UserId).FirstOrDefault().UserName,
                    UserId = link.UserId,
                    Category = link.CategoryName
                });
            }

            return new JsonResult(new { message = "Error. Link Not found" });
        }
    }
}