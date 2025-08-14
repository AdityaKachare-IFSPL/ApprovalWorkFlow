using System;

namespace ApprovalWorkFlow.Models
{
    public class ApprovalHistoryEntry
    {
        public string Step { get; set; } = "";
        public string Decision { get; set; } = "";   // Approved/Rejected/Assigned
        public string Comment { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}
