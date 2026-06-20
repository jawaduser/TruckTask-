using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace tracktask.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Updates the property value and notifies the UI
        /// </summary>
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}