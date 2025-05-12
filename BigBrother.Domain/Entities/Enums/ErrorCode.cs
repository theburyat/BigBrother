namespace BigBrother.Domain.Entities.Enums;

public enum ErrorCode
{
    GroupNotFound,
    GroupAlreadyExists,
    GroupsIsNotEmpty,
    
    SessionNotFound,
    SessionWasNotStarted,
    SessionWasNotFinished,
    SessionIsNotActive,
    
    UserNotFound,
    UserAlreadyExists,
    NotEnoughUsersForAnalysis,
    
    ScoreNotFound,
    InvalidScore
}