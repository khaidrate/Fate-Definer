using System;

namespace FateDefiner.Models
{
    public class NPC
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "New NPC";
        public string Race { get; set; } = "Human";
        public string Role { get; set; } = "Commoner";
        public string Attitude { get; set; } = "Neutral";
        public string Description { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public override string ToString() => Name;
    }
}
