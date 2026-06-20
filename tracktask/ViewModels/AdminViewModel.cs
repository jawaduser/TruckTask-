using Microsoft.Maui.Controls;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using tracktask.Models;
using tracktask.Service;
using tracktask.Services;

namespace tracktask.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private UserRole _selectedRole;
        public UserRole SelectedRole
        {
            get => _selectedRole;
            set { _selectedRole = value; OnPropertyChanged(); }
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();

                if (_selectedUser != null)
                {
                    Name = _selectedUser.Name;
                    Email = _selectedUser.Email;
                    Password = string.Empty;
                    SelectedRole = _selectedUser.Role;
                }
            }
        }

       

        public ObservableCollection<User> Users { get; set; }
        public List<UserRole> Roles { get; }

        public ICommand SaveUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        

        public AdminViewModel()
        {
            _userService = new UserService();
            _authService = new AuthService();

            Users = new ObservableCollection<User>();
            Roles = Enum.GetValues(typeof(UserRole)).Cast<UserRole>().ToList();

            SaveUserCommand = new Command(SaveUser);
        
            DeleteUserCommand = new Command<User>(DeleteUser);
        
            SelectedRole = Roles.FirstOrDefault();
            LoadUsers();
        }

        private void SaveUser()
        {
            if (SelectedUser == null)
                InsertUser();
            else
                UpdateUser();
        }

        private async void InsertUser()
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Missing Data",
                    "Please fill name, email, and password.",
                    "OK");
                return;
            }

            string normalizedEmail = Email.Trim().ToLower();

            bool success = _authService.Register(normalizedEmail, Password, Name);

            if (!success)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "This email already exists.",
                    "OK");
                return;
            }

            var createdUser = _userService.GetAll()
                .FirstOrDefault(u => u.Email == normalizedEmail);

            if (createdUser != null)
            {
                createdUser.Role = SelectedRole;
                _userService.Update(createdUser);

                 LoadUsers(); 
                ClearForm();

                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "User added successfully.",
                    "OK");
            }
        }

        private async void UpdateUser()
        {
            if (SelectedUser == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "No Selection",
                    "Please click Edit on a user first.",
                    "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Missing Data",
                    "Name and email are required.",
                    "OK");
                return;
            }

            try
            {
                SelectedUser.Name = Name;
                SelectedUser.Email = Email.Trim().ToLower();
                SelectedUser.Role = SelectedRole;

                if (!string.IsNullOrWhiteSpace(Password))
                {
                    SelectedUser.PasswordHash = HashPassword(Password);
                }

                _userService.Update(SelectedUser);

                LoadUsers();
                ClearForm();

                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "User updated successfully.",
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

        private async void DeleteUser(User user)
        {
            if (user == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete '{user.Name}'?",
                "Yes",
                "No");

            if (!confirm)
                return;

            try
            {
                _userService.Delete(user);

                if (SelectedUser != null && SelectedUser.UserId == user.UserId)
                    ClearForm();

                LoadUsers();

                await Application.Current.MainPage.DisplayAlert(
                    "Deleted",
                    "User deleted successfully.",
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

        
        

        private void ClearForm()
        {
            Name = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            SelectedRole = Roles.FirstOrDefault();
            SelectedUser = null;
        }

       

        private void LoadUsers()
        {
            Users.Clear();

            var allUsers = _userService.GetAll();
            foreach (var user in allUsers)
            {
                Users.Add(user);
            }
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}