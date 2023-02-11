namespace Entities.Domain;

public class Exam
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public DateTime Date { get; set; }

    public int BackspaceCount { get; set; }
    
    public int CopyCount { get; set; }
    
    public int CutCount { get; set; }
    
    public int DeleteCount { get; set; }
    
    public int EnterCount { get; set; }
    
    public int LineEndWithSelectionCount { get; set; }
    
    public int LineStartWithSelectionCount { get; set; }
    
    public int PasteCount { get; set; }
    
    public int SelectAllCount { get; set; }
    
    public int SelectWordCount { get; set; }
    
    public int TypeCount { get; set; }
    
    public int MoveCaretDownCount { get; set; }
    
    public int MoveCaretDownWithSelectionCount { get; set; }
    
    public int MoveCaretLeftCount { get; set; }
    
    public int MoveCaretLeftWithSelectionCount { get; set; }
    
    public int MoveCaretRightCount { get; set; }
    
    public int MoveCaretRightWithSelectionCount { get; set; }
    
    public int MoveCaretUpCount { get; set; }
    
    public int MoveCaretUpWithSelectionCount { get; set; }
    
    public int PageUpWithSelectionCount { get; set; }
    
    public int PageDownWithSelectionCount { get; set; }
    
    public int CodeCompletionCount { get; set; }
    
    public int RunningCount { get; set; }
    
    public int BuildingCount { get; set; }
}