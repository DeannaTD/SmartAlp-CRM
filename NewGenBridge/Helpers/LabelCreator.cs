using System.Text;
using System.Text.RegularExpressions;

namespace NewGenBridge.Helpers
{
    public class LabelCreator
    {
        public static string GetGroupLabel(string branchName, DayOfWeek groupDay, TimeSpan groupTime)
        {
            //ToDo: Language support of day of weeks
            return $"{branchName} - {groupDay} {groupTime.ToString(@"hh\:mm")}";
        }

        public static string CreateUserName(string name)
        {
            string latinizedName = NameLatinizer.LatinizeUserName(name);
            string userName = latinizedName.Normalize(NormalizationForm.FormD);
            return Regex.Replace(userName, @"[^a-z]", "");
        }

        public static string CreateProjectLabel(int lid, int pid) => lid + "_" + pid;
        public static string CreateProfileLabel(string uid, string extension) => $"ap-{uid}.{extension}";

        public static string CreateHubFileLabel(int fileId, string fileName)
        {
            fileName = NameLatinizer.LatinizeFileName(fileName);
            fileName = Regex.Replace(fileName, @"[^a-z0-9]", "");
            return $"{fileId}-{fileName}";
        }

        public static string CreateAgreementLabel(string companyName, int count = 0)
        {
            companyName = Regex.Replace(companyName, @"[^a-zA-Zа-яА-Я]", "");
            if(companyName.Length < 2)
            {
                companyName = companyName.PadRight(2, 'X').ToUpperInvariant();
            }
            else
            {
                companyName = companyName.Substring(0, 2).ToUpperInvariant();
            }
            DateOnly date = DateOnly.FromDateTime(DateTime.Now);
            return $"#{companyName}{date:ddMM}{(count > 0 ? $"_{count}" : "")}";
        }

    }

    public class LogLabel
    {
        public static string Login(string? userName) => $"LogIn: {userName ?? "unknown"}";
        public static string Logout(string? userName) => $"LogOut: {userName ?? "unknown"}";
        public static string Register(string? createdBy, string userFullName, string role) => $"UserRegister: {userFullName} in role {role} by {createdBy}";
        public static string CreateBranch(string? createdBy, string branchName, int divisionId) => $"BranchCreate: {branchName} in division {divisionId} by {createdBy}";
        public static string CreateDivision(string? createdBy, string branchName, int divisionId) => $"DivisionCreate: {branchName} with ID {divisionId} by {createdBy}";
        public static string CreateGroup(string? createdBy, string groupName, int branchId) => $"GroupCreate: {groupName} in branch {branchId} by {createdBy}";
        public static string AccessGrant(string? by, string to, int divisionid, int? branchId) => $"AccessGrant: to {to} by {by} in {divisionid} : {branchId ?? 0}";
    }
}
