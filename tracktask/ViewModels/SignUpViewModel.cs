using Microsoft.Maui.Controls;
using System;
using System.Windows.Input;
using tracktask.Pages;
using tracktask.Services;

namespace tracktask.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private readonly AuthService _authService;

        public ICommand SignUpCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public SignUpViewModel()
        {
            _authService = new AuthService();

            SignUpCommand = new Command(OnSignUp);
            NavigateToLoginCommand = new Command(OnNavigateToLogin);
        }

        private async void OnSignUp()
        {
            try
            {
                var success = _authService.Register(Email, Password, FullName);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Account created!", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Email already exists", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnNavigateToLogin()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}