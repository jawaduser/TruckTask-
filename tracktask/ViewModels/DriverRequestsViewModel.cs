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
    public class DriverRequestsViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteConnection _db;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TripTask> Trips { get; set; }
        public ObservableCollection<Request> Requests { get; set; }

        private TripTask _selectedTrip;
        public TripTask SelectedTrip
        {
            get => _selectedTrip;
            set { _selectedTrip = value; OnPropertyChanged(); }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        public ICommand SendRequestCommand { get; }

        public DriverRequestsViewModel()
        {
            _db = AppDatabase.GetDb();

            Trips = new ObservableCollection<TripTask>();
            Requests = new ObservableCollection<Request>();

            SendRequestCommand = new Command(SendRequest);

            LoadData();
        }

        private void LoadData()
        {
            Trips.Clear();
            Requests.Clear();

            var trips = _db.Table<TripTask>().ToList();
            var requests = _db.Table<Request>().ToList();

            foreach (var trip in trips)
                Trips.Add(trip);

            foreach (var request in requests)
                Requests.Add(request);
        }

        private async void SendRequest()
        {
            if (SelectedTrip == null ||
                string.IsNullOrWhiteSpace(Message))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Missing Data",
                    "Please select a trip and write a message.",
                    "OK");

                return;
            }

            var request = new Request
            {
                TripId = SelectedTrip.TripId,
                DriverName = SelectedTrip.UserName,
                Message = Message,
                Status = "Pending"
            };

            _db.Insert(request);

            LoadData();

            Message = string.Empty;
            SelectedTrip = null;

            await Application.Current.MainPage.DisplayAlert(
                "Success",
                "Request sent successfully.",
                "OK");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}