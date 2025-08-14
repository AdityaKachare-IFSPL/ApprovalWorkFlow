using ApprovalWorkFlow.Models;
//using ApprovalWorkFlow.Services;
//using System;
//using System.Linq;

namespace ApprovalWorkFlow.Services
{
    public class WorkflowEngine
    {
        private readonly WorkflowConfig _config;
        private readonly RuleEvaluator _evaluator;
        private readonly ApprovalService _service;

        public WorkflowEngine(WorkflowConfig config, RuleEvaluator evaluator, ApprovalService service)
        {
            _config = config;
            _evaluator = evaluator;
            _service = service;
        }

        public void Run(Request request)
        {
            string current = _config.ApprovalFlow.InitialStep;

            while (current != "Completed" && request.Status != "Rejected")
            {
                var step = _config.ApprovalFlow.StandardFlow.FirstOrDefault(s => s.Step == current);
                if (step == null)
                {
                    Console.WriteLine($"Step '{current}' not found in config. Ending workflow.");
                    break;
                }

                // Assignment step first
                if (step.RequiresAssignment)
                {
                    Console.Write($"Enter admin to assign for {request.EmployeeName}: ");
                    var assignedTo = Console.ReadLine();
                    _service.RecordAssignment(request, assignedTo);
                    current = step.NextStep;
                    continue;
                }

                // Approval step
                string currency = _config.WorkflowSettings.CurrencySymbol ?? "";
                Console.Write($"{step.Step} Approval for {request.EmployeeName} of {currency}{request.Price:N0} - Approve? (yes/no): ");
                string input = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                if (input == "yes")
                {
                    _service.RecordApproval(request, step.Step);

                    // After approval → check conditional routing
                    if (step.ConditionalRouting != null && !string.IsNullOrWhiteSpace(step.ConditionalRouting.Condition))
                    {
                        bool routeToAlternate = _evaluator.EvaluateCondition(
                            step.ConditionalRouting.Condition, request, _config.WorkflowSettings);

                        if (routeToAlternate && !string.IsNullOrWhiteSpace(step.ConditionalRouting.AlternateStep))
                        {
                            current = step.ConditionalRouting.AlternateStep;
                            continue;
                        }
                    }

                    // If no conditional routing, go to next step
                    current = step.NextStep;
                }
                else
                {
                    // Lookup rejection reason from JSON if provided
                    string key = $"{step.Step}Rejects";
                    string reason = _config.RejectionLogic != null && _config.RejectionLogic.ContainsKey(key)
                        ? _config.RejectionLogic[key].Reason
                        : $"{step.Step} rejected";

                    _service.RecordRejection(request, step.Step, reason);
                    break;
                }
            }

            if (request.Status != "Rejected")
                request.Status = "Completed";
        }
    }
}
