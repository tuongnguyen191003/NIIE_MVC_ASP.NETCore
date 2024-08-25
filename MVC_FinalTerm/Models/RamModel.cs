using System.ComponentModel.DataAnnotations;

namespace MVC_FinalTerm.Models
{
    public class RamModel
    {
        [Key]
        public int RamId { get; set; }
        public string Value { get; set; }
    }
}
