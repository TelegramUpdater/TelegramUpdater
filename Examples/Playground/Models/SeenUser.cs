using System.ComponentModel.DataAnnotations;

namespace Playground.Models;

internal class SeenUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required long TelegramId { get; set; }

    [Required]
    public required string Name { get; set; }
}
