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
    public class StudentController : Controller
    {
        private readonly IConfiguration _Configuration;
        private readonly IWebHostEnvironment Environment;

        public StudentController(IConfiguration configuration, IWebHostEnvironment _env)
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
            List<StudentDTO> AllStudents = null;
            using (var member = new HttpClient())
            {
                member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                member.DefaultRequestHeaders.Accept.Clear();
                member.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await member.GetAsync("/Student/GetAllMembers");
                if (response.IsSuccessStatusCode)
                {
                    AllStudents = await response.Content.ReadAsAsync<List<StudentDTO>>();
                }
            }
            return View(AllStudents);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Student NewMember)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }

            var UserId = HttpContext.Session.GetObject<string>("UserId");
            if (NewMember.File != null)
            {
                string path = "/Images/ProfileImages/";
                string fileName = Path.GetFileName(NewMember.File.FileName);
                string paths = Path.Combine(Environment.WebRootPath, "Images/ProfileImages", fileName);
                NewMember.ProfilePicture = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(paths, FileMode.Create))
                {
                    NewMember.File.CopyTo(stream);
                }
            }
                NewMember.File = null;
            if (NewMember.id == 0)
            { //Remove name
                NewMember.CreatedBy = UserId;
               // NewMember.CreatedAt = DateTime.Now;
            }
            else
            { //Remove name
                NewMember.UpdatedBy = UserId;
                //NewMember.UpdatedAt = DateTime.Now;
            }
            using (var Member = new HttpClient())
            {
                //var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                Member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                Member.DefaultRequestHeaders.Accept.Clear();
                Member.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await Member.PostAsJsonAsync<Student>("/Student/Post", NewMember);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult TermsAndConditions()
        {
            return View();
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
                var DeleteMember = member.DeleteAsync("Student?id=" + id);
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
            using (var Package = new HttpClient())
            {
                Package.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                Package.DefaultRequestHeaders.Accept.Clear();
                Package.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Package.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await Package.GetAsync("/api/Parents");
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
                IEnumerable<Parent> ParentList = await response.Content.ReadAsAsync<IEnumerable<Parent>>();
                ViewBag.AllParents = new SelectList(ParentList, "id", "ParentName");
            }
            Student member = new Student();
            if (id == 0)
            {
                string RCode;
                using (var no = new HttpClient())
                {
                    no.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                    no.DefaultRequestHeaders.Accept.Clear();
                    no.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    no.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    HttpResponseMessage response = await no.GetAsync("/Student/GetStudentCode");
                    if (response.IsSuccessStatusCode)
                    {
                        RCode = await response.Content.ReadAsAsync<string>();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                member.StudentCode = RCode;
                return PartialView(member);
            }
            using (var Member = new HttpClient())
            {
                Member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                Member.DefaultRequestHeaders.Accept.Clear();
                Member.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage response = await Member.GetAsync("/Student/GetMemberById?id=" + id);
                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
                member = await response.Content.ReadAsAsync<Student>();
            }
            return PartialView(member);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAndPrint(Student NewMember)
        {
            string Token = HttpContext.Session.GetObject<string>("Token");
            if (string.IsNullOrWhiteSpace(Token))
            {
                TempData["TokenTimeout"] = "Your login session has ended. Please log in again.";
                return LocalRedirect("/Identity/Account/Login");
            }
            var UserId = HttpContext.Session.GetObject<string>("UserId");
            if (NewMember.File != null)
            {

                string path = "/Images/ProfileImages/";
                string fileName = Path.GetFileName(NewMember.File.FileName);
                string paths = Path.Combine(Environment.WebRootPath, "Images/ProfileImages", fileName);
                NewMember.ProfilePicture = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(paths, FileMode.Create))
                {
                    NewMember.File.CopyTo(stream);
                }
            }
                NewMember.File = null;
            NewMember.DateOfBirth = NewMember.DateOfBirth.Date;
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
            Parent parent = new Parent();
            using (var Member = new HttpClient())
            {
                Member.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                Member.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage PostMember = await Member.PostAsJsonAsync<Student>("/Student/Post", NewMember);
                if (!PostMember.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
            }
            using (var GetParent = new HttpClient())
            {
                GetParent.BaseAddress = new Uri(_Configuration.GetSection("API_Link").Value);
                GetParent.DefaultRequestHeaders.Accept.Clear();
                GetParent.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                GetParent.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                HttpResponseMessage Response = await GetParent.GetAsync("/api/Packages/" + NewMember.ParentId);
                if (!Response.IsSuccessStatusCode)
                {
                    return BadRequest();
                }
                parent = await Response.Content.ReadAsAsync<Parent>();
                NewMember.Parent = parent;
                NewMember.Parent.FirstName = parent.FirstName;
                NewMember.Parent.LastName = parent.LastName;
                NewMember.Parent.Phone = parent.Phone;
                NewMember.CreatedAt = DateTime.Now;
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
}
