using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankDesktop.Model.EF
{
    [Table("Clients")]
    public class Clients
    {
        [Key]
        public int id { get; set; }
        public string fullName { get; set; }
        public int typeId { get; set; }
        public bool privileged { get; set; }
    }
}