using System.ComponentModel.DataAnnotations;

namespace Kolos.Models;

public class Autor
{
    [Required]
    [MinLength(1)]
    [MaxLength(50)]
    public string firstName { get; set; }
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string lastName { get; set; }
}