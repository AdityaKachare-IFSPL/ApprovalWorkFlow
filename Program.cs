using ApprovalWorkFlow.Models;
using ApprovalWorkFlow.Services;
using System;
using System.IO;
using System.Text.Json;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Company Approval Workflow ===");

        // Input
        Console.Write("Enter Employee Name: ");
        string employee = Console.ReadLine() ?? "";

        Console.Write("Enter Item Name: ");
        string item = Console.ReadLine() ?? "";

        Console.Write("Enter Price: ");
        decimal price = decimal.Parse(Console.ReadLine() ?? "0");

        // Load config
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string rootDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
        string[] candidates = {
            Path.Combine(rootDir, "workflowConfig.json"),
            Path.Combine(baseDir, "workflowConfig.json")
        };

        string configPath = null;
        foreach (var c in candidates)
        {
            if (File.Exists(c)) { configPath = c; break; }
        }
        if (configPath == null)
        {
            Console.WriteLine("workflowConfig.json not found. Place it in project root or output folder.");
            return;
        }

        var config = JsonSerializer.Deserialize<WorkflowConfig>(
            File.ReadAllText(configPath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Build request
        var request = new Request
        {
            EmployeeName = employee,
            ItemName = item,
            Price = price,
            Status = "Pending"
        };

        // Run workflow (JSON-driven rule engine)
        var engine = new WorkflowEngine(config!, new RuleEvaluator(), new ApprovalService());
        engine.Run(request);

        // Output
        Console.WriteLine($"\n=== Workflow Finished: {request.Status} ===");
        Console.WriteLine("\nApproval History:");
        foreach (var entry in request.History)
        {
            Console.WriteLine($"- {entry.Timestamp:dd-MM-yyyy HH:mm:ss} | {entry.Step} | {entry.Decision} | {entry.Comment}");
        }

        if (!string.IsNullOrWhiteSpace(request.AssignedTo))
        {
            Console.WriteLine($"\nFinal Assignment: {request.AssignedTo}");
        }
    }
}
