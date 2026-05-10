using System;

namespace FateDefiner.Models
{
    public class InventoryItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Item";
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public string Type { get; set; } = "Miscellaneous";
        public int WeightLbs { get; set; } = 0;
        public int GoldValue { get; set; } = 0;
    }
}
