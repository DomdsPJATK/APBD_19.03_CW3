using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Xml.Serialization;
using APBD_19._03_CW3.DAL;
using APBD_19._03_CW3.DTOs.Request;
using APBD_19._03_CW3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace APBD_19._03_CW3.Controllers
{

    [ApiController]
    [Route("api/students")]
    
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly IStudentServiceDB _studentServiceDb;
        public IConfiguration Configuration { get; set; }

        public StudentsController(IDbService dbService, SqlStudentServiceDb studentServiceDb, IConfiguration configuration)
        {
            Configuration = configuration;
            _dbService = dbService;
            _studentServiceDb = studentServiceDb;
        }

        [HttpGet]

        public IActionResult MessageToUser()
        {
            return Ok("Dopisz: database || database/nrindexu");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id)
        {
            return Ok("Aktualizacja zakończona");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }

        [HttpGet("database/{idStudent}")]
        public IActionResult GetStudents(string idStudent)
        {
            return Ok(GetStudentQuery("Select * from Student, Studies, Enrollment WHERE Student.IdEnrollment = Enrollment.IdEnrollment AND Enrollment.IdStudy = Studies.IdStudy AND Student.IndexNumber = @idStudent",idStudent));
        }
        
        [HttpGet("database")]
        public IActionResult GetAllStudents()
        {
            return Ok(GetStudentQuery("Select * from Student, Studies, Enrollment WHERE Student.IdEnrollment = Enrollment.IdEnrollment AND Enrollment.IdStudy = Studies.IdStudy",""));
        }

        public List<Student> GetStudentQuery(string str,string par)
        {
            var students = new List<Student>();
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19036;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                com.CommandText = str;
                if (par != "")
                {
                    com.Parameters.AddWithValue("idStudent", par);
                }
                
                client.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var student = new Student()
                    {
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = DateTime.Parse(dr["BirthDate"].ToString()),
                        Studies = new Studies()
                        {
                            name = dr["Name"].ToString()
                        },
                        Semester = Int32.Parse(dr["Semester"].ToString())
                    };
                    students.Add(student);
                }
                client.Close();
            }
            
            return students;
        }

        [HttpPost]
        public IActionResult Login(LoginReguestDTO loginReguest)
        {
            return _studentServiceDb.CheckUserValidation(loginReguest);
        }
        
    }
}