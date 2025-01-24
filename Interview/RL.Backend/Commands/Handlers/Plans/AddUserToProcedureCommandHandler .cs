using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;


namespace RL.Backend.Commands.Handlers.Plans;

public class AddUserToProcedureCommandHandler : IRequestHandler<AddProcedureToUserCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;
    private readonly ILogger<AddUserToProcedureCommandHandler> _logger;


    public AddUserToProcedureCommandHandler(RLContext context, ILogger<AddUserToProcedureCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<Unit>> Handle(AddProcedureToUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("request:");

            if (request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid Plan"));
            // Validate User
            var plan = await _context.Plans
                .Include(p => p.PlanProcedures)
                .FirstOrDefaultAsync(p => p.PlanId == request.PlanId);
            var procedure = await _context.Procedures.FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId);

            _logger.LogInformation("plan:" + plan.PlanId.ToString());
            _logger.LogInformation("procedure:" +procedure.ProcedureId.ToString());

            if (plan is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));
            if (procedure is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));

            // Handle ClearAll logic
            if (request.ClearAll == 1)
            {
                var existingUserProcedures = await _context.UserProcedures
                    .Where(up => up.ProcedureId == request.ProcedureId && up.PlanId == request.PlanId)
                    .ToListAsync(cancellationToken);

                if (existingUserProcedures.Any())
                {
                    _context.UserProcedures.RemoveRange(existingUserProcedures);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return ApiResponse<Unit>.Succeed(Unit.Value);
            }

            // Check if the entry already exists
            bool exists = await _context.UserProcedures
                .AnyAsync(up => up.UserId == request.UserId && up.ProcedureId == request.ProcedureId && up.PlanId == request.PlanId, cancellationToken);
            _logger.LogInformation("UserProcedures:" + exists);

            if (exists)
            {
                var existingUserProcedure = await _context.UserProcedures
                    .SingleAsync(up => up.UserId == request.UserId && up.ProcedureId == request.ProcedureId && up.PlanId == request.PlanId, cancellationToken);

                _context.UserProcedures.Remove(existingUserProcedure);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<Unit>.Succeed(Unit.Value);
            }

            // Add a new UserProcedure entry
            var userProcedure = new UserProcedure
            {
                UserId = request.UserId,
                ProcedureId = request.ProcedureId,
                PlanId = request.PlanId
            };

            _context.UserProcedures.Add(userProcedure);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception:" + ex.StackTrace);

            return ApiResponse<Unit>.Fail(ex);
        }
    }
}
