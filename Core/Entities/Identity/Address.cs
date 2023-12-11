using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Identity;
public class Address
{
    public int Id {get;set;}

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    
    [Required]
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}

