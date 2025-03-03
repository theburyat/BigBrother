using System.Collections.Concurrent;
using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Extensions;
using BigBrother.Domain.Interfaces.Services;

namespace BigBrother.Domain.Services;

public class DetectionService : IDetectionService
{
    private const double NormalizationValue = 0.16;

    public async Task<IDictionary<int, double>> DetectAnomaliesAsync(IReadOnlyCollection<UserActions> usersActions, CancellationToken cancellationToken)
    {
        if (usersActions.Count() < 2) 
        {
            throw new Exception("few actions");
        }
        
        var result = new ConcurrentDictionary<int, double>();

        var actionsDistributions = usersActions.Select(x => x.Actions).ToArray();
        var boxCoxDistributions = actionsDistributions.Select(x => x.ToBoxCoxDistribution()).ToArray();
        
        var maxActionsCount = actionsDistributions.SelectMany(x => x.Values).Max();

        var boxCoxMeans = GetDistributionsMeans(boxCoxDistributions);
        var boxCoxStandardDeviations = GetDistributionsStandardDeviations(boxCoxDistributions, boxCoxMeans);
        var weights = GetActionsWeights(actionsDistributions);

        var tasks = new List<Task>();

        foreach (var userActions in usersActions)
        {
            tasks.Add(Task.Run(() =>
            {
                var outlierScore = GetDistributionOutlierScore(
                    userActions.Actions.ToBoxCoxDistribution(),
                    boxCoxMeans,
                    boxCoxStandardDeviations,
                    weights,
                    maxActionsCount);

                result.TryAdd(userActions.UserId, outlierScore);
            }, cancellationToken));
        }

        await Task.WhenAll(tasks);

        return result;
    }

    private IReadOnlyDictionary<ActionType, double> GetDistributionsMeans(
        IReadOnlyCollection<IReadOnlyDictionary<ActionType, double>> distributions)
    {
        var result = new Dictionary<ActionType, double>();

        foreach (var distribution in distributions)
        {
            foreach (var action in distribution.Keys)
            {
                result.TryAdd(action, 0);
                result[action] += distribution[action] / distributions.Count;
            }
        }

        return result;
    }

    private IReadOnlyDictionary<ActionType, double> GetDistributionsStandardDeviations(
        IReadOnlyCollection<IReadOnlyDictionary<ActionType, double>> distributions,
        IReadOnlyDictionary<ActionType, double> distributionsMeans)
    {
        var result = new Dictionary<ActionType, double>();

        foreach (var distribution in distributions)
        {
            foreach (var action in distribution.Keys)
            {
                result.TryAdd(action, 0);
                result[action] = Math.Pow(distribution[action] - distributionsMeans[action], 2);
            }
        }
        return result.ToDictionary(x => x.Key, x => Math.Sqrt(x.Value / distributions.Count));
    }

    private IReadOnlyDictionary<ActionType, double> GetActionsWeights(
        IReadOnlyCollection<IReadOnlyDictionary<ActionType, int>> distributions)
    {
        var result = new Dictionary<ActionType, double>();

        foreach (var distribution in distributions)
        {
            foreach (var action in distribution.Keys)
            {
                result.TryAdd(action, 0);
                result[action] += distribution[action] > 0 ? 1 : 0;
            }
        }

        return result.ToDictionary(x => x.Key, x => Math.Pow(x.Value / distributions.Count, 2));
    }

    private double GetDistributionOutlierScore(
        IReadOnlyDictionary<ActionType, double> distribution,
        IReadOnlyDictionary<ActionType, double> distributionsMeans,
        IReadOnlyDictionary<ActionType, double> distributionsStandardDeviations,
        IReadOnlyDictionary<ActionType, double> weights,
        int maxActionsCount)
    {
        double numerator = 0;
        double denominator = 0;

        var actionScores = GetActionsOutlierScores(distribution, distributionsMeans, 
            distributionsStandardDeviations, maxActionsCount);

        foreach (var action in distribution.Keys)
        {
            numerator += weights[action] * actionScores[action];
            denominator += weights[action];
        }

        return numerator / denominator;
    }

    private IReadOnlyDictionary<ActionType, double> GetActionsOutlierScores(
        IReadOnlyDictionary<ActionType, double> distribution,
        IReadOnlyDictionary<ActionType, double> distributionsMeans,
        IReadOnlyDictionary<ActionType, double> distributionsStandardDeviations,
        int maxActionsCount)
    {
        var result = new Dictionary<ActionType, double>();

        foreach (var action in distribution.Keys)
        {
            var difference = distribution[action] - distributionsMeans[action];
            var standardDeviation = distributionsStandardDeviations[action];
            var outlierScore = -standardDeviation <= difference && difference <= standardDeviation
                ? 0
                : (1 - GetPraw(action, distribution[action], distributionsMeans, maxActionsCount)) / NormalizationValue;

            result.Add(action, outlierScore);
        }

        return result;
    }

    private double GetPraw(ActionType action, double distributionValue, IReadOnlyDictionary<ActionType, double> distributionsMeans, 
        int maxActionsCount)
    {
        return distributionValue > distributionsMeans[action]
            ? 1 - distributionValue / maxActionsCount
            : distributionValue / maxActionsCount;
    }
}
