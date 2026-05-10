using System;
using System.Windows;
using System.Windows.Threading;
using FateDefiner.Services;
using FateDefiner.ViewModels;

namespace FateDefiner
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel  _vm;
        private readonly DispatcherTimer _autoSaveTimer;

        public MainWindow()
        {
            InitializeComponent();

            // Dependency injection — create service and inject into ViewModel
            IDataService dataService = new DataService();
            _vm = new MainViewModel(dataService);
            DataContext = _vm;

            // Auto-save every 2 minutes using a DispatcherTimer (Multithreading technique).
            // DispatcherTimer fires on the UI thread, so async calls from here are safe.
            _autoSaveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(2)
            };
            _autoSaveTimer.Tick += async (_, _) =>
            {
                try   { await _vm.AutoSaveAsync(); }
                catch (Exception ex) { _vm.StatusMessage = $"Auto-save error: {ex.Message}"; }
            };
            _autoSaveTimer.Start();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _autoSaveTimer.Stop();
            base.OnClosing(e);
        }
    }
}
