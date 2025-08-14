using ApprovalWorkFlow.Models;
using System;
using System.Data;

namespace ApprovalWorkFlow.Services
{
    public class RuleEvaluator
    {
        public bool EvaluateCondition(string condition, Request request, WorkflowSettings settings)
        {
            if (string.IsNullOrWhiteSpace(condition)) return true;

            //variable substitution
            //InvariantCulture
            string expr = condition
                .Replace("price", request.Price.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .Replace("CEOApprovalThreshold", settings.CEOApprovalThreshold.ToString(System.Globalization.CultureInfo.InvariantCulture));

            //for evaluating simple arithmetic expressions  
            var table = new DataTable();
            var result = table.Compute(expr, string.Empty);

            if (result is bool b) return b;

            if (result is IConvertible conv)
            {
                try
                {
                    return Convert.ToDecimal(conv) != 0m;
                }
                catch { /* fall through */ }
            }

            return false;
        }
    }
}
