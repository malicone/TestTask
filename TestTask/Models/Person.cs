using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestTask.Models
{
    public class Person
    {
        [Display(Name = "#")]
        public int Id { get; set; }

        [Display( Name = "First name" )]
        public string FirstName { get; set; }

        [Display( Name = "Last name" )]
        public string LastName { get; set; }

        [DataType( DataType.Date )]
        [Display( Name = "B-day" )]
        public DateTime BirthdayDate { get; set; }
    }
}