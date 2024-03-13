using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserManagement.Data.Models;

namespace UserManagement.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    
    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;

    }

    public async Task<(int, string)> Registeration(RegistrationModel model, string role)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return (0, "User already exists");

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };
        var createUserResult = await _userManager.CreateAsync(user, model.Password);
        if (!createUserResult.Succeeded)
            return (0, "User creation failed! Please check user details and try again.");

        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));

        if (await _roleManager.RoleExistsAsync(role))
            await _userManager.AddToRoleAsync(user, role);

        return (1, "User created successfully!");
    }

    public async Task<TokenViewModel> Login(LoginModel model)
    {
        var tokenViewModel = new TokenViewModel();
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null)
        {
            tokenViewModel.StatusCode = 0;
            tokenViewModel.StatusMessage = "Invalid username";
            return tokenViewModel;
        }
        if (!await _userManager.CheckPasswordAsync(user, model.Password))
        {
            tokenViewModel.StatusCode = 0;
            tokenViewModel.StatusMessage = "Invalid password";
            return tokenViewModel;
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
           new Claim(ClaimTypes.Name, user.UserName),
           new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }
        tokenViewModel.AccessToken = GenerateToken(authClaims);
        tokenViewModel.RefreshToken = GenerateRefreshToken();
        tokenViewModel.StatusCode = 1;
        tokenViewModel.StatusMessage = "Success";

        var _RefreshTokenValidityInDays = Convert.ToInt64(_configuration["JWTKey:RefreshTokenValidityInDays"]);
        user.RefreshToken = tokenViewModel.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_RefreshTokenValidityInDays);
        await _userManager.UpdateAsync(user);

        
        return tokenViewModel;
    }

    public async Task<TokenViewModel> GetRefreshToken(GetRefreshTokenViewModel model)
    {
        var tokenViewModel = new TokenViewModel();
        var principal = GetPrincipalFromExpiredToken(model.AccessToken);
        string username = principal.Identity.Name;
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            tokenViewModel.StatusCode = 0;
            tokenViewModel.StatusMessage = "Invalid access token or refresh token";
            return tokenViewModel;
        }

        var authClaims = new List<Claim>
        {
           new Claim(ClaimTypes.Name, user.UserName),
           new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        var newAccessToken = GenerateToken(authClaims);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        tokenViewModel.StatusCode = 1;
        tokenViewModel.StatusMessage = "Success";
        tokenViewModel.AccessToken = newAccessToken;
        tokenViewModel.RefreshToken = newRefreshToken;
        return tokenViewModel;
    }


    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
        var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInHour"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _configuration["JWTKey:ValidIssuer"],
            Audience = _configuration["JWTKey:ValidAudience"],
            //Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
