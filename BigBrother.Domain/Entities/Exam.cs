namespace BigBrother.Domain.Entities;

public class Exam
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public DateTime Date { get; set; }

    public int CopyCount { get; set; }
    
    public int CutCount { get; set; }
    
    public int DeleteCount { get; set; }
    
    public int EnterCount { get; set; }
    
    public int SelectCount { get; set; }

    public int PasteCount { get; set; }
    
    public int PasteFromUnknownSourceCount { get; set; }

    public int TypeCount { get; set; }
    
    public int MoveCaretDownCount { get; set; }

    public int MoveCaretLeftCount { get; set; }

    public int MoveCaretRightCount { get; set; }

    public int MoveCaretUpCount { get; set; }

    public int CodeCompletionCount { get; set; }
    
    public int RunningCount { get; set; }
    
    public int BuildingCount { get; set; }

    public string ActionsLog { get; set; }
}