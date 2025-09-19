namespace ConsoleApp1;

public class ShoppingCart
{
    public string Username {get; set;} = default!;

    public List<ShoppingCartItem> Items {get; set;} = new();

    public decimal TotalPrice => items.Sum(item => item.Price * item.Quantity);

    public ShoppingCart(string userName) 
    {
        Username = userName;
    }

    // Required for deserialization
    public ShoppingCart() { }    
}
