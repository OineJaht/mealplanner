using System.Security.Claims;
using MealPlanner.Dto;
using MealPlanner.JWT;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MealPlanner.Models.Inventory;

namespace MealPlanner.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IWebHostEnvironment _hostEnvironment;

    public AccountController(
        RoleManager<IdentityRole> roleManager,
        IWebHostEnvironment hostEnvironment,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        IConfiguration configuration,
        ILogger<AccountController> logger,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
        _roleManager = roleManager;
        _hostEnvironment = hostEnvironment;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Invalid input data"
            });
        }

        // Map DTO to domain model
        var user = new ApplicationUser
        {
            UserName = registerDto.Username,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Created = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = string.Join(", ", errors)
            });
        }

        // Generate tokens after successful registration
        var tokens = _jwtService.GenerateTokens(user);

        // Store refresh token in the database
        user.RefreshToken = tokens.RefreshToken;
        var refreshTokenExpiration = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenExpiration);

        await _userManager.UpdateAsync(user);

        return Ok(new ApiResponseDto<TokenDto>
        {
            Success = true,
            Message = "Registration successful",
            Data = tokens
        });
    }

    [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
{
    _logger.LogInformation("Login attempt for username: {Username}", loginDto.Username);
    
    if (Request.Headers.ContainsKey("X-Requested-With") == false)
    {
        Request.Headers.Add("X-Requested-With", "XMLHttpRequest");
    }
    
    if (!ModelState.IsValid)
    {
        return BadRequest(new ApiResponseDto<object>
        {
            Success = false,
            Message = "Invalid input data"
        });
    }

    // Try to find user by username first, then by email
    var user = await _userManager.FindByNameAsync(loginDto.Username);
    if (user == null)
    {
        _logger.LogInformation("User not found by username, trying by email");
        user = await _userManager.FindByEmailAsync(loginDto.Username);
    }
    
    if (user == null)
    {
        _logger.LogWarning("User not found for login attempt: {Username}", loginDto.Username);
        return Unauthorized(new ApiResponseDto<object>
        {
            Success = false,
            Message = "Invalid username or password"
        });
    }

    _logger.LogInformation("Found user: ID={UserId}, Username={Username}, Email={Email}", 
        user.Id, user.UserName, user.Email);

    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
    if (!result.Succeeded)
    {
        _logger.LogWarning("Password check failed for user: {UserId}", user.Id);
        string errorMessage = "Invalid username or password";

        if (result.IsLockedOut)
        {
            errorMessage = "Account is locked out";
        }
        else if (result.IsNotAllowed)
        {
            errorMessage = "Account is not allowed to sign in";
        }
        else if (result.RequiresTwoFactor)
        {
            errorMessage = "Two factor authentication required";
        }

        return Unauthorized(new ApiResponseDto<object>
        {
            Success = false,
            Message = errorMessage
        });
    }

    _logger.LogInformation("Password check succeeded for user: {UserId}", user.Id);

    var tokens = _jwtService.GenerateTokens(user);
    
    _logger.LogInformation("Generated tokens for user: {UserId}", user.Id);

    // Store refresh token in the database
    user.RefreshToken = tokens.RefreshToken;
    var refreshTokenExpiration = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenExpiration);
    
    await _userManager.UpdateAsync(user);

    return Ok(new ApiResponseDto<TokenDto>
    {
        Success = true,
        Message = "Login successful",
        Data = tokens
    });
}

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Invalid input data"
            });
        }

        string accessToken = refreshTokenDto.AccessToken;
        string refreshToken = refreshTokenDto.RefreshToken;

        var principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);
        var username = principal.Identity?.Name;
        
        if (username == null)
        {
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Invalid access token"
            });
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user == null || 
            user.RefreshToken != refreshToken || 
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = "Invalid refresh token or token expired"
            });
        }

        var newTokens = _jwtService.GenerateTokens(user);
        
        user.RefreshToken = newTokens.RefreshToken;
        var refreshTokenExpiration = int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenExpiration);
        
        await _userManager.UpdateAsync(user);

        return Ok(new ApiResponseDto<TokenDto>
        {
            Success = true,
            Message = "Token refreshed successfully",
            Data = newTokens
        });
    }
    
    [HttpGet("whoami")]
    [Authorize]
    public async Task<IActionResult> WhoAmI()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
    
        var user = await _userManager.FindByIdAsync(userId);
    
        return Ok(new {
            JwtClaims = new {
                UserId = userId,
                UserName = userName,
                Email = userEmail
            },
            DatabaseUser = user == null ? null : new {
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName
            },
            AllClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }
    
    
    
    
    

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name;
        if (username == null)
        {
            return BadRequest(new ApiResponseDto<object>
            {
                Success = false,
                Message = "User not authenticated"
            });
        }

        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return NotFound(new ApiResponseDto<object>
            {
                Success = false,
                Message = "User not found"
            });
        }

        // Invalidate refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Ok(new ApiResponseDto<object>
        {
            Success = true,
            Message = "Logged out successfully"
        });
    }
}
