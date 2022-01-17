using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankDesktop.Model.EF
{
    [Table("BankCredits")]
    public class BankCredits
    {
        [Key]
        public string number { get; set; }
        public System.DateTime dateOpen { get; set; }
        public int creditTerm { get; set; }
        public decimal sumCredit { get; set; }
        public int clientId { get; set; }
        public string numberBankAccount { get; set; }
        public decimal paidOut { get; set; }
    }
}