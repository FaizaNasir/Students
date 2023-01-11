using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class UserRolesDTO
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
        public List<string> Users { get; set; }
    }
}
