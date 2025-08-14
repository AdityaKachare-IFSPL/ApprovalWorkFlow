using ApprovalWorkFlow.Models;
using System.Collections.Generic;

namespace ApprovalWorkFlow.Models
{
    public class Request
    {
        public string EmployeeName { get; set; } = "";
        public string ItemName { get; set; } = "";
        public decimal Price { get; set; }
        public string Status { get; set; } = "Pending"; // "Pending", "Completed", "Rejected"
        public string AssignedTo { get; set; } = "";
        public List<ApprovalHistoryEntry> History { get; set; } = new();
    }
}
