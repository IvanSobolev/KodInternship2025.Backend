using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Demo.DAL.Models;

[Table("Employee")] 
public class Employee
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public long TelegramId { get; set; }

    [Required] 
    [MaxLength(50)]  
    [Column("FullName")] 
    public string FullNames{ get; set; }


    [Required]
    [Column("Departament")] 
    public Department Department { get; set; } 
    }




