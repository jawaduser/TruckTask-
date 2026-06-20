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
    public class DriverTripsViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteConnection _db;

        public ObservableCollection<TripTask> Trips { get; set; }

        public ICommand MarkAsDoneCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DriverTripsViewModel()
        {
            _db = AppDatabase.GetDb();

            Trips = new ObservableCollection<TripTask>();

            MarkAsDoneCommand = new Command<TripTask>(MarkAsDone);

            LoadTrips();
        }

        private void LoadTrips()
        {
            Trips.Clear();

            var tracks = _db.Table<Track>().ToList();
            var trips = _db.Table<TripTask>().ToList();

            foreach (var trip in trips)
            {
                var track = tracks.FirstOrDefault(t => t.TrackId == trip.TrackId);

                if (track != null)
                    trip.TrackName = track.Modules;

                Trips.Add(trip);
            }
        }

        private async void MarkAsDone(TripTask trip)
        {
            if (trip == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Complete Trip",
                $"Mark trip '{trip.Product}' as done?",
                "Yes",
                "No");

            if (!confirm)
                return;

            trip.IsDone = true;
            _db.Update(trip);

            LoadTrips();

            await Application.Current.MainPage.DisplayAlert(
                "Done",
                "Trip marked as done successfully.",
                "OK");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}