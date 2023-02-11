using AutoMapper;
using Entities.Database;
using Entities.Domain;
using Entities.Models;

namespace BigBrother.Helpers.MappingProfiles;

public class ExamMappingProfile: Profile
{
    public ExamMappingProfile()
    {
        CreateMap<CreateExamModel, Exam>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => Guid.NewGuid()));

        CreateMap<ExamModel, Exam>().ReverseMap();

        CreateMap<Exam, ExamEntity>().ReverseMap();
    }
}