using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;

namespace AuctionService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Ánh xạ từ Auction sang AuctionDto, bao gồm cả các trường từ Item
            CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
            // Ánh xạ từ Item sang AuctionDto (dùng để lấy các trường từ Item khi ánh xạ Auction -> AuctionDto)
            CreateMap<Item, AuctionDto>();
            // Ánh xạ từ CreateAuctionDto sang Auction, đồng thời ánh xạ toàn bộ CreateAuctionDto sang property Item của Auction
            CreateMap<CreateAuctionDto, Auction>()
                .ForMember(d => d.Item, o => o.MapFrom(s => s));
            // Ánh xạ từ CreateAuctionDto sang Item
            CreateMap<CreateAuctionDto, Item>();
        }
    }
}
