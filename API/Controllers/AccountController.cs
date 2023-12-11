
using Microsoft.AspNetCore.Identity;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using API.Errors;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using API.Extensions;
using AutoMapper;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, 
        SignInManager<AppUser> signInManager, 
        ITokenService tokenService,
        IMapper mapper)
    {
        _tokenService = tokenService;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser(){
        var user = await _userManager.FindByClaimsPrincipleAsync(User);
        return new UserDto(){
            Email = user.Email,
            Token = _tokenService.CreateToken(user),
            DisplayName = user.DisplayName
        };
    }

    [HttpGet("emailexists")]
    public async Task<ActionResult<AppUser>> CheckEmailExistAsync([FromQuery] string email){
        return await _userManager.FindByEmailAsync(email);
    }

    [HttpGet("displayeameexists")]
    public async Task<ActionResult<AppUser>> DisplayNameExistsAsync([FromQuery] string name){
        return await _userManager.FindByNameAsync(name);
    }

    [Authorize]
    [HttpPut("address")]
    public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address){
        var user = await _userManager.FindUserByClaimsPrincipleWithAddressAsync(User);
        user.Address = _mapper.Map<AddressDto, Address>(address);
        var result = await _userManager.UpdateAsync(user);
        if(result.Succeeded) return Ok(_mapper.Map<Address, AddressDto>(user.Address));
        return BadRequest("Problem updating the user");
    }
    
    [Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<AddressDto>> GetUserAdress(){
        var user = await _userManager.FindUserByClaimsPrincipleWithAddressAsync(User);
        return _mapper.Map<Address, AddressDto>(user.Address);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if(user == null)
            return Unauthorized(new ApiResponse(401));
        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if(!result.Succeeded) return Unauthorized(new ApiResponse(401));

        return new UserDto(){
            Email = user.Email,
            Token = _tokenService.CreateToken(user),
            DisplayName = user.DisplayName
        };
    }
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){
        if(await CheckEmailExistAsync(registerDto.Email) is not null){
            return new BadRequestObjectResult(
                new ValidationErrorRespose{Errors = new [] {"Email address is in use"} });
        }
        if(await DisplayNameExistsAsync(registerDto.DisplayName) is not null)
            return new BadRequestObjectResult(
                new ValidationErrorRespose{Errors = new [] {"Display name exists"} });
                
        var user = new AppUser() {
            DisplayName = registerDto.DisplayName,
            UserName = registerDto.DisplayName,
            Email = registerDto.Email
        };
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if(existingUser is not null) return BadRequest(new ApiResponse(400, "Email already exist")); 
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if(!result.Succeeded) return BadRequest(new ApiResponse(400));
        
        return new UserDto(){
            Email = user.Email,
            Token =  _tokenService.CreateToken(user),
            DisplayName = user.DisplayName
        };
    }
}