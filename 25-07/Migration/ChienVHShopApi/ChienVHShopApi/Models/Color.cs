using System.Collections.Generic;

namespace ChienVHShopAPI.Models
{
    public class Color
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; }

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
