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
    class StudentRepository : GenericImplementation<Student>, IStudentRepository
    {
        private readonly StudentDataContext _Context;
        public StudentRepository(StudentDataContext _context) : base(_context)
        {
            _Context = _context;
        }

        public List<StudentDTO> GetActiveMembers()
        {
            return (from r in _Context.Students.Where(x => x.IsActive == true)
                    join p in _Context.Parents on r.ParentId equals p.id
                    select new StudentDTO
                    {
                        StudentId = r.id,
                        FirstName = r.FirstName,
                        LastName = r.LastName,
                        ContactNo = r.ContactNo,
                        EmergencyContactNumber = r.EmergencyContactNumber,
                        Disability = r.Disability,
                        Age = r.Age,
                        CourseName = r.CourseName,
                        HomeAddress = r.HomeAddress,
                        Email = r.Email,
                    }).OrderBy(r=>r.LastName).ToList();
        }

        public async Task<Student> GetStudentInfo(int? id)
        {
            return await _Context.Students.Include(x => x.Parent).FirstOrDefaultAsync(x => x.id == id);
        }

    }
}
