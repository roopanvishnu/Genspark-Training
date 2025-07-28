using System.Collections.Generic;

namespace ChienVHShopAPI.Models
{
    public class Model
    {
        public int ModelId { get; set; }
        public string ModelName { get; set; }

        // Navigation
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
