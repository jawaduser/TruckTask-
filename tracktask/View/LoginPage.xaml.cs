using Microsoft.Maui.Controls;
using tracktask.ViewModels;

namespace tracktask.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();                   // Links XAML
            BindingContext = new LoginViewModel();   // Connects ViewModel
        }
    }
}