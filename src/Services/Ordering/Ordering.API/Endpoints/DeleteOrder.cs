
using Ordering.Application.Orders.Commands.DeleteOrder;

namespace Ordering.API.Endpoints
{
    public record DeleteOrderResponse(bool IsSuccess);

    public class DeleteOrder : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/orders/{id:guid}", async (Guid id, ISender sender) =>
            {
                var command = new DeleteOrderCommand(id);
                var result = await sender.Send(command);
                var response = new DeleteOrderResponse(result.Success);
                return Results.Ok(response);
            })
            .WithName("DeleteOrder")
            .Produces<DeleteOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Delete order")
            .WithDescription("Delete Order");
        }
    }
}
