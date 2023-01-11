using Domain.Entities;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public IFormFile File { get; set; }
        public string ProfilePicture { get; set; }
        public UserRolesDTO[] UserRoles { get; set; }
    }
}
