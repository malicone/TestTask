using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TestTask.Models
{
    public enum InsertDirection { Top, Bottom };

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

        // max 20190601: we can add key to Id field
        public int? NextId { get; set; }

        [NotMapped]
        public int BasePersonId { get; set; }

        [NotMapped]
        public InsertDirection Direction { get; set; }
    }
}