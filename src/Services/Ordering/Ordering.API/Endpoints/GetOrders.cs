using BuildingBlocks.Pagination;
using Ordering.Application.Orders.Query.GetOrders;

namespace Ordering.API.Endpoints
{
    public record GetOrdersResponse(PaginationResult<OrderDto> Orders);

    public class GetOrders : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetOrdersQuery(request));
                var response = result.Adapt<GetOrdersResponse>();
                return Results.Ok(response);
            })
            .WithName("GetOrders")
            .Produces<GetOrdersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get orders")
            .WithDescription("Get orders");
        }
    }
}
