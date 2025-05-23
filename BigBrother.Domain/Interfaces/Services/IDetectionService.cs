using BigBrother.Domain.Entities;

namespace BigBrother.Domain.Interfaces.Services;

public interface IDetectionService 
{
    Task<IDictionary<int, double>> DetectAnomaliesAsync(IReadOnlyCollection<UserIdeActionsDistribution> usersActions, CancellationToken cancellationToken);
}
