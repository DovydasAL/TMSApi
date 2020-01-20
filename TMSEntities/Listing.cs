using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TMSEntities
{
    public class Listing
    {
        [Key]
        public long ListingID { get; set; }
        public string Subject { get; set; }
        public string CourseNumber { get; set; }
        public string InstructionType { get; set; }
        public string InstructionMethod { get; set; }
        public string Section { get; set; }
        public string CRN { get; set; }
        public string Enroll { get; set; }
        public string MaxEnroll { get; set; }
        public string CourseTitle { get; set; }
        public string Times { get; set; }
        public string Instructor { get; set; }
        public Term Term { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public bool IsEqual(Listing other)
        {
            return this.Subject == other.Subject &&
                this.CourseNumber == other.CourseNumber &&
                this.InstructionType == other.InstructionType &&
                this.InstructionMethod == other.InstructionMethod &&
                this.Section == other.Section &&
                this.Enroll == other.Enroll &&
                this.MaxEnroll == other.MaxEnroll &&
                this.CourseTitle == other.CourseTitle &&
                this.Times == other.Times &&
                this.Instructor == other.Instructor;
        }
    }
}
