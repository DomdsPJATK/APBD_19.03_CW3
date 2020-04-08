using APBD_19._03_CW3.DAL;
using APBD_19._03_CW3.DTOs.Request;
using APBD_19._03_CW3.Model;
using Microsoft.AspNetCore.Mvc;

namespace APBD_19._03_CW3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {    
        
        private IStudentServiceDB _service;

        public EnrollmentsController(IStudentServiceDB studentServiceDb)
        {
            _service = studentServiceDb;
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            return _service.EnrollStudent(request);
        }
        
        [HttpPost("promotions")]
        public IActionResult promoteStudent(PromoteStudentRequest request)
        {
            return _service.PromoteStudent(request.semester, request.studies);
        }
        
        
    }
}