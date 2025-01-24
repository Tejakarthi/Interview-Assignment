using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Data;
using RL.Data.DataModels;

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
        var planProcedures = _context.PlanProcedures
            .Select(planProcedure => new PlanProcedure
            {
                ProcedureId = planProcedure.ProcedureId,
                PlanId = planProcedure.PlanId,
                UserId = planProcedure.UserId,
                Procedure = _context.Procedures.FirstOrDefault(p => p.ProcedureId == planProcedure.ProcedureId),
                UserProcedures = _context.UserProcedures
                    .Where(up => up.PlanId == planProcedure.PlanId && up.ProcedureId == planProcedure.ProcedureId)
                    .ToList(),
                CreateDate = planProcedure.CreateDate,
                UpdateDate = planProcedure.UpdateDate
            })
            .ToList(); 

        return planProcedures;
    }


}
