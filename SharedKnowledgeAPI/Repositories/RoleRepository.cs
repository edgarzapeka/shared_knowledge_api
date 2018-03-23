using Microsoft.AspNetCore.Identity;
using SharedKnowledgeAPI.Data;
using SharedKnowledgeAPI.Models.AccountViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedKnowledgeAPI.Repositories
{
    public class RoleRepository
    {
        ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            this._context = context;
            var rolesCreated = CreateInitialRoles();
        }

        public List<RoleViewModel> GetAllRoles()
        {
            var roles = _context.Roles;
            List<RoleViewModel> roleList = new List<RoleViewModel>();

            foreach (var item in roles)
            {
                roleList.Add(new RoleViewModel() { RoleName = item.Name, Id = item.Id });
            }
            return roleList;
        }

        public RoleViewModel GetRole(string roleName)
        {
            var role = _context.Roles.Where(r => r.Name == roleName).FirstOrDefault();
            if (role != null)
            {
                return new RoleViewModel() { RoleName = role.Name, Id = role.Id };
            }
            return null;
        }

        public bool CreateRole(string roleName)
        {
            var role = GetRole(roleName);
            if (role != null)
            {
                return false;
            }
            _context.Roles.Add(new IdentityRole
            {
                Name = roleName,
                Id = roleName,
                NormalizedName = roleName.ToLower()
            });
            _context.SaveChanges();
            return true;
        }

        public bool CreateInitialRoles()
        {
            // Create roles if none exist.
            // This is a simple way to do it but it would be better to use a seeder.
            string[] roleNames = { "Admin", "Moderator", "User" };
            foreach (var roleName in roleNames)
            {
                var created = CreateRole(roleName);
                // Role already exists so exit.
                if (!created)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
