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
    public class TripViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteConnection _db;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Track> Tracks { get; set; }
        public ObservableCollection<TripTask> Trips { get; set; }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        private Track _selectedTrack;
        public Track SelectedTrack
        {
            get => _selectedTrack;
            set { _selectedTrack = value; OnPropertyChanged(); }
        }

        private string _location;
        public string Location
        {
            get => _location;
            set { _location = value; OnPropertyChanged(); }
        }

        private DateTime _date = DateTime.Today;
        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        private string _product;
        public string Product
        {
            get => _product;
            set { _product = value; OnPropertyChanged(); }
        }

        public ICommand SaveTripCommand { get; }
        public ICommand DeleteTripCommand { get; }

        public TripViewModel()
        {
            _db = AppDatabase.GetDb();

            Users = new ObservableCollection<User>();
            Tracks = new ObservableCollection<Track>();
            Trips = new ObservableCollection<TripTask>();

            SaveTripCommand = new Command(SaveTrip);
            DeleteTripCommand = new Command<TripTask>(DeleteTrip);

            LoadData();
        }

        private void LoadData()
        {
            Users.Clear();
            Tracks.Clear();
            Trips.Clear();

            var users = _db.Table<User>().ToList();
            var tracks = _db.Table<Track>().ToList();
            var trips = _db.Table<TripTask>().ToList();

            foreach (var user in users)
                Users.Add(user);

            foreach (var track in tracks)
                Tracks.Add(track);

            foreach (var trip in trips)
            {
                var user = users.FirstOrDefault(u => u.UserId == trip.UserId);
                var track = tracks.FirstOrDefault(t => t.TrackId == trip.TrackId);

                if (user != null)
                    trip.UserName = user.Name;

                if (track != null)
                    trip.TrackName = track.Modules;

                Trips.Add(trip);
            }
        }

        private async void SaveTrip()
        {
            if (SelectedUser == null || SelectedTrack == null ||
                string.IsNullOrWhiteSpace(Product) ||
                string.IsNullOrWhiteSpace(Location))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Missing Data",
                    "Please select driver, track, product, and location.",
                    "OK");
                return;
            }

            var trip = new TripTask
            {
                UserId = SelectedUser.UserId,
                TrackId = SelectedTrack.TrackId,
                Product = Product,
                Location = Location,
                Date = Date
            };

            if (SelectedTrip == null)
            {
                _db.Insert(trip);
            }
            else
            {
                SelectedTrip.UserId = SelectedUser.UserId;
                SelectedTrip.TrackId = SelectedTrack.TrackId;
                SelectedTrip.Product = Product;
                SelectedTrip.Location = Location;
                SelectedTrip.Date = Date;

                _db.Update(SelectedTrip);
            }

            LoadData();
            ClearForm();



            await Application.Current.MainPage.DisplayAlert(
                "Success",
                "Trip added successfully.",
                "OK");
        }

        private TripTask _selectedTrip;
        public TripTask SelectedTrip
        {
            get => _selectedTrip;
            set
            {
                _selectedTrip = value;
                OnPropertyChanged();

                if (_selectedTrip != null)
                {
                    Product = _selectedTrip.Product;
                    Location = _selectedTrip.Location;
                    Date = _selectedTrip.Date;

                    SelectedUser = Users.FirstOrDefault(u => u.UserId == _selectedTrip.UserId);
                    SelectedTrack = Tracks.FirstOrDefault(t => t.TrackId == _selectedTrip.TrackId);
                }
            }
        }

        private async void DeleteTrip(TripTask trip)
        {
            if (trip == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                "Are you sure you want to delete this trip?",
                "Yes",
                "No");

            if (!confirm)
                return;

            _db.Delete(trip);
            LoadData();

            await Application.Current.MainPage.DisplayAlert(
                "Deleted",
                "Trip deleted successfully.",
                "OK");
        }

        private void ClearForm()
        {
            SelectedUser = null;
            SelectedTrack = null;
            Product = string.Empty;
            Location = string.Empty;
            Date = DateTime.Today;
            SelectedTrip = null;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}