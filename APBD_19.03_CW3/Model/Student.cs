﻿using System;
using System.Data;
using System.Text.Json.Serialization;
using APBD_19._03_CW3.DAL;

namespace APBD_19._03_CW3.Model
{
    public class Student
    {
        
        public int IdStudent { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IndexNumber { get; set; }
        
        [JsonConverter(typeof(CustonDateTimeConverter))]
        public DateTime BirthDate { get; set; }
        
        [JsonConverter(typeof(CustomStudiesConverter))]
        public Studies Studies { get; set; }

        public int Semester { get; set; }

    }
}