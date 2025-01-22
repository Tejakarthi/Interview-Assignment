using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;
using System.Numerics;

namespace RL.Backend.UnitTests;

//[TestClass]
//public class AddUserToProcedureTests
//{
//    [TestMethod]
//    [DataRow(-1)]
//    [DataRow(0)]
//    [DataRow(int.MinValue)]
//    public async Task AddUserToProcedureTests_InvalidUser_ReturnsBadRequest(int planId)
//    {
//        //Given
//        var context = new Mock<RLContext>();
//        var sut = new AddUserToProcedureCommandHandler(context.Object);
//        var request = new AddProcedureToUserCommand()
//        {
//            PlanId = planId,
//            ProcedureId = 1,
//            UserId = 99,
//            ClearAll = 0
//        };
//        //When
//        var result = await sut.Handle(request, new CancellationToken());

//        //Then
//        result.Exception.Should().BeOfType(typeof(BadRequestException));
//        result.Succeeded.Should().BeFalse();
//    }
//}

[TestClass]
public class AddUserToProcedureTests
{

    [TestMethod]
    public async Task Handle_InvalidUserId_ShouldThrowBadRequestException()
    {
        
        //Given
        var context = new Mock<RLContext>();
        var sut = new AddUserToProcedureCommandHandler(context.Object);
        var request = new AddProcedureToUserCommand
        {
            UserId = 0,
            ProcedureId = 1,
            PlanId = 1
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_InvalidProcedureId_ShouldThrowBadRequestException()
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new AddUserToProcedureCommandHandler(context.Object);
        var request = new AddProcedureToUserCommand
        {
            UserId = 1,
            ProcedureId = 0,
            PlanId = 1
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle_InvalidPlanId_ShouldThrowBadRequestException()
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new AddUserToProcedureCommandHandler(context.Object);
        var request = new AddProcedureToUserCommand
        {
            UserId = 1,
            ProcedureId = 1,
            PlanId = 0
        };
        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }
    [TestMethod]
    public async Task Handle_ExistUserCheck_ReturnsSuccess()
    {
        // Given
        var context = DbContextHelper.CreateContext();
        var sut = new AddUserToProcedureCommandHandler(context);

        var request = new AddProcedureToUserCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = 1
        };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1 });
        await context.SaveChangesAsync();

        // When
        var result = await sut.Handle(request, CancellationToken.None);

        // Then
        var dbPlanProcedure = await context.UserProcedures
            .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId &&
                                       pp.ProcedureId == request.ProcedureId &&
                                       pp.UserId == request.UserId);

        dbPlanProcedure.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }



}
