
using Application.Handlers;
using AQCricket_ERP.Common.Utilities;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AQAcademy_ERP.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly SignInManager<ApplicationUsers> _signInManager;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _Configuration;
        //private readonly IEmailSender _emailSender;

        public UserRolesController(
            UserManager<ApplicationUsers> userManager,
            SignInManager<ApplicationUsers> signInManager,
            ILogger<UserController> logger, RoleManager<IdentityRole> roleManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _Configuration = config;
        }

        // [Authorize]
        public async Task<IActionResult> Index()
        {
            var RolesDTO = new List<UserRolesDTO>();
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                foreach (var item in roles)
                {
                    if (item.Name != "Super Admin")
                    {
                        RolesDTO.Add(new UserRolesDTO()
                        {
                            Id = item.Id,
                            RoleName = item.Name,
                        });
                    }
                }
                return View(RolesDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        public IActionResult _Create()
        {
            UserRolesDTO userRoles = new();
            return PartialView(userRoles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserRolesDTO userRoles)
        {

            var result = await _roleManager.CreateAsync(new IdentityRole() { Name = userRoles.RoleName });
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("Name", string.Join(",", result.Errors));
            }

            return View(userRoles);
        }
        public async Task<IActionResult> EditRolePermission(string id)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            HttpContext.Session.SetObject("RoleId", id);
            var permissions = new List<NavigationMenuDTO>();
            if (!string.IsNullOrWhiteSpace(id))
            {
                using (var itms = new HttpClient())
                {
                    itms.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                    itms.DefaultRequestHeaders.Clear();
                    itms.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    itms.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    HttpResponseMessage response = await itms.GetAsync("/api/UserRoles/GetPermissionsByRoleId/" + id);
                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest();
                    }

                    permissions = await response.Content.ReadAsAsync<List<NavigationMenuDTO>>();
                }
                //permissions = await _dataAccessService.GetPermissionsByRoleIdAsync(id);

                List<TreeViewNode> nodes = new List<TreeViewNode>();
                foreach (var item in permissions)
                {
                    nodes.Add(new TreeViewNode
                    {
                        id = item.Id,
                        parent = item.ParentMenuId.ToString() == "" ? "#" : item.ParentMenuId.ToString(),
                        text = item.Name.ToString(),
                        state = item.ParentMenuId.ToString() == "" ?
                        (item.Permitted == true ? (new state { opened = true, selected = true }) : new state { opened = true, selected = false })
                                                                    :
                        (new state { opened = (item.Permitted == true) ? true : false, selected = item.Permitted })
                    });
                }
                ViewBag.Json = JsonConvert.SerializeObject(nodes);
            }
            //state = item.ParentMenuId.ToString() == "" ? new state { opened = true, selected = false } : new state { opened = (item.Permitted == true) ? true : false, selected = item.Permitted }
            return View(permissions);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTreeItems(string selectedItems)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            List<TreeViewNode> items = JsonConvert.DeserializeObject<List<TreeViewNode>>(selectedItems);

            var childs = items.Where(a => a.parent != "#");
            var parentsId = childs.Select(x => Convert.ToInt32(x.parent));
            var childsId = childs.Select(x => Convert.ToInt32(x.id));
            var parents = items.Where(a => a.parent == "#").Select(x => Convert.ToInt32(x.id));
            string id = HttpContext.Session.GetObject<string>("RoleId");

            var permissions = parentsId.Concat(childsId).Concat(parents).Distinct();        
            using (var per = new HttpClient())
            {
                per.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                per.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage PostPermissions = await per.PostAsJsonAsync<IEnumerable<int>>("/api/UserRoles/SetPermissionsByRoleId?id=" + id, permissions);
                if (!PostPermissions.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
            return RedirectToAction(nameof(Index));
            //}
            //return RedirectToAction(nameof(Index));
        }
    }
}
