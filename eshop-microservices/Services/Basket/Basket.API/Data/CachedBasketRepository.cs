
namespace Basket.API.Data
{
    public class CachedBasketRepository(IBasketRepository repository)
        : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
        {
            return await repository.GetBasket(userName, cancellationToken);
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            return await repository.StoreBasket(basket, cancellationToken);
        }

        public Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            return repository.DeleteBasket(userName, cancellationToken);
        }
    }
}
