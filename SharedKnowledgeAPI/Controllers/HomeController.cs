using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedKnowledgeAPI.Data;
using SharedKnowledgeAPI.Models;
using SharedKnowledgeAPI.Repositories;

namespace SharedKnowledgeAPI.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

       

       
    }
}
