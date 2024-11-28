namespace PharmaInsightsServices.DTOs
{
    public class ProductInventorySummaryDto
    {
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalUnitsEntered { get; set; }
        public int TotalUnitsSold { get; set; }
        public int RemainingUnits { get; set; }
    }
}
