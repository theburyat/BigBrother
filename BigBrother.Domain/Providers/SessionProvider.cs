using BigBrother.Domain.Entities;
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

    public async Task<IEnumerable<Session>> GetGroupSessionsAsync(int groupId, CancellationToken cancellationToken)
    {
        await ValidateGroupExistingAsync(groupId, cancellationToken);
        return await _repository.GetGroupSessionsAsync(groupId, cancellationToken);
    }

    public async Task<Session> GetSessionAsync(int id, CancellationToken cancellationToken)
    {
        var session = await _repository.GetSessionAsync(id, cancellationToken);
        
        return session ?? throw new Exception();
    }

    public async Task<int> CreateSessionAsync(int groupId, CancellationToken cancellationToken)
    {
        await ValidateGroupExistingAsync(groupId, cancellationToken);
        return await _repository.CreateSessionAsync(groupId, cancellationToken);
    }

    public async Task DeleteSessionAsync(int id, CancellationToken cancellationToken)
    {
        await ValidateSessionExistingAsync(id, cancellationToken);
        await _repository.DeleteSessionAsync(id, cancellationToken);
    }

    public async Task StartSessionAsync(int id, CancellationToken cancellationToken)
    {
        await ValidateSessionExistingAsync(id, cancellationToken);
        await _repository.StartSessionAsync(id, cancellationToken);
    }

    public async Task StopSessionAsync(int id, CancellationToken cancellationToken)
    {
        await ValidateSessionExistingAsync(id, cancellationToken);
        
        var session = await _repository.GetSessionAsync(id, cancellationToken);
        if (session!.StartDate == null) {
            throw new Exception();
        }

        await _repository.StopSessionAsync(id, cancellationToken);
    }

    public Task<bool> IsSessionExistAsync(int id, CancellationToken cancellationToken) 
    {
        return _repository.IsSessionExistAsync(id, cancellationToken);
    }

    private async Task ValidateSessionExistingAsync(int id, CancellationToken cancellationToken)
    {
        if (!await IsSessionExistAsync(id, cancellationToken))
        {
            throw new Exception();
        }
    }

    private async Task ValidateGroupExistingAsync(int groupId, CancellationToken cancellationToken) 
    {
        if (!await _groupProvider.IsGroupExistAsync(groupId, cancellationToken))
        {
            throw new Exception();
        }
    }
}
