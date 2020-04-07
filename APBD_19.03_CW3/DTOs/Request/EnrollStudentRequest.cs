using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using APBD_19._03_CW3.DAL;
using APBD_19._03_CW3.Model;

namespace APBD_19._03_CW3.DTOs.Request
{
    public class EnrollStudentRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string IndexNumber { get; set; }
        
        [Required]
        [JsonConverter(typeof(CustonDateTimeConverter))]
        public DateTime BirthDate { get; set; }
        
        [Required]
        [JsonConverter(typeof(CustomStudiesConverter))]
        public Studies Studies { get; set; }
    }
}