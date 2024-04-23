using Jwt.Models;
using Jwt.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IResetPasswordService _resetPasswordService;
        private readonly IAuthServices _authServices;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(IAuthServices authServices, UserManager<ApplicationUser> userManager, IResetPasswordService resetPasswordService)
        {
            _authServices = authServices;
            _userManager = userManager;
            _resetPasswordService = resetPasswordService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authServices.RegisterAsync(model);

            if (!result.isAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authServices.GetTokenAsync(model);

            if (!result.isAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] VerificationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _resetPasswordService.ForgetPassword(request);

            return Ok();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _resetPasswordService.ResetPassword(model);

            return Ok();
        }
    }
}
