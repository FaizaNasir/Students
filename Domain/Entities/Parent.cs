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
    public class Parent : GenericModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }

        [Display(Name = "I.D. #")]
        public string IdNumber { get; set; }
        public string? HomeAddress { get; set; }

        [Display(Name = "D.O.B")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Phone 1")]
        public string? Phone { get; set; }
        [Display(Name = "Work Phone")]
        public string? WorkPhone { get; set; }
        [Display(Name = "Home Phone")]
        public string? HomePhone { get; set; }
    }
}
