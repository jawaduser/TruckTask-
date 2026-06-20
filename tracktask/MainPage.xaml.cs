using Microsoft.Maui.Controls;
using tracktask.ViewModels;

namespace tracktask.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new AdminViewModel(); 
        }
    }
}