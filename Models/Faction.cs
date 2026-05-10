using System;
using System.Collections.Generic;

namespace FateDefiner.Models
{
    public class Faction
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "New Faction";
        public string Alignment { get; set; } = "Neutral";
        public string Description { get; set; } = string.Empty;
        public string Goals { get; set; } = string.Empty;
        public string RelationshipNotes { get; set; } = string.Empty;
        public List<NPC> Members { get; set; } = new List<NPC>();
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public override string ToString() => Name;
    }
}
