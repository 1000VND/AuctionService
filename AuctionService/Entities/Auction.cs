namespace AuctionService.Entities
{
    public class Auction
    {
        public Guid Id { get; set; }
        /// <summary>Giá đặt trước</summary>
        public int ReservePrice { get; set; } = 0;
        public string? Seller { get; set; }
        public string? Winner { get; set; }
        public int? SoldAmount { get; set; }
        public int? CurrentHightBid { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime AuctionEnd { get; set; }
        public Status Status { get; set; }
        public Item Item { get; set; }
    }
}
