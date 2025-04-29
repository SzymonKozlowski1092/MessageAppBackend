using AutoMapper;
using MessageAppBackend.DbModels;
using MessageAppBackend.DTO.ChatDTOs;
using MessageAppBackend.DTO.ChatInvitationDTOs;
using MessageAppBackend.DTO.MessageDTOs;
using MessageAppBackend.DTO.UserDTOs;

namespace MessageAppBackend.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            //ENTITY TO DTO MAPPINGS
            CreateMap<User, UserDto>();

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderDisplayName, 
                opt => opt.MapFrom(src => src.Sender != null ? src.Sender.DisplayName : string.Empty));

            CreateMap<Chat, ChatDto>()
                .ForMember(dest => dest.Users, opt => opt
                .MapFrom(src => src.Users != null ? src.Users.Select(uc => uc.User).Where(u => u != null) : new List<User>()))
                .ForMember(dest => dest.Messages, opt => opt
                .MapFrom(src => src.Messages != null ? src.Messages.Where(m => m != null) : new List<Message>()));

            CreateMap<Chat, SimpleChatDto>()
                .ForMember(dest => dest.LastMessageContent, opt => opt.MapFrom(src =>
                src.Messages != null && src.Messages.Any()
                    ? src.Messages.OrderByDescending(m => m.SentAt).First().Content : string.Empty))
                .ForMember(dest => dest.LastMessageSenderDisplayName, opt => opt.MapFrom(src =>
                src.Messages != null && src.Messages.Any() && src.Messages.OrderByDescending(m => m.SentAt).First().Sender != null
                    ? src.Messages.OrderByDescending(m => m.SentAt).First().Sender!.DisplayName : string.Empty));

            CreateMap<ChatInvitation, ChatInvitationDto>()
                .ForMember(dest => dest.ChatName, opt => opt.MapFrom(src => src.Chat.Name))
                .ForMember(dest => dest.InvitedUsername, opt => opt.MapFrom(src => src.InvitedUser.Username))
                .ForMember(dest => dest.InvitedByUsername, opt => opt.MapFrom(src => src.InvitedByUser.Username))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            //DTO TO ENTITY MAPPINGS
            CreateMap<MessageDto, Message>();

            CreateMap<NewMessageDto, Message>().ForMember(
                    dest => dest.SentAt,
                    opt => opt.MapFrom(d => DateTime.UtcNow));
        }
    }
}
