namespace PharmaInsightsServices.DTOs;

public class InventoryDto
{
    public int PharmacyId { get; set; }
    public int ProductId { get; set; }
    public int QuantityEntered { get; set; }
    public int QuantitySold { get; set; }
    public DateTime EntryDate { get; set; }
    public DateTime SaleDate { get; set; }
}
