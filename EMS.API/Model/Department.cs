using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.API.Model
{
    public class Department
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }
        [Required,MaxLength(50)]
        public string DepartmentName { get; set; }=string.Empty;
        public bool IsActive { get; set; }
    }
}
