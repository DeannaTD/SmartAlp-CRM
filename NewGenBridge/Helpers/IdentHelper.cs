using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace NewGenBridge.Helpers
{
    public class IdentHelper
    {
    }

    public enum NPUserRole
    {
        Student,
        Mentor,
        Manager,
        Admin
    }

    public struct NPUR
    {
        public static readonly string Student = "Student";
        public static readonly string Mentor = "Mentor";
        public static readonly string Manager = "Manager";
        public static readonly string Admin = "Admin";
        public static readonly string Supervisor = "Supervisor";
    }

    public struct UserData
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public struct UserStatus
    {
        public const int All = 0;
        public const int Active = 1;
        public const int Blocked = 2;
        public const int Archived = 3;

        public static string GetName(int? statusId)
        {
            return statusId switch
            {
                1 => "Active",
                2 => "Blocked",
                3 => "Archived",
                _ => "Unknown"
            };
        }

        public static List<(int, string)> GetCollection()
        {
            return
            [
                (0, "All"),
                (1, "Active"),
                (2, "Blocked"),
                (3, "Archived")
            ];
        }

        public static string GetOpposite(int? statusId)
        {
            return statusId switch
            {
                2 => "Unblock",
                3 => "Restore",
                _ => "Unknown"
            };
        }

        public static string GetJsonCollection()
        {
            return """[{"id":1,"name":"Active"},{"id":2,"name":"Blocked"},{"id":3,"name":"Archived"}]""";
        }
    }

    public struct RequestStatus
    {
        public const int New = 0;
        public const int Pending = 1;
        public const int Completed = 2;
        public static string GetName(int? statusId)
        {
            return statusId switch
            {
                0 => "new",
                1 => "pending",
                2 => "completed",
                _ => "unknown"
            };
        }
    }
}
