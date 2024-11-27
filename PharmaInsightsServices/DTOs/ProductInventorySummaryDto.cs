namespace PharmaInsightsServices.DTOs
{
    public class ProductInventorySummaryDto
    {
        public int PharmacyId { get; set; }
        public required string PharmacyName { get; set; }
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public decimal Price { get; set; }
        public int TotalUnits { get; set; }
    }
}
