using Domain.DTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Business.Interfaces
{
    public interface IDataAccessRepository
    {
        Task<List<NavigationMenuDTO>> GetPermissionsByRoleIdAsync(string id);
        Task<bool> SetPermissionsByRoleIdAsync(string id, IEnumerable<int> permissionIds);
        List<RoleMenuPermission> GetRoleMenuPermissions(IEnumerable<string> UserRoles);
    }
}
