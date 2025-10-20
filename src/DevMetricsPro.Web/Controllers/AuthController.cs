using DevMetricsPro.Application.DTOs.Auth;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevMetricsPro.Web.Controllers;

/// <summary>
/// API controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService,
        ILogger<AuthController> logger,
        IConfiguration configuration
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        // Check if the model is valid
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Create a new user
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
        };

        // Attempt to create the user
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            return BadRequest(ModelState);
        }

        // TODO: Add default role after implementing role seeding
        // await _userManager.AddToRoleAsync(user, "User");

        // Generate JWT token
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync();

        _logger.LogInformation("User {Email} registered successfully", user.Email);

        var expirationMinutes = Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"]);
        
        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            DisplayName = user.UserName,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            RefreshToken = refreshToken
        });        
    }

    /// <summary>
    /// Login a user
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        // Check if the model is valid
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Find model by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Unauthorized("Invalid e-mail or password");

        // Check password
        var result = await _signInManager.CheckPasswordSignInAsync(
            user, 
            request.Password, 
            lockoutOnFailure: true
        );

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {Email} has been locked out", user.Email);
                return Unauthorized(new { message = "Account locked due to multiple failed attempts"});
            }
            return Unauthorized(new { message = "Invalid e-mail or password"});
        }

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        // Generate JWT token
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync();

        _logger.LogInformation("User {Email} logged in successfully", user.Email);

        var expirationMinutes = Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"]);
        
        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email!,
            DisplayName = user.UserName,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
            RefreshToken = refreshToken
        });
    }


}