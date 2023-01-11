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
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? StudentCode { get; set; }
        public string? HomeAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string ContactNo { get; set; }
        public string EmergencyContactNumber { get; set; }
        public string? Email { get; set; }
        public string? CourseName { get; set; }
        public bool Disability { get; set; } = false;
        public string? Parent { get; set; }
    }
}
