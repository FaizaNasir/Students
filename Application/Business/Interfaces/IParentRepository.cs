using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Business.GenericInterfaces;
using Domain.DTO;
using Domain.Entities; 
namespace Application.Business.Interfaces
{
    public interface IParentRepository : IGenericInterface<Parent>
    {
        public List<ParentDTO> GetActiveMembers();
        Task<Parent> GetParentInfo(int? id);
    }
}
