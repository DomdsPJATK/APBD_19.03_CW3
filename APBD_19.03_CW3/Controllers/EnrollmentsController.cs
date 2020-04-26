using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using APBD_19._03_CW3.DAL;
using APBD_19._03_CW3.DTOs.Request;
using APBD_19._03_CW3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace APBD_19._03_CW3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {    
        
        private IStudentServiceDB _service;
        public IConfiguration Configuration { get; set; }

        public EnrollmentsController(IStudentServiceDB studentServiceDb, IConfiguration configuration)
        {
            _service = studentServiceDb;
        }

        [HttpPost]
        [Authorize(Policy = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            return _service.EnrollStudent(request);
        }
        
        [HttpPost("promotions")]
        [Authorize(Policy = "employee")]
        public IActionResult promoteStudent(PromoteStudentRequest request)
        {
            return _service.PromoteStudent(request.semester, request.studies);
        }
        
        [HttpPost]
        [Authorize]
        public IActionResult Login(LoginReguestDTO login)
        {
            if(!_service.CheckValidation(login).Read()) return new BadRequestResult();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, login.Login),
                new Claim(ClaimTypes.Name, "Admin")
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Domds",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );
            
            return new OkObjectResult(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
            
        }
        
        
    }
}