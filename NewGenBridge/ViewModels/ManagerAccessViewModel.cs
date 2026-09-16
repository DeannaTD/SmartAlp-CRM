namespace NewGenBridge.ViewModels
{
    public class ManagerAccessViewModel
    {
        public string StructureName { get; set; }
        public int BranchId { get; set; }
        public int DivisionId { get; set; }
        public List<(string, string)> ManagersIds { get; set; }
    }
}
