using Application.Business.Interfaces;
using Domain.DTO;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Business
{
    class DataAccessRepository : IDataAccessRepository
    {
        private readonly StudentDataContext _Context;
        public DataAccessRepository(StudentDataContext context)
        {
            _Context = context;
        }

		public async Task<List<NavigationMenuDTO>> GetPermissionsByRoleIdAsync(string id)
		{
			var items = await (from m in _Context.NavigationMenu
							   join rm in _Context.RoleMenuPermission
								on new { X1 = m.Id, X2 = id } equals new { X1 = rm.NavigationMenuId, X2 = rm.RoleId }
								into rmp
							   from rm in rmp.DefaultIfEmpty()
							   where m.Visible == true
							   select new NavigationMenuDTO()
							   {
								   Id = m.Id,
								   Name = m.Name,
								   Area = m.Area,
								   ActionName = m.ActionName,
								   ControllerName = m.ControllerName,
								   IsExternal = m.IsExternal,
								   ExternalUrl = m.ExternalUrl,
								   DisplayOrder = m.DisplayOrder,
								   ParentMenuId = m.ParentMenuId,
								   Visible = m.Visible,
								   Permitted = rm.RoleId == id
							   }).AsNoTracking().ToListAsync();
			return items;
		}

		public async Task<bool> SetPermissionsByRoleIdAsync(string id, IEnumerable<int> permissionIds)
		{
			var existing = await _Context.RoleMenuPermission.Where(x => x.RoleId == id).ToListAsync();
			_Context.RemoveRange(existing);

			foreach (var item in permissionIds)
			{
				await _Context.RoleMenuPermission.AddAsync(new RoleMenuPermission()
				{
					RoleId = id,
					NavigationMenuId = item,
				});
			}

			var result = await _Context.SaveChangesAsync();

			return result > 0;
		}

		public List<RoleMenuPermission> GetRoleMenuPermissions(IEnumerable<string> UserRoles) 
		{
			IEnumerable<string> UserRolesObjs = _Context.Roles.Where(x => UserRoles.Contains(x.Name)).Select(x => x.Id).ToList();
			return _Context.RoleMenuPermission.Where(x => UserRolesObjs.Contains(x.RoleId)).ToList();
		}
	}
}
