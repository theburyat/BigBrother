namespace BigBrother.Interfaces;

public interface ICopyAndModifyDetectionService
{
    public IDictionary<Tuple<Guid, Guid>, double> DetectCopyAndModify(string group, DateTime dateTime);
}