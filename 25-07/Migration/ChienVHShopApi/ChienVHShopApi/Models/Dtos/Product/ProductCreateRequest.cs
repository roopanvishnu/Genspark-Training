namespace ChienVHShopAPI.Dtos.Product
{
    public class ProductCreateRequest
    {
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
    }
}
