using AutoMapper;
using P360WebReference;

namespace CreditOne.P360FormSubmissionService.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<P360WebReference.Property, P360FormService.Models.Requests.FormData>()
                .ForMember(dest => dest.FieldValue, opts => opts.MapFrom(src => src.Value))
                .ForMember(dest => dest.FieldName, opts => opts.MapFrom(src => src.Name));
            CreateMap<P360FormService.Models.Requests.FormData, Property>()
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.FieldName))
                .ForMember(dest => dest.Value, opts => opts.MapFrom(src => src.FieldValue));
            CreateMap<P360WebReference.Property, P360FormService.Models.Responses.Property>().ReverseMap();
            CreateMap<P360WebReference.Attributes, P360FormService.Models.Responses.Attributes>();
            CreateMap<P360WebReference.Attribute, P360FormService.Models.Responses.Attribute>();
            CreateMap<P360WebReference.PropertySet, P360FormService.Models.Responses.PropertySet>();
            CreateMap<P360WebReference.Folder, P360FormService.Models.Responses.Folder>();
            CreateMap<P360WebReference.FolderItem, P360FormService.Models.Responses.FolderItem>();
            CreateMap<P360WebReference.Document, P360FormService.Models.Responses.Document>()
                .Include<DesktopDocument, P360FormService.Models.Responses.DesktopDocument>()
                .Include<ImageDocument, P360FormService.Models.Responses.ImageDocument>();
                
            CreateMap<P360WebReference.DesktopDocument, P360FormService.Models.Responses.DesktopDocument>();
            CreateMap<P360WebReference.ImageDocument, P360FormService.Models.Responses.ImageDocument>()
                .ForMember(dest => dest.DesktopFileData, opts => opts.MapFrom(src => src.Pages[0].ImageData))
                .ForMember(dest => dest.DocumentExtension, opts => opts.MapFrom(src => src.Pages[0].DocumentExtension));

            CreateMap<P360WebReference.ImagePage, P360FormService.Models.Responses.ImagePage>();
            CreateMap<P360WebReference.DocumentType, P360FormService.Models.Responses.DocumentType>();
            CreateMap<P360WebReference.DocumentSubtype, P360FormService.Models.Responses.DocumentSubtype>();
            CreateMap<P360WebReference.Workpacket, P360FormService.Models.Responses.Workpacket>();
            CreateMap<P360WebReference.CatalogItem, P360FormService.Models.Responses.Workpacket>()
                .ForMember(dest => dest.WorklistAttributes, opts => opts.MapFrom(src => src.Attributes));
        }
    }
}
