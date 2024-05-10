using System.ComponentModel.DataAnnotations;

namespace Kolos.Models;

public class AutorKsiazki
{
    [Required]
    public int id { get; set; }
    [Required]
    [MaxLength(100)]
    public string title { get; set; }
    [Required]
    public ICollection<Autor> autorzy { get; set; }
}