using System.Security.Claims;
using Application.DTOs;
using Application.Factories;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IRepository<User> _repository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly UserFactory _userFactory;
    private readonly JwtTokenService _jwtTokenService;
    
    public UsersController(IRepository<User> repository,
        IPasswordHasher<User> passwordHasher,
        UserFactory userFactory,
        JwtTokenService jwtTokenService)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _userFactory = userFactory;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO userInfo)
    {
        if (string.IsNullOrEmpty(userInfo.Email) || string.IsNullOrEmpty(userInfo.Password))
        {
            return BadRequest("Email or password is incorrect");
        }
        
        var user = await _repository.Read(u => u.Email.Equals(userInfo.Email));

        if (user == null)
        {
            return BadRequest("Email does not exist.");
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.Password, userInfo.Password) != PasswordVerificationResult.Success)
        {
            return Unauthorized("Invalid password");
        }
        
        var token = _jwtTokenService.GenerateToken(user.Id, user.Password, "Standard");
        
        return Ok(new { Token = token });
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserLoginDTO userInfo)
    {
        if (string.IsNullOrEmpty(userInfo.Email) || string.IsNullOrEmpty(userInfo.Password))
        {
            return BadRequest("Email and password cannot be empty");
        }

        var user = _userFactory.Create(userInfo);
        user.Password = _passwordHasher.HashPassword(user, user.Password);

        try
        {
            await _repository.Create(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        
        var token = _jwtTokenService.GenerateToken(user.Id, user.Password, "Standard");
        
        return Ok(new { Token = token });
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] User user)
    {
        try
        {
            var createdUser = await _repository.Create(user);

            return CreatedAtAction(nameof(Read), new { id = createdUser.Id }, createdUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Read(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.FindFirstValue(ClaimTypes.Role) != "Admin" && userId != id.ToString())
        {
            return Forbid();
        }
        
        var user = await _repository.Read(id);
        
        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(user);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _repository.GetAll();
        
        return Ok(users);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserLoginDTO userInfo)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.FindFirstValue(ClaimTypes.Role) != "Admin" && userId != id.ToString())
        {
            return Forbid();
        }
        
        try
        {
            var storedUser = await _repository.Read(id);
            
            if (storedUser == null)
            {
                return NotFound("User not found");
            }
            
            var updatedUser = _userFactory.Create(userInfo);
            updatedUser.Id = storedUser.Id;
            
            await _repository.Update(updatedUser);
            
            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.FindFirstValue(ClaimTypes.Role) != "Admin" && userId != id.ToString())
        {
            return Forbid();
        }
        
        try
        {
            var success = await _repository.Delete(id);

            if (success)
            {
                return NoContent();
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}