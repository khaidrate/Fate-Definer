using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FateDefiner.Models;
using FateDefiner.Services;

namespace FateDefiner.ViewModels
{
    /// <summary>
    /// ViewModel for the Session Notes editor.
    /// Demonstrates LINQ-based keyword search across note title, content, and tags.
    /// </summary>
    public class NotesEditorViewModel : BaseViewModel
    {
        private readonly IDataService _data;
        private Campaign? _campaign;

        private Note?  _selected;
        private string _search    = string.Empty;
        private string _statusMsg = string.Empty;

        public ObservableCollection<Note> Notes { get; } = new();

        public Note? SelectedNote
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public string SearchText
        {
            get => _search;
            set { if (SetProperty(ref _search, value)) ApplySearch(); }
        }

        public string StatusMessage
        {
            get => _statusMsg;
            set => SetProperty(ref _statusMsg, value);
        }

        public ICommand AddNoteCommand    { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand SaveCommand       { get; }
        public ICommand ClearSearchCommand{ get; }

        public NotesEditorViewModel(IDataService data)
        {
            _data = data;

            AddNoteCommand     = new RelayCommand(_ => AddNote());
            DeleteNoteCommand  = new RelayCommand(_ => DeleteNote(), _ => _selected != null);
            SaveCommand        = new RelayCommand(async _ => await SaveAsync(), _ => _campaign != null);
            ClearSearchCommand = new RelayCommand(_ => SearchText = string.Empty);
        }

        public void LoadCampaign(Campaign campaign)
        {
            _campaign = campaign;
            ApplySearch();
            SelectedNote = Notes.FirstOrDefault();
            StatusMessage = $"{campaign.SessionNotes.Count} session note(s)";
        }

        /// <summary>
        /// Full-text keyword search across title, content, and tags using LINQ.
        /// Demonstrates Searching technique.
        /// </summary>
        private void ApplySearch()
        {
            if (_campaign == null) return;

            IEnumerable<Note> query = _campaign.SessionNotes;

            if (!string.IsNullOrWhiteSpace(_search))
            {
                string term = _search.Trim();
                // LINQ Where with multiple OR conditions — Searching technique
                query = query.Where(n =>
                    n.Title.Contains(term, StringComparison.OrdinalIgnoreCase)   ||
                    n.Content.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    n.Tags.Any(t => t.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }

            // Sort by session date descending — Sorting technique
            var sorted = query.OrderByDescending(n => n.SessionDate).ToList();

            Notes.Clear();
            foreach (var n in sorted)  // Loop
                Notes.Add(n);

            StatusMessage = string.IsNullOrWhiteSpace(_search)
                ? $"{Notes.Count} note(s)"
                : $"{Notes.Count} result(s) for \"{_search}\"";

            if (_selected != null && !Notes.Contains(_selected))
                SelectedNote = Notes.FirstOrDefault();
        }

        private void AddNote()
        {
            if (_campaign == null) return;

            var note = new Note
            {
                Title       = $"Session {_campaign.SessionNotes.Count + 1}",
                Content     = string.Empty,
                SessionDate = DateTime.Now
            };

            _campaign.SessionNotes.Add(note);
            Notes.Insert(0, note);
            SelectedNote  = note;
            StatusMessage = $"{_campaign.SessionNotes.Count} note(s)";
        }

        private void DeleteNote()
        {
            if (_selected == null || _campaign == null) return;

            _campaign.SessionNotes.Remove(_selected);
            Notes.Remove(_selected);
            SelectedNote  = Notes.FirstOrDefault();
            StatusMessage = $"{_campaign.SessionNotes.Count} note(s)";
        }

        private async Task SaveAsync()
        {
            if (_campaign == null) return;

            if (_selected != null)
                _selected.LastModified = DateTime.Now;

            try
            {
                await _data.SaveCampaignAsync(_campaign);
                StatusMessage = $"Saved — {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Save failed: {ex.Message}";
            }
        }
    }
}
