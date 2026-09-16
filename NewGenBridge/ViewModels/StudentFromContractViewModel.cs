namespace NewGenBridge.ViewModels
{
    public class StudentFromContractViewModel
    {
        public int LeadId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        public int LevelId { get; set; }
        public int GroupId { get; set; }
        public int ContractId { get; set; } = 0;
    }
}
