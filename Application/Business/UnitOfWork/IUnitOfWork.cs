using Application.Business.Interfaces;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Business.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository studentRepository { get; }
        IParentRepository parentRepository { get; }
        IDataAccessRepository DataAccess { get; }
        

        Task<int> SaveDbChanges();
    }
}
