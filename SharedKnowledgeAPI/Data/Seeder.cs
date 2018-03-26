using Microsoft.AspNetCore.Identity;
using SharedKnowledgeAPI.Models;
using SharedKnowledgeAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Data
{
    public class Seeder
    {
        ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;

        public Seeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            InitRoles();
            InitUsers();
            InitCategories();
            InitLinks();
        }

        private void InitRoles()
        {
            RoleRepository roleRepo = new RoleRepository(_context);
            roleRepo.CreateInitialRoles();
        }

        private void InitCategories()
        {
            _context.Category.Add(new Category()
            {
                Name = "Startups"
            });
            _context.Category.Add(new Category()
            {
                Name = "Social Networks"
            });
            _context.Category.Add(new Category()
            {
                Name = "Hadrware"
            });
            _context.SaveChanges();
        }

        private void InitUsers()
        {
            ApplicationUser testIfExists = _context.ApplicationUser.Where(au => au.Email.Equals("admin@admin.com")).FirstOrDefault();
            if (testIfExists == null)
            {
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    Karma = 0,
                    UserRole = "Admin",
                    CustomUserName = "Default Administrator"
                }, "123$qwE");
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "moderator@moderator.com",
                    Email = "moderator@moderator.com",
                    Karma = 0,
                    UserRole = "Moderator",
                    CustomUserName = "Default Moderator"
                }, "123$qwE");
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "user@user.com",
                    Email = "user@user.com",
                    Karma = 0,
                    UserRole = "User",
                    CustomUserName = "Default User"
                }, "123$qwE");
            }
        }

        private void InitLinks()
        {
            ApplicationUser user = _context.ApplicationUser.Where(au => au.Email.Equals("admin@admin.com")).FirstOrDefault();
            _context.Link.Add(new Link()
            {
                Title = "Uber to sell south-east Asia business to competitor Grab",
                LinkURL = "https://www.theguardian.com/technology/2018/mar/26/uber-to-sell-south-east-asia-business-to-competitor-grab",
                Rating = 15,
                Date = DateTime.Now,
                CategoryName = "Startups",
                UserId = user.Id,
                ApplicationUser = user
            });
            _context.Link.Add(new Link()
            {
                Title = "Facebook's privacy practices are under investigation, FTC confirms",
                LinkURL = "https://www.theguardian.com/technology/2018/mar/26/facebook-data-privacy-cambridge-analytica-investigation-ftc-latest",
                Rating = -10,
                Date = DateTime.Now,
                CategoryName = "Social Networks",
                UserId = user.Id,
                ApplicationUser = user
            });
            _context.Link.Add(new Link()
            {
                Title = "Elon Musk joins #DeleteFacebook effort as Tesla and SpaceX pages vanish",
                LinkURL = "https://www.theguardian.com/technology/2018/mar/23/elon-musk-delete-facebook-spacex-tesla-mark-zuckerberg",
                Rating = 23,
                Date = DateTime.Now,
                CategoryName = "Social Networks",
                UserId = user.Id,
                ApplicationUser = user
            });
            _context.SaveChanges();
        }
    }
}