
namespace Catalog.API.Products.GetProductByCategory;

public record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;
public record GetProductByCategoryResult(IEnumerable<Product> Products);

public class GetProductByCategoryHandler(IDocumentSession session, ILogger<GetProductByCategoryHandler> logger)
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {QueryName} for Category: {Category}", nameof(GetProductByCategoryQuery), request.Category);

        var products = await session.Query<Product>()
            .Where(p => p.Category.Contains(request.Category))
            .ToListAsync();

        return new GetProductByCategoryResult(products);
    }
}