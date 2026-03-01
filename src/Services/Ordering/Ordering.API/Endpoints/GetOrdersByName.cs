
using Ordering.Application.Orders.Query.GetOrdersByName;

namespace Ordering.API.Endpoints
{
    public record GetOrdersByNameResponse(IEnumerable<OrderDto> Orders);

    public class GetOrdersByName : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/{userName}", async (string userName, ISender sender) =>
            {
                var query = new GetOrdersByNameQuery(userName);
                var orders = await sender.Send(query);
                var response = new GetOrdersByNameResponse(orders.Adapt<IEnumerable<OrderDto>>());
                return Results.Ok(response);
            })
            .WithName("GetOrdersByName")
            .Produces<GetOrdersByNameResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get orders by user name")
            .WithDescription("Get orders by user name");
        }
    }
}
