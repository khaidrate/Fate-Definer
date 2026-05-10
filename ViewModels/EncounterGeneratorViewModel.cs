using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using FateDefiner.Models;

namespace FateDefiner.ViewModels
{
    /// <summary>
    /// ViewModel for the Random Encounter &amp; Loot Generator screen.
    /// </summary>
    public class EncounterGeneratorViewModel : BaseViewModel
    {
        private string _environment = "Forest";
        private string _difficulty  = "Any";
        private string _lootTier    = "Medium";
        private int    _partyLevel  = 5;

        private EncounterEntry? _encounter;
        private string          _encounterText = "Press Generate to create an encounter.";
        private bool            _hasEncounter  = false;
        private bool            _hasLoot       = false;

        // ── Selector options ─────────────────────────────────────────────────────
        public ObservableCollection<string> Environments { get; }
        public ObservableCollection<string> Difficulties { get; } = new(new[]
            { "Any", "Easy", "Medium", "Hard", "Deadly" });
        public ObservableCollection<string> LootTiers { get; } = new(new[]
            { "Low", "Medium", "High", "Legendary" });

        // ── Properties ────────────────────────────────────────────────────────────
        public string SelectedEnvironment
        {
            get => _environment;
            set => SetProperty(ref _environment, value);
        }
        public string SelectedDifficulty
        {
            get => _difficulty;
            set => SetProperty(ref _difficulty, value);
        }
        public string SelectedLootTier
        {
            get => _lootTier;
            set => SetProperty(ref _lootTier, value);
        }
        public int PartyLevel
        {
            get => _partyLevel;
            set => SetProperty(ref _partyLevel, Math.Clamp(value, 1, 20));
        }
        public EncounterEntry? CurrentEncounter
        {
            get => _encounter;
            set => SetProperty(ref _encounter, value);
        }
        public string EncounterText
        {
            get => _encounterText;
            set => SetProperty(ref _encounterText, value);
        }
        public bool HasEncounter
        {
            get => _hasEncounter;
            set => SetProperty(ref _hasEncounter, value);
        }
        public bool HasLoot
        {
            get => _hasLoot;
            set => SetProperty(ref _hasLoot, value);
        }
        public ObservableCollection<LootEntry> Loot { get; } = new();

        // ── Commands ─────────────────────────────────────────────────────────────
        public ICommand GenerateEncounterCommand { get; }
        public ICommand GenerateLootCommand      { get; }

        public EncounterGeneratorViewModel()
        {
            Environments = new ObservableCollection<string>(EncounterTable.Environments);

            GenerateEncounterCommand = new RelayCommand(_ => GenerateEncounter());
            GenerateLootCommand      = new RelayCommand(_ => GenerateLoot());
        }

        private void GenerateEncounter()
        {
            try
            {
                var enc = EncounterTable.GetRandom(_environment, _difficulty);
                CurrentEncounter = enc;
                EncounterText    = $"{enc.Name}\n\n{enc.Description}";
                HasEncounter     = true;
            }
            catch (Exception ex)
            {
                EncounterText = $"Error: {ex.Message}";
            }
        }

        private void GenerateLoot()
        {
            try
            {
                var items = LootTable.Generate(_lootTier, _partyLevel);
                Loot.Clear();
                foreach (var item in items)
                    Loot.Add(item);
                HasLoot = true;
            }
            catch (Exception ex)
            {
                EncounterText = $"Loot error: {ex.Message}";
            }
        }
    }
}
