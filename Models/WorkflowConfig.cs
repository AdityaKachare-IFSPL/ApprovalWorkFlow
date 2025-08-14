using System.Collections.Generic;

namespace ApprovalWorkFlow.Models
{
    public class WorkflowConfig
    {
        public WorkflowSettings WorkflowSettings { get; set; }
        public ApprovalFlow ApprovalFlow { get; set; }
        public Dictionary<string, RejectionRule> RejectionLogic { get; set; }
    }

    public class WorkflowSettings
    {
        public decimal CEOApprovalThreshold { get; set; }
        public string CurrencySymbol { get; set; }
        public bool RequireAllApprovalsForHighValue { get; set; }
    }

    public class ApprovalFlow
    {
        public string InitialStep { get; set; }
        public List<ApprovalStep> StandardFlow { get; set; }
    }

    public class ApprovalStep
    {
        public string Step { get; set; }
        public string NextStep { get; set; }
        public bool CanReject { get; set; }
        public bool Required { get; set; }
        public bool RequiresAssignment { get; set; }
        public ConditionalRouting ConditionalRouting { get; set; }
    }

    public class ConditionalRouting
    {
        public string Condition { get; set; }       // e.g., "price > CEOApprovalThreshold"
        public string AlternateStep { get; set; }   // e.g., "CEO"
    }

    public class RejectionRule
    {
        public string Result { get; set; }
        public string Reason { get; set; }
    }
}
