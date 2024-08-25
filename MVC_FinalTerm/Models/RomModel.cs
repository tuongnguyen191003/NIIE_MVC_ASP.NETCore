using System.ComponentModel.DataAnnotations;

namespace MVC_FinalTerm.Models
{
    public class RomModel
    {
        [Key]
        public int RomId { get; set; }
        public string Value { get; set; }
    }
}
