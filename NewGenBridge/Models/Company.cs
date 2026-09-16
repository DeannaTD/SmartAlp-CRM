using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewGenBridge.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; } //имя компании (имя филиала?)
        public string ExecutiveName { get; set; } //имя руководителя
        public string PhoneNumber { get; set; } //номер телефона для связи с компанией (не руководителя)
        public string Email { get; set; } 
        public string TaxId { get; set; } //Codice fiscale
        public string BackAccount { get; set; } //номер банковского счета (типа IBAN)
        public string BankName { get; set; } //название банка
        public string BankId { get; set; } //код банка (BIC/SWIFT)
        public string? Address { get; set; } //адрес компании

        [ValidateNever]
        public virtual ICollection<Agreement> Agreements { get; set; }
    }
}
