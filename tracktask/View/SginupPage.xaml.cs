using Microsoft.Maui.Controls;
using tracktask.ViewModels;

namespace tracktask.Pages
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
            BindingContext = new SignUpViewModel();
        }
    }
}