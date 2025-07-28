namespace ChienVHShopAPI.Models
{
    public class Cart
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public Cart(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        // Optional: Add a parameterless constructor if you plan to use this as a model binder or deserialize
        public Cart() { }
    }
}
