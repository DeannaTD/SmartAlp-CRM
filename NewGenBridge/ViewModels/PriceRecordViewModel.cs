namespace NewGenBridge.ViewModels
{
    public class PriceRecordViewModel
    {
        public int PriceId { get; set; }
        public string? DivisionName { get; set; }
        public string? BranchName { get; set; }
        public int BranchId { get; set; }
        public int LevelId { get; set; }
        public decimal Sum { get; set; }
        public int Duration { get; set; }
        public decimal? MaterialPrice { get; set; }
    }
}
