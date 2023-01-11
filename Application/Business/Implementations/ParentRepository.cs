using Application.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Domain.Entities;
using Domain.DTO;
using Application.Business.GenericInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Business
{
    class ParentRepository : GenericImplementation<Parent>, IParentRepository
    {
        private readonly StudentDataContext _Context;
        public ParentRepository(StudentDataContext _context) : base(_context)
        {
            _Context = _context;
        }

        public List<ParentDTO> GetActiveMembers()
        {
            return (from r in _Context.Parents.Where(x => x.IsActive == true)
                    select new ParentDTO
                    {
                        ParentID = r.id,
                        FirstName = r.FirstName,
                        LastName = r.LastName,
                        Phone = r.Phone,
                        Email = r.Email,
                        IdNumber = r.IdNumber,
                        HomeAddress = r.HomeAddress,
                        DateOfBirth = r.DateOfBirth
                    }).OrderBy(r => r.FirstName).ToList();
        }

        public async Task<Parent> GetParentInfo(int? id)
        {
            return await _Context.Parents.FirstOrDefaultAsync(x => x.id == id);
        }

    }
}
