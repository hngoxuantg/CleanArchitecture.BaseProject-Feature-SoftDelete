using System.ComponentModel.DataAnnotations;

namespace Project.Domain.Entities
{
    public class Product : BaseEntity
    {
        [Required, MaxLength(55)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        [Required, MaxLength]
        public decimal Price { get; set; }
        public Guid? CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
