
using Ordering.Application.Orders.Query.GetOrdersByCustomer;

namespace Ordering.API.Endpoints
{
    public record GetOrderByCustomerResponse(IEnumerable<OrderDto> Orders);

    public class GetOrdersByCustomer : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/customer/{customerId:guid}", async (Guid customerId, ISender sender) =>
            {
                var query = new GetOrdersByCustomerQuery(customerId);
                var result = await sender.Send(query);
                var response = result.Adapt<GetOrderByCustomerResponse>();
                return Results.Ok(response);
            })
            .WithName("GetOrdersByCustomer")
            .Produces<GetOrderByCustomerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get orders by customer id")
            .WithDescription("Get orders by customer id");
        }
    }
}
