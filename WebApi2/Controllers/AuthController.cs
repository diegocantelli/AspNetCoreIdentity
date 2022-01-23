using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] Credential credential)
        {
            if (credential.UserName == "Admin" && credential.Password == "123")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@admin.com"),
                    new Claim("Admin", "true"),
                    new Claim("HrManagerOnly", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2021-09-01"),
                    //Com esta claim será possível acessar a página de HumanResource
                    new Claim("Department", "HR")
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    access_token = "",
                    expires_at = expiresAt
                });
            }
            ModelState.AddModelError("Unauthorized","You're not authorized to access this resource.");
            return Unauthorized(ModelState);
        }
    }
    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
