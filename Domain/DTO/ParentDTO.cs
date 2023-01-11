using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain.DTO
{
    public class ParentDTO
    {
        public int ParentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string IdNumber { get; set; }
        public string? HomeAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Phone { get; set; }

    }
}
