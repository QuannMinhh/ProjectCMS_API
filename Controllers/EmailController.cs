using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectCMS.Services;
using ProjectCMS.ViewModels;

namespace ProjectCMS.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> TestSendEmail(string string1, string string2)
        {
            //await _emailService.NewIdeaNotify(string1, string2);
            return Ok();
        }
    }
}
