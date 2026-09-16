using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace NewGenBridge.ViewModels
{
    public class AddAccessViewModel
    {
        public int DivisionId { get; set; }
        public int? BranchId { get; set; }
        public string UserId { get; set; }
        [ValidateNever]
        public string StructureName { get; set; }
    }
}
