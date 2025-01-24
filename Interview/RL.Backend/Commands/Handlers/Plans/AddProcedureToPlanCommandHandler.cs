using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans;

public class AddProcedureToPlanCommandHandler : IRequestHandler<AddProcedureToPlanCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;
    private readonly ILogger<AddProcedureToPlanCommandHandler> _logger;


    public AddProcedureToPlanCommandHandler(RLContext context, ILogger<AddProcedureToPlanCommandHandler> logger)
    {
        _context = context;
        _logger = logger;

    }

    public async Task<ApiResponse<Unit>> Handle(AddProcedureToPlanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("request:" + request.PlanId.ToString());

            //Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

            var plan = await _context.Plans
                .Include(p => p.PlanProcedures)
                .FirstOrDefaultAsync(p => p.PlanId == request.PlanId);
            var procedure = await _context.Procedures.FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);
            _logger.LogInformation("plan:" + plan.PlanId.ToString());
            _logger.LogInformation("procedure:" + procedure.ProcedureId);

            if (plan is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));
            if (procedure is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));

            //Already has the procedure, so just succeed
            if (plan.PlanProcedures.Any(p => p.ProcedureId == procedure.ProcedureId))
                return ApiResponse<Unit>.Succeed(new Unit());

            plan.PlanProcedures.Add(new PlanProcedure
            {
                ProcedureId = procedure.ProcedureId
            });

            await _context.SaveChangesAsync();

            return ApiResponse<Unit>.Succeed(new Unit());
            

        }
        catch (Exception e)
        {
            _logger.LogError("Exception:" + e.StackTrace);

            return ApiResponse<Unit>.Fail(e);
        }
    }
}