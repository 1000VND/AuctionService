using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams input)
        {
            var query = DB.PagedSearch<Item, Item>();

            if (!string.IsNullOrEmpty(input.SearchTerm))
            {
                query.Match(Search.Full, input.SearchTerm).SortByTextScore();
            }

            query = input.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(a => a.Make)),
                "new" => query.Sort(x => x.Descending(a => a.CreateAt)),
                _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
            };

            query = input.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(7)),
                "endingSoon" => query.Match(x => x.AuctionEnd > DateTime.UtcNow.AddHours(7) && x.AuctionEnd < DateTime.UtcNow.AddHours(7 + 6)),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow.AddHours(7))
            };

            if (!string.IsNullOrEmpty(input.Seller))
            {
                query.Match(x => x.Seller == input.Seller);
            }

            if (!string.IsNullOrEmpty(input.Winner))
            {
                query.Match(x => x.Winner == input.Winner);
            }

            query.PageNumber(input.PageNumber);
            query.PageSize(input.PageSize);

            var result = await query.ExecuteAsync();
            return Ok(new
            {
                result = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}
