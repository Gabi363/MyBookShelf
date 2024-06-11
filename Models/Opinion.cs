using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


[PrimaryKey(nameof(BookId), nameof(UserId))]
public class Opinion
{
    [ForeignKey("Book")]
    public int BookId { get; set; }
    public Book Book { get; set; } 
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } 
    public int Rank { get; set; }
    public string Review { get; set; } = "";
}