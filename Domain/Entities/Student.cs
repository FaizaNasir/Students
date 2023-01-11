using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Student : GenericModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? StudentCode { get; set; }

        [Display(Name = "Father's Name")]
        public string FatherName { get; set; }

        [Display(Name = "I.D. #")]
        public string IdNumber { get; set; }
        public string? HomeAddress { get; set; }

        [Display(Name = "D.O.B")]
        public DateTime DateOfBirth { get; set; }

        public int? Age { get; set; }
        public string? Gender { get; set; }
        [Display(Name = "Phone")]
        public string ContactNo { get; set; }

        [Display(Name = "Emergency Phone")]
        public string EmergencyContactNumber { get; set; }
        public string? Email { get; set; }
        public string? CourseName { get; set; } 
        public bool Disability { get; set; } = false;

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }
        public Parent? Parent { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
