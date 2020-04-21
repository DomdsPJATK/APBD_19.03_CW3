using System.Data.SqlClient;
using System.Threading.Tasks;
using APBD_19._03_CW3.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

namespace APBD_19._03_CW3.DAL
{
    public interface IStudentServiceDB
    {
        public IActionResult EnrollStudent(EnrollStudentRequest request);
        public IActionResult PromoteStudent(int semester, string studiesName);
        public SqlDataReader CheckValidation(LoginReguestDTO login);

    }
    
}