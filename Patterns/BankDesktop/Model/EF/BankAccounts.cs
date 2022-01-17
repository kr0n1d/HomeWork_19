using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankDesktop.Model.EF
{
    [Table("BankAccounts")]
    public class BankAccounts
    {
        [Key]
        public string number { get; set; }
        public System.DateTime dateOpen { get; set; }
        public decimal balance { get; set; }
        public bool capitalization { get; set; }
        public int numberTimesIncreased { get; set; }
        public int clientId { get; set; }
    }
}