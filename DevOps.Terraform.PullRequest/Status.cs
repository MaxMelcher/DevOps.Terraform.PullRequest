using Microsoft.AspNetCore.Mvc;

namespace DevOps.Terraform.PullRequest
{
    [Route("api/[controller]")]
    public class Status : Controller
    {
        // GET
        public IActionResult Hook()
        {
            return new StatusCodeResult(200);
        }
    }
}