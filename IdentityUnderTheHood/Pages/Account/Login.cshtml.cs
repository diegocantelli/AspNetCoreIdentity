using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityUnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }
        public void OnGet()
        {
        }

        //M�todo chamado quando um post � efetuado na tela
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if(Credential.UserName == "Admin" && Credential.Password == "123")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@admin.com"),
                    new Claim("Admin", "true"),
                    new Claim("HrManagerOnly", "true"),
                    new Claim("Manager", "true"),
                    new Claim("EmploymentDate", "2021-09-01"),
                    //Com esta claim ser� poss�vel acessar a p�gina de HumanResource
                    new Claim("Department", "HR")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                var autProperties = new AuthenticationProperties
                {
                    //Define se o cookie ser� persistente ou n�o
                    IsPersistent = Credential.RememberMe
                };

                //SignInAsync -> ir� serializar o conte�do de claimsPrincipal, ir� encript�-lo e salvar como cookie no 
                //contexto Http
                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, autProperties);

                return RedirectToPage("/Index");
            }

            return Page();
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
