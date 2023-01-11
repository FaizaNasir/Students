using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Domain.DTO;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Xml.Linq;
using AQCricket_ERP.Common.Utilities;
using System.Diagnostics.Metrics;
using Application.Business.UnitOfWork;

namespace Students.Controllers
{
    public class ParentController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IWebHostEnvironment Environment;

        public ParentController(IConfiguration configuration, IWebHostEnvironment _env)
        {
            _Configuration = configuration;
            Environment = _env;
        }
        public async Task<IActionResult> Index()
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            List<ParentDTO> AllParents = null;
            using (var member = new HttpClient())
            {
                member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                member.DefaultRequestHeaders.Accept.Clear();
                member.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await member.GetAsync("/Parent/GetAllMembers");
                if (response.IsSuccessStatusCode)
                {
                    AllParents = await response.Content.ReadAsAsync<List<ParentDTO>>();
                }
            }
            return View(AllParents);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Parent NewMember)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }

            var UserId = HttpContext.Session.GetObject<string>("UserId");
            if (NewMember.id == 0)
            { //Remove name
                NewMember.CreatedBy = UserId;
            }
            else
            { //Remove name
                NewMember.UpdatedBy = UserId;
            }
            using (var Member = new HttpClient())
            {
                Member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                Member.DefaultRequestHeaders.Accept.Clear();
                Member.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await Member.PostAsJsonAsync<Parent>("/Parent/Post", NewMember);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
            return RedirectToAction("Index");
        }
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            using (var member = new HttpClient())
            {
                member.BaseAddress = new Uri(_Configuration.GetSection("Api_Link").Value);
                member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                //HTTP POST
                var DeleteMember = member.DeleteAsync("Parent?id=" + id);
                DeleteMember.Wait();

                var result = DeleteMember.Result;
                if (!result.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
            return Json("OK");
        }
        public async Task<IActionResult> _Create(int id)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
           
            Parent member = new Parent();
            using (var Member = new HttpClient())
            {
                Member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                Member.DefaultRequestHeaders.Accept.Clear();
                Member.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await Member.GetAsync("/Parent/GetMemberById?id=" + id);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
                member = await response.Content.ReadAsAsync<Parent>();
            }
            return PartialView(member);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAndPrint(Parent NewMember)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            var UserId = HttpContext.Session.GetObject<string>("UserId");
            NewMember.DateOfBirth = NewMember.DateOfBirth.GetValueOrDefault();
            if (NewMember.id == 0)
            {
                //Remove name
                NewMember.CreatedBy = UserId;
            }
            else
            {
                //Remove name
                NewMember.UpdatedBy = UserId;
            }
            using (var Member = new HttpClient())
            {
                Member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                Member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage PostMember = await Member.PostAsJsonAsync<Parent>("/Parent/Post", NewMember);
                if (!PostMember.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
         
            var pdf = new Rotativa.AspNetCore.ViewAsPdf(NewMember)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
            var byteArray = await pdf.BuildFile(ControllerContext);
            return File(byteArray, "application/pdf");
        }
    }
}
