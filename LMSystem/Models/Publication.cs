using System.ComponentModel.DataAnnotations;

namespace LMSystem.Models
{
    public enum PublicationType { Newspaper, Magazine }

    public class Publication
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        [StringLength(50)]
        public string? Publisher { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Published Date")]
        public DateTime PublishedDate { get; set; }

        [Required]
        public PublicationType Type { get; set; } // Differentiates Newspaper vs Magazine

        public bool IsAvailable { get; set; } = true;
    }
}
