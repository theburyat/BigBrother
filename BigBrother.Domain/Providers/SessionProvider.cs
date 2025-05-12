using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Entities.Exceptions;
using BigBrother.Domain.Interfaces.Providers;
using BigBrother.Domain.Interfaces.Repositories;

namespace BigBrother.Domain.Providers;

public sealed class SessionProvider: ISessionProvider
{
    private ISessionRepository _repository;
    private IGroupProvider _groupProvider;

    public SessionProvider(ISessionRepository repository, IGroupProvider groupProvider)
    {
        _repository = repository;
        _groupProvider = groupProvider;
    }

    public async Task<int> CreateSessionAsync(int groupId, CancellationToken cancellationToken)
    {
        await _groupProvider.EnsureGroupExistAsync(groupId, cancellationToken);
        
        return await _repository.CreateSessionAsync(groupId, cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetSessionsInGroupAsync(int groupId, CancellationToken cancellationToken)
    {
        await _groupProvider.EnsureGroupExistAsync(groupId, cancellationToken);
        
        return await _repository.GetSessionsInGroupAsync(groupId, cancellationToken);
    }

    public async Task<Session> GetSessionAsync(int id, CancellationToken cancellationToken)
    {
        return await _repository.GetSessionAsync(id, cancellationToken)
            ?? throw new BadRequestException(ErrorCode.SessionNotFound, $"Session with id '{id}' was not found");
    }

    public async Task DeleteSessionAsync(int id, CancellationToken cancellationToken)
    {
        await EnsureSessionExistAsync(id, cancellationToken);
        
        await _repository.DeleteSessionAsync(id, cancellationToken);
    }

    public async Task StartSessionAsync(int id, CancellationToken cancellationToken)
    {
        await EnsureSessionExistAsync(id, cancellationToken);
        
        await _repository.StartSessionAsync(id, cancellationToken);
    }

    public async Task StopSessionAsync(int id, CancellationToken cancellationToken)
    {
        await EnsureSessionExistAsync(id, cancellationToken);
        
        var session = await _repository.GetSessionAsync(id, cancellationToken);
        if (session!.StartDate == null) {
            throw new BadRequestException(ErrorCode.SessionWasNotStarted, $"Session with id '{id}' was not started");
        }

        await _repository.StopSessionAsync(id, cancellationToken);
    }

    public async Task EnsureSessionExistAsync(int id, CancellationToken cancellationToken)
    {
        if (!await _repository.IsSessionExistAsync(id, cancellationToken))
        {
            throw new BadRequestException(ErrorCode.SessionNotFound, $"Session with id '{id}' was not found");
        }
    }
}
