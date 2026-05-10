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
    /// ViewModel for the Faction &amp; NPC tracker screen.
    /// </summary>
    public class FactionTrackerViewModel : BaseViewModel
    {
        private readonly IDataService _data;
        private Campaign? _campaign;

        private Faction? _selectedFaction;
        private NPC?     _selectedNPC;
        private string   _statusMsg = string.Empty;

        public ObservableCollection<Faction> Factions { get; } = new();
        public ObservableCollection<NPC>     NPCs     { get; } = new();

        public Faction? SelectedFaction
        {
            get => _selectedFaction;
            set
            {
                if (SetProperty(ref _selectedFaction, value))
                {
                    OnPropertyChanged(nameof(HasFaction));
                    RefreshNPCs();
                }
            }
        }

        public NPC? SelectedNPC
        {
            get => _selectedNPC;
            set => SetProperty(ref _selectedNPC, value);
        }

        public string StatusMessage
        {
            get => _statusMsg;
            set => SetProperty(ref _statusMsg, value);
        }

        public bool HasFaction => _selectedFaction != null;

        public ICommand AddFactionCommand    { get; }
        public ICommand DeleteFactionCommand { get; }
        public ICommand AddNPCCommand        { get; }
        public ICommand DeleteNPCCommand     { get; }
        public ICommand SaveCommand          { get; }

        public FactionTrackerViewModel(IDataService data)
        {
            _data = data;

            AddFactionCommand    = new RelayCommand(_ => AddFaction());
            DeleteFactionCommand = new RelayCommand(_ => DeleteFaction(), _ => _selectedFaction != null);
            AddNPCCommand        = new RelayCommand(_ => AddNPC(),        _ => _selectedFaction != null);
            DeleteNPCCommand     = new RelayCommand(_ => DeleteNPC(),     _ => _selectedNPC != null);
            SaveCommand          = new RelayCommand(async _ => await SaveAsync(), _ => _campaign != null);
        }

        public void LoadCampaign(Campaign campaign)
        {
            _campaign = campaign;
            Factions.Clear();

            // LINQ OrderBy — Sorting technique
            var sorted = campaign.Factions.OrderBy(f => f.Name).ToList();
            foreach (var f in sorted)
                Factions.Add(f);

            SelectedFaction = Factions.FirstOrDefault();
            StatusMessage = $"{campaign.Factions.Count} faction(s)";
        }

        private void RefreshNPCs()
        {
            NPCs.Clear();
            if (_selectedFaction == null) return;

            // Loop through NPC list — Loop technique
            foreach (var npc in _selectedFaction.Members)
                NPCs.Add(npc);
        }

        private void AddFaction()
        {
            if (_campaign == null) return;

            var f = new Faction
            {
                Name      = "New Faction",
                Alignment = "Neutral",
                Goals     = "Unknown"
            };

            _campaign.Factions.Add(f);
            Factions.Add(f);
            SelectedFaction = f;
            StatusMessage = $"{_campaign.Factions.Count} faction(s)";
        }

        private void DeleteFaction()
        {
            if (_selectedFaction == null || _campaign == null) return;

            _campaign.Factions.Remove(_selectedFaction);
            Factions.Remove(_selectedFaction);
            SelectedFaction = Factions.FirstOrDefault();
            StatusMessage = $"{_campaign.Factions.Count} faction(s)";
        }

        private void AddNPC()
        {
            if (_selectedFaction == null) return;

            var npc = new NPC { Name = "New NPC", Role = "Member" };
            _selectedFaction.Members.Add(npc);
            NPCs.Add(npc);
            SelectedNPC = npc;
        }

        private void DeleteNPC()
        {
            if (_selectedNPC == null || _selectedFaction == null) return;

            _selectedFaction.Members.Remove(_selectedNPC);
            NPCs.Remove(_selectedNPC);
            SelectedNPC = null;
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
