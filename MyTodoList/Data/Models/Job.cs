using System.ComponentModel.DataAnnotations;

namespace MyTodoList.Data.Models;

public class Job
{
    public int Id { get; init; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public int CategoryId { get; set; }
    public bool IsDone { get; set; }

    public Category Category { get; set; } = null!;
}
