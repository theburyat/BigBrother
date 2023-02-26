using AutoMapper;
using BigBrother.Interfaces;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace BigBrother.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController: ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<UserModel> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserAsync(id, cancellationToken);
        return _mapper.Map<UserModel>(user);
    }
    
    [HttpGet("byName")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<UserModel> GetUserByNameAsync(
        [FromQuery] string userName, 
        [FromQuery] string userGroup,
        CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByNameAsync(userName, userGroup, cancellationToken);
        return _mapper.Map<UserModel>(user);
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BbException))]
    public async Task<Guid> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = await _userService.DeleteUserAsync(id, cancellationToken);
        return userId;
    }
}