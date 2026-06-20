using tracktask;
using Microsoft.Maui.Controls;
using System;
using System.Windows.Input;
using tracktask.Models;
using tracktask.Pages;
using tracktask.Services;

namespace tracktask.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
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

        public ICommand LoginCommand { get; }
        public ICommand NavigateToSignUpCommand { get; }

        public LoginViewModel()
        {
            _authService = new AuthService();

            LoginCommand = new Command(OnLogin);
            NavigateToSignUpCommand = new Command(OnNavigateToSignUp);
        }

        private async void OnLogin()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter email and password", "OK");
                return;
            }

            try
            {
                var user = _authService.Login(Email, Password);

                if (user != null)
                {

                    if (Shell.Current is AppShell shell)
                    {
                        if (user.Role == UserRole.Admin)
                        {
                            shell.LoadAdminShell();
                        }
                        else
                        {
                            shell.LoadDriverShell();
                        }

                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Invalid email or password", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnNavigateToSignUp()
        {
            await Shell.Current.GoToAsync(nameof(SignUpPage));
        }
    }
}