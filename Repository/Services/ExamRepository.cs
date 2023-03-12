using AutoMapper;
using Entities.Database;
using Entities.Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Services;

public class ExamRepository: IExamRepository
{
    private readonly IMapper _mapper;

    public ExamRepository(IMapper mapper)
    {
        _mapper = mapper;
    }
    
    public async Task<Guid> CreateExamAsync(Exam exam, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            await context.ExamEntities.AddAsync(_mapper.Map<ExamEntity>(exam), cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        return exam.Id;
    }

    public async Task<Exam> GetExamAsync(Guid examId, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            return _mapper.Map<Exam>(await context.ExamEntities.FirstOrDefaultAsync(x => x.Id == examId, cancellationToken));
        }
    }

    public IReadOnlyCollection<Exam> GetUserExams(Guid userId)
    {
        using (var context = new AppDbContext())
        {
            return _mapper.Map<IReadOnlyCollection<Exam>>(context.ExamEntities.Where(x => x.UserId == userId));
        }
    }

    public async Task<Guid> DeleteExamAsync(Guid examId, CancellationToken cancellationToken)
    {
        using (var context = new AppDbContext())
        {
            var examEntity = await context.ExamEntities.FirstOrDefaultAsync(x => x.Id == examId, cancellationToken);
            context.ExamEntities.Remove(examEntity!);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        return examId;
    }
}
