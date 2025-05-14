using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Demo.DAL.Models;

[Table("ProjectTask")] 
public class ProjectTask
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id {get; set;}

    [Required] 
    [Column("Name")] 
    public string Name { get; set; }
    [Required]
    [Column("TaskText")]
    public string TaskText {get; set;}

    [Required]
    [Column("Status")]
    public ProjecTaskStatus Status {get; set;}

    [Required]
    [Column("Departament")] 
    public Department Department { get; set; } 
}