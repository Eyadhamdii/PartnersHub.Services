using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    [HttpGet("claims")]
    public IActionResult GetClaims([FromQuery] string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenObj = handler.ReadJwtToken(token);

            var claims = tokenObj.Claims.ToList();

            return Ok(claims);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}