using Jwt.Helpers;
using Jwt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jwt.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly JWT _jwt;
        public AuthServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> RoleManager, IOptions<JWT>jwt)
        {
            _userManager = userManager;
            _RoleManager=RoleManager;
            _jwt = jwt.Value;
        }
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email IS Already Registered" };
            if (await _userManager.FindByNameAsync(model.FirstName) is not null)
                return new AuthModel { Message = "FirstName IS Already Registered" };
            var user = new ApplicationUser
            {
                UserName = model.FirstName+model.LastName,
                Email = model.Email,
                Firstname = model.FirstName,
                Lastname = model.LastName,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = model.Password
            };
           var Result= await _userManager.CreateAsync(user,model.Password);
            if(!Result.Succeeded)
            {
                var Errors= string.Empty;

                foreach (var error in Result.Errors)
                {
                    Errors+=$"{error.Description},";
                }
                return new AuthModel { Message =Errors};
            }
            await _userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                isAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.Firstname
            };
        }
        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authmodel=new AuthModel();
            var user=await _userManager.FindByEmailAsync(model.Email);
            if (user is null|| !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authmodel.Message = "Email Or Password Is Incorrect";
                return authmodel;
            } 
            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);
            authmodel.isAuthenticated = true;
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authmodel.Email = user.Email;
            authmodel.Username = user.Firstname + user.Lastname;
            authmodel.ExpiresOn = jwtSecurityToken.ValidTo;
            authmodel.Roles= rolesList.ToList();
            return authmodel;
        }
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserID);
            if (user is null|| !await _RoleManager.RoleExistsAsync(model.Role))
                return "Invalid User Id Or Role";
            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";
            var result= await _userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? string.Empty : "Something Went Wrong";
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationsInDays),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
    }
}
