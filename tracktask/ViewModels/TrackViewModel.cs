using Microsoft.Maui.Controls;
using SQLite;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using tracktask.Data;
using tracktask.Models;

namespace tracktask.ViewModels
{
    public class TrackViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteConnection _db;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _modules;
        public string Modules
        {
            get => _modules;
            set { _modules = value; OnPropertyChanged(); }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private bool _availability;
        public bool Availability
        {
            get => _availability;
            set { _availability = value; OnPropertyChanged(); }
        }

        private Track _selectedTrack;
        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged();

                if (_selectedTrack != null)
                {
                    Modules = _selectedTrack.Modules;
                    Status = _selectedTrack.Status;
                    Availability = _selectedTrack.Availability;
                }
            }
        }

        public ObservableCollection<Track> Tracks { get; set; }

        public ICommand SaveTrackCommand { get; }
        public ICommand LoadTracksCommand { get; }
        public ICommand DeleteTrackCommand { get; }
        public ICommand CancelEditCommand { get; }

        public TrackViewModel()
        {
            _db = AppDatabase.GetDb();

            Tracks = new ObservableCollection<Track>();

            SaveTrackCommand = new Command(SaveTrack);
            LoadTracksCommand = new Command(LoadTracks);
            DeleteTrackCommand = new Command<Track>(DeleteTrack);
            CancelEditCommand = new Command(CancelEdit);

            LoadTracks();
        }

        private void SaveTrack()
        {
            if (SelectedTrack == null)
                AddTrack();
            else
                UpdateTrack();
        }

        private async void AddTrack()
        {
            if (string.IsNullOrWhiteSpace(Modules) ||
                string.IsNullOrWhiteSpace(Status))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Missing Data",
                    "Please fill modules and status.",
                    "OK");
                return;
            }

            try
            {
                var track = new Track
                {
                    Modules = Modules,
                    Status = Status,
                    Availability = Availability
                };

                _db.Insert(track);
                LoadTracks();
                ClearForm();

                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Track added successfully.",
                    "OK");
            }
            catch (SQLiteException ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Database Error",
                    ex.Message,
                    "OK");
            }
        }

        private async void UpdateTrack()
        {
            if (SelectedTrack == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "No Selection",
                    "Please click Edit on a track first.",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Modules) ||
                string.IsNullOrWhiteSpace(Status))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Missing Data",
                    "Please fill modules and status.",
                    "OK");
                return;
            }

            try
            {
                SelectedTrack.Modules = Modules;
                SelectedTrack.Status = Status;
                SelectedTrack.Availability = Availability;

                _db.Update(SelectedTrack);
                LoadTracks();
                ClearForm();

                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Track updated successfully.",
                    "OK");
            }
            catch (SQLiteException ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Database Error",
                    ex.Message,
                    "OK");
            }
        }

        private async void DeleteTrack(Track track)
        {
            if (track == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete track '{track.Modules}'?",
                "Yes",
                "No");

            if (!confirm)
                return;

            try
            {
                _db.Delete(track);
                LoadTracks();

                if (SelectedTrack != null && SelectedTrack.TrackId == track.TrackId)
                    ClearForm();

                await Application.Current.MainPage.DisplayAlert(
                    "Deleted",
                    "Track deleted successfully.",
                    "OK");
            }
            catch (SQLiteException ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Database Error",
                    ex.Message,
                    "OK");
            }
        }

        private void CancelEdit()
        {
            ClearForm();
        }

        private void ClearForm()
        {
            Modules = string.Empty;
            Status = string.Empty;
            Availability = false;
            SelectedTrack = null;
        }
       

        private void LoadTracks()
        {
            Tracks.Clear();

            var allTracks = _db.Table<Track>().ToList();

            foreach (var track in allTracks)
            {
                Tracks.Add(track);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}