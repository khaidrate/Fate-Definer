using System;
using System.Collections.Generic;
using System.Linq;

namespace FateDefiner.Models
{
    public class EncounterEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Medium";
        public string Environment { get; set; } = string.Empty;
    }

    /// <summary>
    /// Static encounter table with preset encounters per environment.
    /// Uses weighted random selection (Searching and Sorting / LINQ technique).
    /// </summary>
    public static class EncounterTable
    {
        private static readonly Random _rng = new Random();

        private static readonly Dictionary<string, List<EncounterEntry>> _tables = new()
        {
            ["Forest"] = new List<EncounterEntry>
            {
                new() { Name = "Wolf Pack",         Description = "A pack of 3d6 hungry wolves stalks the party through the undergrowth.",    Difficulty = "Easy"   },
                new() { Name = "Bandit Ambush",     Description = "2d4 armed bandits drop from the trees, demanding toll.",                   Difficulty = "Easy"   },
                new() { Name = "Giant Spiders",     Description = "2d4 giant spiders descend from silken webs overhead.",                     Difficulty = "Medium" },
                new() { Name = "Dryad Wardens",     Description = "Fey guardians emerge to protect their sacred grove.",                      Difficulty = "Medium" },
                new() { Name = "Owlbear",           Description = "A territorial owlbear crashes through the brush, furious.",                Difficulty = "Hard"   },
                new() { Name = "Orc War Band",      Description = "2d8 orcs on a bloody raid, led by a scarred warchief.",                   Difficulty = "Hard"   },
                new() { Name = "Green Dragon",      Description = "A young green dragon surveys its forest realm — and spots the party.",     Difficulty = "Deadly" },
                new() { Name = "Treant",            Description = "An ancient treant, enraged by recent logging, rises from the earth.",      Difficulty = "Deadly" },
            },
            ["Dungeon"] = new List<EncounterEntry>
            {
                new() { Name = "Skeleton Warriors", Description = "3d4 skeletons rattle to life, swords raised.",                            Difficulty = "Easy"   },
                new() { Name = "Goblin Ambush",     Description = "2d6 goblins spring a trap — pits, ropes, and cackling.",                  Difficulty = "Easy"   },
                new() { Name = "Gelatinous Cube",   Description = "A nearly-invisible cube fills the corridor, silently engulfing all.",     Difficulty = "Medium" },
                new() { Name = "Zombie Horde",      Description = "2d8 zombies shuffle from a flooded chamber toward the party.",            Difficulty = "Medium" },
                new() { Name = "Mimic",             Description = "That innocent-looking treasure chest suddenly sprouts teeth.",             Difficulty = "Medium" },
                new() { Name = "Troll",             Description = "A regenerating troll guards the passage — fire is advised.",              Difficulty = "Hard"   },
                new() { Name = "Vampire Spawn",     Description = "A vampire spawn crawls across the ceiling, starving for blood.",          Difficulty = "Hard"   },
                new() { Name = "Beholder",          Description = "An eye tyrant floats into view, demanding immediate fealty.",             Difficulty = "Deadly" },
            },
            ["Plains"] = new List<EncounterEntry>
            {
                new() { Name = "Dire Wolves",       Description = "A pack of 2d4 dire wolves crests a hill at full sprint.",                 Difficulty = "Easy"   },
                new() { Name = "Road Bandits",      Description = "Mounted bandits block the road: 'Your coin or your life.'",               Difficulty = "Easy"   },
                new() { Name = "Gnoll Pack",        Description = "2d6 gnolls on a blood frenzy, trailing a string of corpses.",             Difficulty = "Medium" },
                new() { Name = "Giant Eagles",      Description = "A mated pair of giant eagles mistakes the party for prey.",               Difficulty = "Medium" },
                new() { Name = "Ogre Brothers",     Description = "Two squabbling ogres decide to settle their fight on the party.",         Difficulty = "Medium" },
                new() { Name = "Manticore",         Description = "A manticore swoops from the sky, tail raised and spines ready.",          Difficulty = "Hard"   },
                new() { Name = "Hill Giant",        Description = "A hill giant hurls boulders the size of bales of hay.",                   Difficulty = "Hard"   },
                new() { Name = "Wyvern",            Description = "A wyvern circles overhead before diving for the easiest target.",         Difficulty = "Deadly" },
            },
            ["Mountain"] = new List<EncounterEntry>
            {
                new() { Name = "Ice Mephits",       Description = "3d6 ice mephits pour from a frost-rimmed crevasse.",                      Difficulty = "Easy"   },
                new() { Name = "Orc Warband",       Description = "2d8 mountain orcs with a half-orc warlord descend from a pass.",          Difficulty = "Medium" },
                new() { Name = "Griffon",           Description = "A griffon shrieks from its rocky nest and attacks.",                       Difficulty = "Medium" },
                new() { Name = "Harpy Flock",       Description = "2d4 harpies sing their maddening lure from the clifftops.",               Difficulty = "Medium" },
                new() { Name = "Peryton",           Description = "A peryton stalks the party — it craves a hero's heart literally.",        Difficulty = "Hard"   },
                new() { Name = "Stone Giant",       Description = "A stone giant hurls car-sized boulders with casual menace.",              Difficulty = "Hard"   },
                new() { Name = "Roc",               Description = "A roc's shadow blots out the sun — it sees the party as hors d'oeuvres.", Difficulty = "Deadly" },
                new() { Name = "White Dragon",      Description = "A young white dragon erupts from a snowfield in a cone of frost.",        Difficulty = "Deadly" },
            },
            ["Coastal"] = new List<EncounterEntry>
            {
                new() { Name = "Crab Folk",         Description = "Territorial crabfolk defend a tide-pool cache of salvage.",               Difficulty = "Easy"   },
                new() { Name = "Pirates",           Description = "A skiff of 2d8 pirates moves to intercept, cannons hot.",                 Difficulty = "Medium" },
                new() { Name = "Merrow",            Description = "2d4 merrow surge from the surf, barnacled spears glinting.",              Difficulty = "Medium" },
                new() { Name = "Sahuagin Raiders",  Description = "A Sahuagin war party raids a fishing village — and finds the party.",     Difficulty = "Medium" },
                new() { Name = "Sea Hag Coven",     Description = "Three sea hags lure sailors to their reef with illusions of paradise.",   Difficulty = "Hard"   },
                new() { Name = "Giant Shark",       Description = "A massive shark circles the boat, bumping the hull rhythmically.",        Difficulty = "Hard"   },
                new() { Name = "Marid",             Description = "A Marid bursts from the waves, furious at some perceived insult.",        Difficulty = "Deadly" },
                new() { Name = "Bronze Dragon",     Description = "A curious young bronze dragon surfaces, eager to hear tales of war.",     Difficulty = "Deadly" },
            },
        };

        public static List<string> Environments => _tables.Keys.OrderBy(k => k).ToList();

        public static EncounterEntry GetRandom(string environment, string difficulty)
        {
            if (!_tables.TryGetValue(environment, out var pool))
                pool = _tables["Forest"];

            // Filter by difficulty (LINQ searching)
            var filtered = difficulty == "Any"
                ? pool
                : pool.Where(e => e.Difficulty == difficulty).ToList();

            if (filtered.Count == 0)
                filtered = pool;

            // Sort ascending by difficulty then pick random (Sorting + Searching technique)
            var sorted = filtered
                .OrderBy(e => DifficultyRank(e.Difficulty))
                .ToList();

            var pick = sorted[_rng.Next(sorted.Count)];
            pick.Environment = environment;
            return pick;
        }

        private static int DifficultyRank(string d) => d switch
        {
            "Easy"   => 0,
            "Medium" => 1,
            "Hard"   => 2,
            "Deadly" => 3,
            _        => 1
        };
    }
}
