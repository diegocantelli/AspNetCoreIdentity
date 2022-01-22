using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityUnderTheHood.Pages
{
    [Authorize(Policy = "HrManagerOnly")]
    public class HRmanagerModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
