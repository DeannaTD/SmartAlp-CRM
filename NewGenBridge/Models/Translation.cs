using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NewGenBridge.Models
{
    public class Translation
    {
        [Key]
        public string NPKey { get; set; }

        public string EnValue { get; set; } = "";
        public string RuValue { get; set; } = "";
        public string TrValue { get; set; } = "";
        public string FrValue { get; set; } = "";
        public string ItValue { get; set; } = "";
        public string DeValue { get; set; } = "";

    }
}
