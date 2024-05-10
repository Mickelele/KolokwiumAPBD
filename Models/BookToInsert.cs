using System.ComponentModel.DataAnnotations;

namespace Kolos.Models;

public class BookToInsert
{
    [Required]
    [MaxLength(100)]
    public string title { get; set; }
    [Required]
    public ICollection<Autor> autorzy { get; set; }
}