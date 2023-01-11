using Application.Business.Interfaces;
using Domain.Entities;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Business.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StudentDataContext _Context;
        private readonly IConfiguration _config;
        public IParentRepository parentRepository { get; private set; }

        public IStudentRepository studentRepository { get; private set; }

        public IDataAccessRepository DataAccess { get; }
        public UnitOfWork(StudentDataContext context, IConfiguration config)
        {
            _Context = context;
            _config = config;
            studentRepository = new StudentRepository(_Context);
            parentRepository = new ParentRepository(_Context);
            DataAccess = new DataAccessRepository(_Context);
        }


        public void Dispose()
        {
            _Context.Dispose();
        }

        public async Task<int> SaveDbChanges()
        {
            return await _Context.SaveChangesAsync();
        }
    }
}
