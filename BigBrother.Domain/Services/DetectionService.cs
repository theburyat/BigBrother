using System.Collections.Concurrent;
using BigBrother.Domain.Entities;
using BigBrother.Domain.Entities.Enums;
using BigBrother.Domain.Extensions;
using BigBrother.Domain.Interfaces.Services;

namespace BigBrother.Domain.Services;

public class DetectionService : IDetectionService
{
    private const double NormalizationValue = 0.16;

    public async Task<IDictionary<int, double>> DetectAnomaliesAsync(IReadOnlyCollection<UserIdeActionsDistribution> usersActions, CancellationToken cancellationToken)
    {
        if (usersActions.Count() < 2) 
        {
            throw new Exception("few actions");
        }
        
        var result = new ConcurrentDictionary<int, double>();

        var actionsDistributions = usersActions.Select(x => x.IdeActionsDistribution).ToArray();
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
                    userActions.IdeActionsDistribution.ToBoxCoxDistribution(),
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

    private IReadOnlyDictionary<IdeActionType, double> GetDistributionsMeans(
        IReadOnlyCollection<IReadOnlyDictionary<IdeActionType, double>> distributions)
    {
        var result = new Dictionary<IdeActionType, double>();

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

    private IReadOnlyDictionary<IdeActionType, double> GetDistributionsStandardDeviations(
        IReadOnlyCollection<IReadOnlyDictionary<IdeActionType, double>> distributions,
        IReadOnlyDictionary<IdeActionType, double> distributionsMeans)
    {
        var result = new Dictionary<IdeActionType, double>();

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

    private IReadOnlyDictionary<IdeActionType, double> GetActionsWeights(
        IReadOnlyCollection<IReadOnlyDictionary<IdeActionType, int>> distributions)
    {
        var result = new Dictionary<IdeActionType, double>();

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
        IReadOnlyDictionary<IdeActionType, double> distribution,
        IReadOnlyDictionary<IdeActionType, double> distributionsMeans,
        IReadOnlyDictionary<IdeActionType, double> distributionsStandardDeviations,
        IReadOnlyDictionary<IdeActionType, double> weights,
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

    private IReadOnlyDictionary<IdeActionType, double> GetActionsOutlierScores(
        IReadOnlyDictionary<IdeActionType, double> distribution,
        IReadOnlyDictionary<IdeActionType, double> distributionsMeans,
        IReadOnlyDictionary<IdeActionType, double> distributionsStandardDeviations,
        int maxActionsCount)
    {
        var result = new Dictionary<IdeActionType, double>();

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

    private double GetPraw(IdeActionType action, double distributionValue, IReadOnlyDictionary<IdeActionType, double> distributionsMeans, 
        int maxActionsCount)
    {
        return distributionValue > distributionsMeans[action]
            ? 1 - distributionValue / maxActionsCount
            : distributionValue / maxActionsCount;
    }
}
