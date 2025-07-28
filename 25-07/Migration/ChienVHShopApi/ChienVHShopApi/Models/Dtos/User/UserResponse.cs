namespace ChienVHShopAPI.Dtos.User
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        // Optional: Number of posts/products
        public int ProductCount { get; set; }
        public int NewsCount { get; set; }
    }
}
