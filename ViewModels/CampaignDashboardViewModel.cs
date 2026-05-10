using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FateDefiner.Models;
using FateDefiner.Services;

namespace FateDefiner.ViewModels
{
    /// <summary>
    /// ViewModel for the Campaign Dashboard screen.
    /// Manages CRUD operations on campaigns.  Uses async/await (multithreading technique).
    /// </summary>
    public class CampaignDashboardViewModel : BaseViewModel
    {
        private readonly IDataService _data;

        private Campaign? _selected;
        private string    _newName       = string.Empty;
        private bool      _isLoading     = false;
        private string    _statusMessage = "Loading campaigns…";

        // ── Public properties ────────────────────────────────────────────────────

        public ObservableCollection<Campaign> Campaigns { get; } = new();

        public Campaign? SelectedCampaign
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public string NewCampaignName
        {
            get => _newName;
            set => SetProperty(ref _newName, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // ── Event raised when the user opens a campaign ──────────────────────────
        public event Action<Campaign>? CampaignOpened;

        // ── Commands ─────────────────────────────────────────────────────────────
        public ICommand OpenCampaignCommand   { get; }
        public ICommand NewCampaignCommand    { get; }
        public ICommand SaveCampaignCommand   { get; }
        public ICommand DeleteCampaignCommand { get; }
        public ICommand RefreshCommand        { get; }

        public CampaignDashboardViewModel(IDataService data)
        {
            _data = data;

            OpenCampaignCommand   = new RelayCommand(_ => OpenCampaign(),           _ => _selected != null);
            NewCampaignCommand    = new RelayCommand(_ => CreateCampaign(),         _ => !string.IsNullOrWhiteSpace(_newName));
            SaveCampaignCommand   = new RelayCommand(async _ => await SaveAsync(),  _ => _selected != null);
            DeleteCampaignCommand = new RelayCommand(async _ => await DeleteAsync(),_ => _selected != null);
            RefreshCommand        = new RelayCommand(async _ => await LoadAsync());

            // Kick off async load — fire-and-forget is fine here because errors are caught inside
            _ = LoadAsync();
        }

        // ── Methods ───────────────────────────────────────────────────────────────

        private void OpenCampaign()
        {
            if (_selected != null)
                CampaignOpened?.Invoke(_selected);
        }

        private void CreateCampaign()
        {
            if (string.IsNullOrWhiteSpace(_newName)) return;

            var campaign = new Campaign
            {
                Name        = NewCampaignName.Trim(),
                Description = "A new adventure awaits…",
                Setting     = "Unknown Realm"
            };

            Campaigns.Insert(0, campaign);
            SelectedCampaign = campaign;
            NewCampaignName  = string.Empty;

            _ = SaveAsync(campaign);
        }

        private async Task SaveAsync(Campaign? c = null)
        {
            var target = c ?? _selected;
            if (target == null) return;

            try
            {
                await _data.SaveCampaignAsync(target);
                StatusMessage = $"Saved \"{target.Name}\" — {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save failed: {ex.Message}";
            }
        }

        private async Task DeleteAsync()
        {
            if (_selected == null) return;

            var result = MessageBox.Show(
                $"Delete \"{_selected.Name}\"? This cannot be undone.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _data.DeleteCampaignAsync(_selected.Id);
                Campaigns.Remove(_selected);
                SelectedCampaign = null;
                StatusMessage = "Campaign deleted.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Delete failed: {ex.Message}";
            }
        }

        /// <summary>Loads all campaigns from disk on a background thread (Multithreading).</summary>
        public async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var list = await _data.LoadAllCampaignsAsync();

                // Update ObservableCollection on UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Campaigns.Clear();
                    foreach (var c in list)
                        Campaigns.Add(c);

                    StatusMessage = list.Count == 0
                        ? "No campaigns found — create one above!"
                        : $"{list.Count} campaign(s) loaded.";
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
