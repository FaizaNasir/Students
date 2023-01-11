using Application.Business.UnitOfWork;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Students_API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;

        public StudentController(IUnitOfWork UOW)
        {
            _UnitOfWork = UOW;
        }
        [Route("GetAllMembers")]
        [HttpGet]
        public List<StudentDTO> GetAllMembers()
        {
            return _UnitOfWork.studentRepository.GetActiveMembers().ToList();
        }
        [Route("GetMemberById")]
        [HttpGet]
        public async Task<ActionResult<Student>> GetMember([FromQuery] int id)
        {
            var Member = _UnitOfWork.studentRepository.FindById(id);

            if (Member == null)
            {
                return NotFound();
            }

            return Member;
        }

        [HttpPost]
        [Route("Post")]
        public async Task<IActionResult> PostClients(Student Member)
        {
            try
            {
                //Parent ParentInfo = _UnitOfWork.parentRepository.GetParentInfo(Member.ParentId);
                int MemberId = Member.id;
                if (Member.id == 0)
                {
                    Member.CreatedAt = DateTime.Now;
                  //  Member.Parent = ParentInfo;
                    _UnitOfWork.studentRepository.Add(Member);
                    await _UnitOfWork.SaveDbChanges();

                }
                else
                {
                    _UnitOfWork.studentRepository.Update(Member);
                    await _UnitOfWork.SaveDbChanges();
                }

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMember([FromQuery] int id)
        {
            try
            {
                var member = _UnitOfWork.studentRepository.FindById(id);
                member.IsActive = false;
                _UnitOfWork.studentRepository.Update(member);
                await _UnitOfWork.SaveDbChanges();
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return NoContent();
        }

        [Route("GetStudentById")]
        [HttpGet]
        public async Task<Student> GetStudent(int id)
        {
            var Student = await _UnitOfWork.studentRepository.GetStudentInfo(id);
            
            return Student;
        }
    }
}
