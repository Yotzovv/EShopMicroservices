
namespace Catalog.API.Products.GetProductByCategory;

public record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;
public record GetProductByCategoryResult(IEnumerable<Product> Products);

public class GetProductByCategoryQueryHandler(IDocumentSession session)
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await session.Query<Product>()
            .Where(p => p.Category.Contains(request.Category))
            .ToListAsync();

        return new GetProductByCategoryResult(products);
    }
}