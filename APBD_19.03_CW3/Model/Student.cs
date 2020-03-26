using System;
using System.Data;

namespace APBD_19._03_CW3.Model
{
    public class Student
    {
        
        public int IdStudent { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        
        public DateTime BirthDate { get; set; }
        
        public Studies Studies { get; set; }

        public int Semester { get; set; }

    }
}