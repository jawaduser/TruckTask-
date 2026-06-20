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
    public class AdminRequestsViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteConnection _db;

        public ObservableCollection<Request> Requests { get; set; }

        public ICommand ApproveRequestCommand { get; }
        public ICommand RejectRequestCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AdminRequestsViewModel()
        {
            _db = AppDatabase.GetDb();

            Requests = new ObservableCollection<Request>();

            ApproveRequestCommand = new Command<Request>(ApproveRequest);
            RejectRequestCommand = new Command<Request>(RejectRequest);

            LoadRequests();
        }

        private void LoadRequests()
        {
            Requests.Clear();

            var allRequests = _db.Table<Request>().ToList();

            foreach (var request in allRequests)
            {
                Requests.Add(request);
            }
        }

        private async void ApproveRequest(Request request)
        {
            if (request == null)
                return;

            request.Status = "Approved";
            _db.Update(request);

            LoadRequests();

            await Application.Current.MainPage.DisplayAlert(
                "Approved",
                "Request approved successfully.",
                "OK");
        }

        private async void RejectRequest(Request request)
        {
            if (request == null)
                return;

            request.Status = "Rejected";
            _db.Update(request);

            LoadRequests();

            await Application.Current.MainPage.DisplayAlert(
                "Rejected",
                "Request rejected successfully.",
                "OK");
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}