using System;
using System.Collections.Generic;

namespace FateDefiner.Models
{
    public class Campaign
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "New Campaign";
        public string Description { get; set; } = string.Empty;
        public string Setting { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastModified { get; set; } = DateTime.Now;
        public List<Character> Characters { get; set; } = new List<Character>();
        public List<Faction> Factions { get; set; } = new List<Faction>();
        public List<Note> SessionNotes { get; set; } = new List<Note>();
    }
}
