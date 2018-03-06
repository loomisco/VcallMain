using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Vcall.Models
{
    public class Person
    {
        [Required]
        [StringLength(15)]
        [Column(TypeName ="varchar")]
        [RegularExpression(@"\d{6,10}", ErrorMessage ="Invalid Account Number")]
        [Display (Name ="Account #")]
        public int AccountNumber { get; set; }
        [Required ]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]

        public string OtherPID { get; set; }
        [Required]
        [Display(Name = "Employee ID")]

        public string LastName { get; set; }
        [Required]
        [Display(Name = "Middle Initial")]
        public string MidInit { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Phone")]
        public string phone { get; set; }
        [Required]
        [Display(Name = "DOB")]
        public string DOB { get; set; }
        public string Name
        {
            get
            {
                return string.Format("{0} {2}  {1}", this.FirstName, this.LastName, this.MidInit);
            }
        }


    }
}