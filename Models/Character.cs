using System;
using System.Collections.Generic;

namespace FateDefiner.Models
{
    public class Character
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "New Character";
        public string Race { get; set; } = "Human";
        public string Class { get; set; } = "Fighter";
        public int Level { get; set; } = 1;
        public string Background { get; set; } = string.Empty;
        public string Alignment { get; set; } = "Neutral Good";
        public string FactionId { get; set; } = string.Empty;
        public CharacterStats Stats { get; set; } = new CharacterStats();
        public List<InventoryItem> Inventory { get; set; } = new List<InventoryItem>();
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public override string ToString() => Name;
    }
}
