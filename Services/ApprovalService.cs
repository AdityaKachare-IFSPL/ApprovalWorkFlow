using ApprovalWorkFlow.Models;
using System;

namespace ApprovalWorkFlow.Services
{
    public class ApprovalService
    {
        public void RecordApproval(Request request, string step)
        {
            request.History.Add(new ApprovalHistoryEntry
            {
                Step = step,
                Decision = "Approved",
                Comment = "",
                Timestamp = DateTime.Now
            });
        }

        public void RecordRejection(Request request, string step, string reason)
        {
            request.Status = "Rejected";
            request.History.Add(new ApprovalHistoryEntry
            {
                Step = step,
                Decision = "Rejected",
                Comment = reason,
                Timestamp = DateTime.Now
            });
        }

        public void RecordAssignment(Request request, string assignedTo)
        {
            request.AssignedTo = assignedTo ?? "";
            request.History.Add(new ApprovalHistoryEntry
            {
                Step = "AdminAssignment",
                Decision = "Assigned",
                Comment = $"Assigned to {assignedTo}",
                Timestamp = DateTime.Now
            });
        }
    }
}
