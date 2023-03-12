namespace BigBrother.Interfaces;

public interface ICopyAndModifyDetectionService
{
    public Task<IDictionary<Tuple<Guid, Guid>, double>> DetectCopyAndModify(string group, DateTime dateTime);
}
