#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // need this when doing NotMapped
namespace WeddingPlanner.Models;
public class Wedding
{
    [Key]
    public int WeddingId {get;set;}

    [Required(ErrorMessage = "Name of one wedder is required")]
    [Display(Name = "Wedder One")]
    public string WedderOne {get;set;}

    [Required(ErrorMessage = "Name of one wedder is required")]
    [Display(Name = "Wedder Two")]
    public string WedderTwo {get;set;}

    [Required(ErrorMessage = "Wedding date is required")]
    [Display(Name = "Wedding Date")]
    [DataType(DataType.Date)]
    [FutureDate]
    public DateTime WeddingDate {get;set;}

    [Required(ErrorMessage = "Wedding address is required")]
    [Display(Name = "Wedding Address")]
    public string WeddingAddress {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;

    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public int UserId {get;set;}
    public User? WeddingCreator {get;set;}

    public List<UserWeddingParticipation> WeddingParticipants {get;set;} = new List<UserWeddingParticipation>(); // added for many to many
}


public class FutureDateAttribute : ValidationAttribute
{
    // Need the question marks to account for any possible null values
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // the following if statement will help pass the validator so that it continues to the next step because you cannot check something that is empty
        if(value == null)
        {
            return ValidationResult.Success;
        }

        DateTime date = (DateTime)value;

        if(date <= DateTime.Now)
        {
            return new ValidationResult("must be in the future");
        }

        return ValidationResult.Success;
    }
}