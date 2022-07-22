using AutoMapper;
using InventoryModels;
using InventoryModels.DTOs;

namespace EFCore_Activity0302
{
    public class InventoryMapper : Profile
    {
        public InventoryMapper()
        {
            CreateMaps();
        }

        private void CreateMaps()
        {
            CreateMap<Item, ItemDto>().ReverseMap();

            CreateMap<Item, ItemDetailDto>();

            CreateMap<Category, CategoryDto>()
                .ForMember(x => x.Category, opt => opt.MapFrom(y => y.Name))
                .ReverseMap()
                .ForMember(y => y.Name, opt => opt.MapFrom(x => x.Category));

            CreateMap<CategoryDetail, CategoryDetailDto>()
                .ForMember(x => x.Color, opt => opt.MapFrom(y => y.ColorName))
                .ForMember(x => x.Value, opt => opt.MapFrom(y => y.ColorValue))
                .ReverseMap()
                .ForMember(x => x.ColorValue, opt => opt.MapFrom(y => y.Value))
                .ForMember(y => y.ColorName, opt => opt.MapFrom(x => x.Color));

            CreateMap<Item, CreateOrUpdateItemDto>()
                .ReverseMap()
                .ForMember(x => x.Category, opt => opt.Ignore());
        }
    }
}
