using System.ComponentModel;

namespace Domain.Entities.GeneralModule
{
    public class BaseEntity
    {
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int? ModifiedBy { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
    }
}
