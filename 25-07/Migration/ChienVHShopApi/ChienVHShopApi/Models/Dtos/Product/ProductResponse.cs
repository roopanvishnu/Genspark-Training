namespace ChienVHShopAPI.Dtos.Product
{
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public double? Price { get; set; }

        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? ModelId { get; set; }
        public int? StorageId { get; set; }

        public DateTime? SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }

        public int? IsNew { get; set; }

        // Optional names (if you want to show them via JOINs)
        public string CategoryName { get; set; }
        public string ColorName { get; set; }
        public string ModelName { get; set; }
        public string Username { get; set; }
    }
}
