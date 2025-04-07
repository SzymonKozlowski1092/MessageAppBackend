using MessageAppBackend.DTO;
using AutoMapper;
using MessageAppBackend.DbModels;

namespace MessageAppBackend
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewMessageDto, Message>().ForMember(
                    dest => dest.SentAt,
                    opt => opt.MapFrom(d => DateTime.UtcNow));
        }
    }
}
