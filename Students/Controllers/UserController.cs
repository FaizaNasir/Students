using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AQCricket_ERP.Common.Utilities;

namespace AQAcademy_ERP.Controllers
{
    public class UserController : Controller
    {
        private readonly SignInManager<ApplicationUsers> _signInManager;
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment Environment;

        public UserController(
            UserManager<ApplicationUsers> userManager,
            SignInManager<ApplicationUsers> signInManager,
            ILogger<UserController> logger, RoleManager<IdentityRole> roleManager, IWebHostEnvironment _env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            Environment = _env;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var UserId = HttpContext.Session.GetObject<string>("UserId");
                //var users = await _userManager.Users.Where(a => a.Id != UserId && a.Email != "admin@admin.com").ToListAsync();
                var users = await _userManager.Users.ToListAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                UserDTO userDTO = new();
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var user = await _userManager.FindByIdAsync(id);
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var allRoles = await _roleManager.Roles.ToListAsync();
                    userDTO.Id = user.Id;
                    userDTO.Name = user.UserName;
                    userDTO.Email = user.Email;
                    userDTO.UserRoles = allRoles.Where(a => a.Name != "Super Admin").Select(x => new UserRolesDTO()
                    {
                        Id = x.Id,
                        RoleName = x.Name,
                        Selected = userRoles.Contains(x.Name)
                    }).ToArray();
                }
                return View(userDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserDTO obj)
        {
            var user = await _userManager.FindByIdAsync(obj.Id);
            var userRoles = await _userManager.GetRolesAsync(user);

            user.Email = obj.Email;
            user.UserName = obj.Name;
            var result = await _userManager.UpdateAsync(user);

            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRolesAsync(user, obj.UserRoles.Where(x => x.Selected).Select(x => x.RoleName));

            return RedirectToAction(nameof(Index));


            return View(obj);
        }
        public async Task<IActionResult> EditProfile(string id)
        {
            try
            {
                UserDTO userDTO = new();
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var user = await _userManager.FindByIdAsync(id);
                    userDTO.Id = user.Id;
                    userDTO.Name = user.UserName;
                    userDTO.Email = user.Email;
                    userDTO.PhoneNo = user.PhoneNumber;
                    userDTO.Address = user.Address;
                }
                return View(userDTO);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(UserDTO obj)
        {
            try
            {
                if (obj.File != null)
                {

                    string path = "/Images/ProfileImages/";
                    string fileName = Path.GetFileName(obj.File.FileName);
                    string paths = Path.Combine(Environment.WebRootPath, "Images/ProfileImages", fileName);
                    obj.ProfilePicture = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(paths, FileMode.Create))
                    {
                        obj.File.CopyTo(stream);
                    }
                }
                var user = await _userManager.FindByIdAsync(obj.Id);
                user.UserName = obj.Name;
                user.PhoneNumber = obj.PhoneNo;
                user.Address = obj.Address;
               
                await _userManager.UpdateAsync(user);
                HttpContext.Session.SetObject("User", user);
                return LocalRedirect("/Home/Index");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
