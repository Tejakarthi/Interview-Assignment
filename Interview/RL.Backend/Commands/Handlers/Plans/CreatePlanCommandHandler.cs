using MediatR;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;
using System.Data.Entity;
using System.Numerics;

namespace RL.Backend.Commands.Handlers.Plans;

public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, ApiResponse<Plan>>
{
    private readonly RLContext _context;

    public CreatePlanCommandHandler(RLContext context)
    {
        _context = context;
    }


    public async Task<ApiResponse<Plan>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var plans = _context.Plans.ToList();
            var plan = plans.FirstOrDefault(p => p.PlanId == request.PlanId);

            if (plan == null)
            {
                plan = new Plan
                {
                    PlanId = request.PlanId
                };

                _context.Plans.Add(plan);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return ApiResponse<Plan>.Succeed(plan);
        }
        catch (Exception e)
        {
            return ApiResponse<Plan>.Fail(e);
        }
    }


}