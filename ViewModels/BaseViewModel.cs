using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FateDefiner.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels.
    /// Provides INotifyPropertyChanged and a SetProperty helper.
    /// Inheritance technique: all ViewModels inherit from this class.
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Sets a backing field and fires PropertyChanged only when the value has changed.
        /// Returns true if the value changed.
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value,
            [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
