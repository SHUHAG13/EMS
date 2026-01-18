using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.API.Model
{
    public class Designation
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        [Required,MaxLength(100)]
        public string DesignationName { get; set; }=string.Empty;
    }
}
