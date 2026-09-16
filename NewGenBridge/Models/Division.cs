using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace NewGenBridge.Models
{
    public class Division
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [ValidateNever]
        public virtual ICollection<Branch> Branches { get; set; }
    }
}
