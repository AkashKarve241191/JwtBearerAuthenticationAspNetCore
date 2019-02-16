using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtAuthentication.Controller1
{
    public class TokenGeneratorController : Controller
    {
        [HttpGet("api/[controller]/[action]")]
        public IActionResult RequestToken()
        {
            var claims = new[]
        {
            new Claim("TestClaim","TestValue"),
            new Claim(ClaimTypes.Role,"Manager")
        };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("TestKey1234567845454545454545454545454"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        // NOTE:  // Need to send token in Authorization Header as "Bearer TokenValue"
        [HttpGet("api/[controller]/IsUserValid"), Authorize]
        public IActionResult IsUSerValid()
        {
            var userClaims = User.Claims;
            return Ok(true);
        }

        // NOTE:  // Need to send token in Authorization Header as "Bearer TokenValue"
        [HttpGet("api/[controller]/IsManager"), Authorize(Policy = "ManagerPolicy")]
        public IActionResult IsManager()
        {
            var userClaims = User.Claims;
            return Ok(true);
        }
    }
}
