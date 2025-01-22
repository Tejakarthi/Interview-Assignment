using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Data;
using RL.Data.DataModels;
using System.Data.Entity;

namespace RL.Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class PlanProcedureController : ControllerBase
{
    private readonly ILogger<PlanProcedureController> _logger;
    private readonly RLContext _context;

    public PlanProcedureController(ILogger<PlanProcedureController> logger, RLContext context)
    {
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [EnableQuery]
    public IEnumerable<PlanProcedure> Get()
    {
        List<PlanProcedure> plans = new List<PlanProcedure>();  // Initialize plans as an empty list
                                                                // Fetch all plan procedures
        List<PlanProcedure> data = _context.PlanProcedures.ToList();

        foreach (PlanProcedure procedure in data)
        {
            var id = procedure.PlanId;
            var ProcedureId = procedure.ProcedureId;

            // Fetch user procedures related to the plan
            List<UserProcedure> users = _context.UserProcedures.Where(a => a.PlanId == id && a.ProcedureId == ProcedureId).ToList();
            Procedure procedures = _context.Procedures.Where(a => a.ProcedureId == ProcedureId).Single();

            if (procedure.Procedure == null)
            {
                procedure.Procedure = new Procedure();
            }
            procedure.Procedure = procedures;
            // Ensure the UserProcedures list is initialized
            if (procedure.UserProcedures == null)
            {
                procedure.UserProcedures = new List<UserProcedure>();
            }

            procedure.UserProcedures.AddRange(users);

            // Add the updated procedure to the plans list
            plans.Add(procedure);  // Use Add() here
        }

        return plans;  // Return the updated list
    }

}
