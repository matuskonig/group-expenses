﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Authentication;
using WebApplication.Dto;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;

        public AuthController(UserManager<ApplicationUser> userManager_, RoleManager<IdentityRole> roleManager_,
            IConfiguration configuration_)
        {
            userManager = userManager_;
            roleManager = roleManager_;
            configuration = configuration_;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password)) return Unauthorized();
            var userRoles = await userManager.GetRolesAsync(user);

            var authClaims = new[]
            {
                /*new Claim(JwtRegisteredClaimNames.GivenName, user.UserName), //TODO: mozno iba ako name*/
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }.Concat(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                //issuer: configuration["JWT:ValidIssuer"],
                //audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });

        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var userExists = await userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError /*new Response { Status = "Error", Message = "User already exists!" }*/);

            var user = new ApplicationUser
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            return result.Succeeded
                ? Ok()
                : StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto model)
        {
            var userExists = await userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError /*, new Response { Status = "Error", Message = "User already exists!" }*/);

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError /*, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." }*/);

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return Ok( /*new Response { Status = "Success", Message = "User created successfully!" }*/);
        }
    }
}