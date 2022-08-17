#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // need this when doing NotMapped
namespace WeddingPlanner.Models;
public class UserWeddingParticipation
{
    [Key]
    public int UserWeddingParticipationId {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public int UserId {get;set;}
    public User? WeddingAttendee {get;set;}
    public int WeddingId {get;set;}
    public Wedding? WeddingEvent {get;set;}
}