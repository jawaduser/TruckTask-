using tracktask.Models;
using tracktask.ViewModels;

namespace tracktask.View;

public partial class TripPage : ContentPage
{
    public TripPage()
    {
        InitializeComponent();
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is TripTask trip &&
            BindingContext is TripViewModel vm)
        {
            vm.SelectedTrip = trip;
        }
    }
}