using System.ComponentModel.DataAnnotations;

namespace Project.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public virtual Guid Id { get; set; } = Guid.NewGuid();
        public virtual DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public virtual DateTime? UpdateAt { get; set; }
    }
}
