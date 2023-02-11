using AutoMapper;
using BigBrother.Interfaces;
using Repository.Interfaces;

namespace BigBrother.Services;

public class ExamService: IExamService
{
    private IRepository _repository;
    private IMapper _mapper;
    
    public ExamService(IRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
}