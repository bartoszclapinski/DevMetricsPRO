using DevMetricsPro.Application.DTOs.Auth;
using DevMetricsPro.Application.Interfaces;
using DevMetricsPro.Core.Entities;
using DevMetricsPro.Core.Exceptions;
using DevMetricsPro.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Linq;

namespace DevMetricsPro.Web.Controllers;

/// <summary>
/// API controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting(RateLimitingConfiguration.ApiPolicy)]
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
    [EnableRateLimiting(RateLimitingConfiguration.AuthPolicy)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        EnsureModelStateIsValid();

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
            var identityErrors = string.Join(" | ", result.Errors.Select(e => e.Description));
            throw new ValidationException(identityErrors);
        }

        // Future enhancement: Add default role assignment after implementing role seeding
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
    [EnableRateLimiting(RateLimitingConfiguration.AuthPolicy)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        EnsureModelStateIsValid();

        // Find model by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Invalid login attempt for {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid e-mail or password");
        }

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
                throw new BusinessRuleException("Account locked due to multiple failed attempts");
            }
            throw new UnauthorizedAccessException("Invalid e-mail or password");
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

    private void EnsureModelStateIsValid()
    {
        if (ModelState.IsValid)
        {
            return;
        }

        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) 
                ? e.Exception?.Message 
                : e.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToList();

        var message = errors.Any()
            ? string.Join(" | ", errors)
            : "Invalid request payload";

        throw new ValidationException(message);
    }
}