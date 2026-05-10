using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FateDefiner.Models;
using FateDefiner.Services;

namespace FateDefiner.ViewModels
{
    /// <summary>
    /// ViewModel for the Character Sheet screen.
    /// Demonstrates: Classes, Inheritance, LINQ (search/sort), Loops, Exception Handling.
    /// </summary>
    public class CharacterSheetViewModel : BaseViewModel
    {
        private readonly IDataService _data;
        private Campaign? _campaign;

        private Character?     _selected;
        private InventoryItem? _selectedItem;
        private string         _searchText  = string.Empty;
        private string         _newItemName = string.Empty;
        private string         _statusMsg   = string.Empty;

        // ── Public collections & properties ──────────────────────────────────────

        public ObservableCollection<Character>     Characters { get; } = new();
        public ObservableCollection<InventoryItem> Inventory  { get; } = new();

        public Character? SelectedCharacter
        {
            get => _selected;
            set
            {
                if (SetProperty(ref _selected, value))
                {
                    OnPropertyChanged(nameof(HasSelection));
                    RefreshInventory();
                }
            }
        }

        public InventoryItem? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) ApplyFilter(); }
        }

        public string NewItemName
        {
            get => _newItemName;
            set => SetProperty(ref _newItemName, value);
        }

        public string StatusMessage
        {
            get => _statusMsg;
            set => SetProperty(ref _statusMsg, value);
        }

        public bool HasSelection => _selected != null;

        // ── Commands ─────────────────────────────────────────────────────────────

        public ICommand AddCharacterCommand    { get; }
        public ICommand DeleteCharacterCommand { get; }
        public ICommand AddItemCommand         { get; }
        public ICommand RemoveItemCommand      { get; }
        public ICommand SaveCommand            { get; }

        // ── Constructor ───────────────────────────────────────────────────────────

        public CharacterSheetViewModel(IDataService data)
        {
            _data = data;

            AddCharacterCommand    = new RelayCommand(_ => AddCharacter());
            DeleteCharacterCommand = new RelayCommand(_ => DeleteCharacter(),
                                         _ => _selected != null);
            AddItemCommand         = new RelayCommand(_ => AddItem(),
                                         _ => _selected != null && !string.IsNullOrWhiteSpace(_newItemName));
            RemoveItemCommand      = new RelayCommand(_ => RemoveItem(),
                                         _ => _selectedItem != null);
            SaveCommand            = new RelayCommand(async _ => await SaveAsync(),
                                         _ => _campaign != null);
        }

        // ── Public API ────────────────────────────────────────────────────────────

        public void LoadCampaign(Campaign campaign)
        {
            _campaign = campaign;
            ApplyFilter();
            SelectedCharacter = Characters.FirstOrDefault();
            StatusMessage = $"{campaign.Characters.Count} character(s)";
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        /// <summary>Filters and sorts the character list using LINQ.</summary>
        private void ApplyFilter()
        {
            if (_campaign == null) return;

            // LINQ: Where (search) + OrderBy (sort)
            var query = string.IsNullOrWhiteSpace(_searchText)
                ? _campaign.Characters.AsEnumerable()
                : _campaign.Characters.Where(c =>
                    c.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                    c.Race.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                    c.Class.Contains(_searchText, StringComparison.OrdinalIgnoreCase));

            var sorted = query.OrderBy(c => c.Name).ToList();

            Characters.Clear();
            foreach (var c in sorted)   // Loop technique
                Characters.Add(c);

            if (_selected != null && !Characters.Contains(_selected))
                SelectedCharacter = Characters.FirstOrDefault();
        }

        private void RefreshInventory()
        {
            Inventory.Clear();
            if (_selected == null) return;

            foreach (var item in _selected.Inventory)  // Loop
                Inventory.Add(item);
        }

        private void AddCharacter()
        {
            if (_campaign == null) return;

            var c = new Character { Name = "New Character" };
            _campaign.Characters.Add(c);
            Characters.Add(c);
            SelectedCharacter = c;
            StatusMessage = $"{_campaign.Characters.Count} character(s)";
        }

        private void DeleteCharacter()
        {
            if (_selected == null || _campaign == null) return;

            _campaign.Characters.Remove(_selected);
            Characters.Remove(_selected);
            SelectedCharacter = Characters.FirstOrDefault();
            StatusMessage = $"{_campaign.Characters.Count} character(s)";
        }

        private void AddItem()
        {
            if (_selected == null || string.IsNullOrWhiteSpace(_newItemName)) return;

            var item = new InventoryItem { Name = _newItemName.Trim() };
            _selected.Inventory.Add(item);
            Inventory.Add(item);
            NewItemName = string.Empty;
        }

        private void RemoveItem()
        {
            if (_selectedItem == null || _selected == null) return;

            _selected.Inventory.Remove(_selectedItem);
            Inventory.Remove(_selectedItem);
            SelectedItem = null;
        }

        private async Task SaveAsync()
        {
            if (_campaign == null) return;

            try
            {
                await _data.SaveCampaignAsync(_campaign);
                StatusMessage = $"Saved — {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
