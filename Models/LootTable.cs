using System;
using System.Collections.Generic;
using System.Linq;

namespace FateDefiner.Models
{
    public class LootEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Rarity { get; set; } = "Common";
        public int GoldValue { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Static loot table. Generates random treasure based on encounter tier and party level.
    /// Demonstrates Sorting + LINQ techniques.
    /// </summary>
    public static class LootTable
    {
        private static readonly Random _rng = new Random();

        private static readonly Dictionary<string, List<LootEntry>> _pools = new()
        {
            ["Low"] = new List<LootEntry>
            {
                new() { Name = "Healing Potion",         Type = "Consumable", Rarity = "Common",   GoldValue = 50,   Description = "Restores 2d4+2 HP when consumed."               },
                new() { Name = "Rope (50 ft)",           Type = "Equipment",  Rarity = "Common",   GoldValue = 1,    Description = "Hempen rope, well-used but sturdy."              },
                new() { Name = "Torch Bundle (x10)",     Type = "Equipment",  Rarity = "Common",   GoldValue = 1,    Description = "Ten torches, good for an hour each."            },
                new() { Name = "Traveler's Cloak",       Type = "Armor",      Rarity = "Common",   GoldValue = 5,    Description = "A worn but functional traveling cloak."         },
                new() { Name = "Dagger +1",              Type = "Weapon",     Rarity = "Uncommon", GoldValue = 500,  Description = "A blade that glows faintly when sharpened."     },
                new() { Name = "Spell Scroll (Cantrip)", Type = "Scroll",     Rarity = "Common",   GoldValue = 25,   Description = "A cantrip inscribed in fading ink."             },
                new() { Name = "Potion of Climbing",     Type = "Consumable", Rarity = "Common",   GoldValue = 75,   Description = "Grants a climb speed of 30 ft for 1 hour."     },
                new() { Name = "Map (Partial)",          Type = "Misc",       Rarity = "Common",   GoldValue = 10,   Description = "A hand-drawn map of the local area."           },
            },
            ["Medium"] = new List<LootEntry>
            {
                new() { Name = "Potion of Greater Healing", Type = "Consumable", Rarity = "Uncommon", GoldValue = 150,  Description = "Restores 4d4+4 HP."                          },
                new() { Name = "Shortsword +1",             Type = "Weapon",     Rarity = "Uncommon", GoldValue = 1000, Description = "A blade that never dulls."                    },
                new() { Name = "Shield +1",                 Type = "Armor",      Rarity = "Uncommon", GoldValue = 1000, Description = "Arcane runes reinforce this battered shield." },
                new() { Name = "Bag of Holding",            Type = "Wondrous",   Rarity = "Uncommon", GoldValue = 4000, Description = "Holds up to 500 lbs in an extradimensional space." },
                new() { Name = "Boots of Elvenkind",        Type = "Wondrous",   Rarity = "Uncommon", GoldValue = 2500, Description = "Advantage on Dexterity (Stealth) checks."    },
                new() { Name = "Cloak of Protection",       Type = "Wondrous",   Rarity = "Uncommon", GoldValue = 3500, Description = "+1 bonus to AC and all saving throws."        },
                new() { Name = "Spell Scroll (Level 2)",    Type = "Scroll",     Rarity = "Uncommon", GoldValue = 250,  Description = "A second-level spell sealed in silver ink."   },
                new() { Name = "Goggles of Night",          Type = "Wondrous",   Rarity = "Uncommon", GoldValue = 2500, Description = "Darkvision 60 ft while worn."                },
            },
            ["High"] = new List<LootEntry>
            {
                new() { Name = "Longsword +2",           Type = "Weapon",   Rarity = "Rare",      GoldValue = 5000,  Description = "Crackles with suppressed magical energy."       },
                new() { Name = "Plate Armor +1",         Type = "Armor",    Rarity = "Rare",      GoldValue = 6000,  Description = "Full plate with an interlocking arcane ward."  },
                new() { Name = "Ring of Protection",     Type = "Ring",     Rarity = "Rare",      GoldValue = 3500,  Description = "+1 to AC and all saving throws."               },
                new() { Name = "Necklace of Fireballs",  Type = "Wondrous", Rarity = "Rare",      GoldValue = 5000,  Description = "Beads that each detonate as Fireball (DC 15)." },
                new() { Name = "Winged Boots",           Type = "Wondrous", Rarity = "Rare",      GoldValue = 8000,  Description = "4 hours of flying speed equal to your walk speed." },
                new() { Name = "Staff of Healing",       Type = "Staff",    Rarity = "Rare",      GoldValue = 8000,  Description = "10 charges. Cure Wounds, Lesser Restoration, Mass Cure Wounds." },
                new() { Name = "Gem of Seeing",          Type = "Wondrous", Rarity = "Rare",      GoldValue = 5000,  Description = "True Sight 120 ft, 3 charges/day."             },
                new() { Name = "Oathbow",                Type = "Weapon",   Rarity = "Rare",      GoldValue = 6500,  Description = "Whispers a sworn oath against a chosen enemy." },
            },
            ["Legendary"] = new List<LootEntry>
            {
                new() { Name = "Vorpal Sword",              Type = "Weapon",   Rarity = "Legendary", GoldValue = 50000,  Description = "Severs limbs on a nat 20. Speaks quietly to its wielder." },
                new() { Name = "Holy Avenger",              Type = "Weapon",   Rarity = "Legendary", GoldValue = 40000,  Description = "The paladin's perfect blade. Radiates protection against fiends." },
                new() { Name = "Armor of Invulnerability",  Type = "Armor",    Rarity = "Legendary", GoldValue = 60000,  Description = "Resistance to nonmagical damage; once/day, immunity for 10 min." },
                new() { Name = "Ring of Three Wishes",      Type = "Ring",     Rarity = "Legendary", GoldValue = 90000,  Description = "Three wish spells. Use wisely."                },
                new() { Name = "Sphere of Annihilation",    Type = "Wondrous", Rarity = "Legendary", GoldValue = 75000,  Description = "A void that destroys everything it touches."  },
                new() { Name = "Staff of the Magi",         Type = "Staff",    Rarity = "Legendary", GoldValue = 65000,  Description = "50 charges of wizard spells. Retributive strike if broken." },
                new() { Name = "Deck of Many Things",       Type = "Wondrous", Rarity = "Legendary", GoldValue = 100000, Description = "Draw and face your fate. Most adventurers regret this." },
                new() { Name = "Ioun Stone (Mastery)",      Type = "Wondrous", Rarity = "Legendary", GoldValue = 55000,  Description = "Proficiency bonus increases by 1 while orbiting." },
            },
        };

        public static List<string> Tiers => _pools.Keys.OrderBy(TierRank).ToList();

        public static List<LootEntry> Generate(string tier, int partyLevel)
        {
            var results = new List<LootEntry>();

            // Gold payout scaled by party level
            int gold = GetGold(tier, partyLevel);
            results.Add(new LootEntry
            {
                Name = $"{gold:N0} Gold Pieces",
                Type = "Currency",
                Rarity = "Common",
                GoldValue = gold,
                Description = "Coin, gems, or a mixture of both."
            });

            if (!_pools.TryGetValue(tier, out var pool))
                pool = _pools["Medium"];

            // Shuffle with LINQ (OrderBy random), then take a few items
            int count = tier switch
            {
                "Low"       => _rng.Next(1, 3),
                "Medium"    => _rng.Next(2, 4),
                "High"      => _rng.Next(3, 5),
                "Legendary" => _rng.Next(3, 6),
                _           => 2
            };

            var picks = pool.OrderBy(_ => _rng.Next()).Take(count);
            results.AddRange(picks);

            return results;
        }

        private static int GetGold(string tier, int partyLevel) => tier switch
        {
            "Low"       => _rng.Next(5, 25)   * Math.Max(1, partyLevel),
            "Medium"    => _rng.Next(20, 80)  * Math.Max(1, partyLevel),
            "High"      => _rng.Next(80, 250) * Math.Max(1, partyLevel),
            "Legendary" => _rng.Next(200, 800)* Math.Max(1, partyLevel),
            _           => 10
        };

        private static int TierRank(string t) => t switch
        {
            "Low" => 0, "Medium" => 1, "High" => 2, "Legendary" => 3, _ => 1
        };
    }
}
