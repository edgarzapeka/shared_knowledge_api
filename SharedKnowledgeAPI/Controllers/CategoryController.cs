using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharedKnowledgeAPI.Data;
using SharedKnowledgeAPI.Repositories;

namespace SharedKnowledgeAPI.Controllers
{
    public class CategoryController : Controller
    {
        CategoryRepository _categoryRepo;
        public CategoryController(ApplicationDbContext context)
        {
            _categoryRepo = new CategoryRepository(context);
        }
        public IActionResult GetAll()
        {
            return new JsonResult(_categoryRepo.GetAll());
        }
    }
}