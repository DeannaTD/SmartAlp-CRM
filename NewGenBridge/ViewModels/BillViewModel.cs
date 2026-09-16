namespace NewGenBridge.ViewModels
{
    public class BillViewModel
    {
        public string PaymentNumber { get; set; }
        public string ContractNumber { get; set; }
        public string StudentName { get; set; }
        public decimal Sum { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool Status { get; set; }
        public int BillId { get; set; }
        public bool HasFile { get; set; } = false;
    }
}
