namespace PharmaInsightsServices.DTOs;

public class UserDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public int PharmacyId { get; set; }
}
