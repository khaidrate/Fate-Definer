using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FateDefiner.Models;
using FateDefiner.Services;

namespace FateDefiner.ViewModels
{
    /// <summary>
    /// Root ViewModel for the application shell.
    /// Owns navigation state and all child ViewModels.
    /// Inherits from BaseViewModel (Inheritance technique).
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly IDataService _data;

        private BaseViewModel? _current;
        private Campaign?      _campaign;
        private bool           _hasCampaign = false;
        private string         _statusMsg   = "Welcome to Fate Definer";

        // ── Child ViewModels (Classes technique) ─────────────────────────────────
        public CampaignDashboardViewModel  Dashboard  { get; }
        public CharacterSheetViewModel     Characters { get; }
        public FactionTrackerViewModel     Factions   { get; }
        public NotesEditorViewModel        Notes      { get; }
        public DiceRollerViewModel         Dice       { get; }
        public EncounterGeneratorViewModel Encounters { get; }

        // ── Bindable properties ───────────────────────────────────────────────────
        public BaseViewModel? CurrentViewModel
        {
            get => _current;
            set => SetProperty(ref _current, value);
        }

        public Campaign? ActiveCampaign
        {
            get => _campaign;
            private set
            {
                if (SetProperty(ref _campaign, value))
                {
                    HasCampaign = value != null;
                    OnPropertyChanged(nameof(CampaignName));

                    if (value != null)
                    {
                        Characters.LoadCampaign(value);
                        Factions.LoadCampaign(value);
                        Notes.LoadCampaign(value);
                    }
                }
            }
        }

        public string CampaignName => _campaign?.Name ?? "No Campaign Selected";

        public bool HasCampaign
        {
            get => _hasCampaign;
            private set => SetProperty(ref _hasCampaign, value);
        }

        public string StatusMessage
        {
            get => _statusMsg;
            set => SetProperty(ref _statusMsg, value);
        }

        // ── Navigation commands ───────────────────────────────────────────────────
        public ICommand GoToDashboardCommand  { get; }
        public ICommand GoToCharactersCommand { get; }
        public ICommand GoToFactionsCommand   { get; }
        public ICommand GoToNotesCommand      { get; }
        public ICommand GoToDiceCommand       { get; }
        public ICommand GoToEncountersCommand { get; }

        public MainViewModel(IDataService data)
        {
            _data = data;

            // Instantiate child ViewModels (Classes + Inheritance)
            Dashboard  = new CampaignDashboardViewModel(data);
            Characters = new CharacterSheetViewModel(data);
            Factions   = new FactionTrackerViewModel(data);
            Notes      = new NotesEditorViewModel(data);
            Dice       = new DiceRollerViewModel();
            Encounters = new EncounterGeneratorViewModel();

            // Wire up campaign-opened event
            Dashboard.CampaignOpened += OnCampaignOpened;

            GoToDashboardCommand  = new RelayCommand(_ => CurrentViewModel = Dashboard);
            GoToCharactersCommand = new RelayCommand(_ => CurrentViewModel = Characters,
                                        _ => HasCampaign);
            GoToFactionsCommand   = new RelayCommand(_ => CurrentViewModel = Factions,
                                        _ => HasCampaign);
            GoToNotesCommand      = new RelayCommand(_ => CurrentViewModel = Notes,
                                        _ => HasCampaign);
            GoToDiceCommand       = new RelayCommand(_ => CurrentViewModel = Dice);
            GoToEncountersCommand = new RelayCommand(_ => CurrentViewModel = Encounters);

            // Start on the dashboard
            CurrentViewModel = Dashboard;
        }

        private void OnCampaignOpened(Campaign c)
        {
            ActiveCampaign   = c;
            StatusMessage    = $"Campaign loaded: {c.Name}";
            CurrentViewModel = Characters;
        }

        /// <summary>
        /// Called by the auto-save timer in MainWindow every 2 minutes.
        /// Demonstrates async multithreading coordination between window and ViewModel.
        /// </summary>
        public async Task AutoSaveAsync()
        {
            if (_campaign == null) return;

            try
            {
                await _data.SaveCampaignAsync(_campaign);
                StatusMessage = $"Auto-saved — {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Auto-save failed: {ex.Message}";
            }
        }
    }
}
