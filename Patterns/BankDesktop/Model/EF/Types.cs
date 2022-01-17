using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankDesktop.Model.EF
{
    [Table("Types")]
    public class Types
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
    }
}