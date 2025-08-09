using System.ComponentModel.DataAnnotations;

namespace Project.Domain.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        public virtual DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public virtual Guid? CreatedBy { get; set; }


        public virtual DateTime? UpdateAt { get; private set; }
        public virtual Guid? UpdatedBy { get; private set; }


        public virtual DateTime? DeleteAt { get; private set; }
        public virtual Guid? DeletedBy { get; private set; }
        public virtual bool IsDeleted => DeleteAt.HasValue;

        [Timestamp]
        public virtual byte[]? Version { get; set; }


        public virtual void MarkAsUpdated(Guid updateBy)
        {
            UpdateAt = DateTime.UtcNow;
            UpdatedBy = updateBy;
        }
        public virtual void MarkAsDeleted(Guid deleteBy)
        {
            DeleteAt = DateTime.UtcNow;
            DeletedBy = deleteBy;
        }
        public virtual void RestoreFromDeleted()
        {
            DeleteAt = null;
            DeletedBy = null;
        }
        public virtual void ResetUpdateInfo()
        {
            UpdateAt = null;
            UpdatedBy = null;
        }
    }
}
