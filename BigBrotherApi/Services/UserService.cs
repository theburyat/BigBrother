using AutoMapper;
using BigBrother.Interfaces;
using Entities.Domain;
using Entities.Enums;
using Entities.Exceptions;
using Entities.Models;
using Repository.Interfaces;

namespace BigBrother.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Guid> CreateUserAsync(CreateUserModel userModel, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<User>(userModel);
        user.Id = Guid.NewGuid();

        return await _userRepository.CreateUserAsync(user, cancellationToken);
    }

    public async Task<Guid> GetUserIdByNameWithCreationIfNotExistAsync(CreateUserModel userModel, CancellationToken cancellationToken)
    {
        return UserWithNameExists(userModel.Name, userModel.Group)
            ? (await GetUserByNameAsync(userModel.Name, userModel.Group, cancellationToken)).Id
            : await CreateUserAsync(userModel, cancellationToken);
    }

    public async Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new BbException(ErrorCode.USER_NOT_FOUND, $"User with id {userId} not found");
        }

        return user;
    }

    public async Task<User> GetUserByNameAsync(string userName, string userGroup, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByNameAsync(userName, userGroup, cancellationToken);
        if (user is null)
        {
            throw new BbException(ErrorCode.USER_NOT_FOUND, $"User with name {userName} from group {userGroup} not found");
        }
        
        return user;
    }

    public IEnumerable<User> GetUsersFromGroup(string userGroup)
    {
        return _userRepository.GetUsersFromGroup(userGroup);
    }


    public bool UserWithNameExists(string userName, string userGroup)
    {
        return _userRepository.UserWithNameExists(userName, userGroup);
    }

    public async Task<Guid> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var deletedUserId = await _userRepository.DeleteUserAsync(userId, cancellationToken);
        if (deletedUserId == Guid.Empty)
        {
            throw new BbException(ErrorCode.USER_NOT_FOUND, $"User with id {userId} not found");
        }

        return deletedUserId;
    }
}