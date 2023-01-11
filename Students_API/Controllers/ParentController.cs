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
    public class ParentController : ControllerBase
    {
        private readonly IUnitOfWork _UnitOfWork;

        public ParentController(IUnitOfWork UOW)
        {
            _UnitOfWork = UOW;
        }
        [Route("GetAllMembers")]
        [HttpGet]
        public List<ParentDTO> GetAllMembers()
        {
            return _UnitOfWork.parentRepository.GetActiveMembers().ToList();
        }
        [Route("GetMemberById")]
        [HttpGet]
        public async Task<ActionResult<Parent>> GetMember([FromQuery] int id)
        {
            var Member = _UnitOfWork.parentRepository.FindById(id);

            if (Member == null)
            {
                return NotFound();
            }

            return Member;
        }

        [HttpPost]
        [Route("Post")]
        public async Task<IActionResult> PostClients(Parent Member)
        {
            try
            {
                int MemberId = Member.id;
                if (Member.id == 0)
                {
                    Member.CreatedAt = DateTime.Now;
                    _UnitOfWork.parentRepository.Add(Member);
                    await _UnitOfWork.SaveDbChanges();

                }
                else
                {
                    _UnitOfWork.parentRepository.Update(Member);
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
                var member = _UnitOfWork.parentRepository.FindById(id);
                member.IsActive = false;
                _UnitOfWork.parentRepository.Update(member);
                await _UnitOfWork.SaveDbChanges();
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            return NoContent();
        }

        [Route("GetParentById")]
        [HttpGet]
        public async Task<Parent> GetParent(int id)
        {
            var Parent = await _UnitOfWork.parentRepository.GetParentInfo(id);
            
            return Parent;
        }
    }
}
