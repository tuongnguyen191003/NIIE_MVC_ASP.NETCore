using System.ComponentModel.DataAnnotations;

namespace MVC_FinalTerm.Models
{
    public class ColorModel
    {
        [Key]
        public int ColorId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
