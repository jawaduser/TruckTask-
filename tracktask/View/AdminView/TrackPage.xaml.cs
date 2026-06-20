using tracktask.Models;
using tracktask.ViewModels;

namespace tracktask.View;

public partial class TrackPage : ContentPage
{
    public TrackPage()
    {
        InitializeComponent();
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is Track track &&
            BindingContext is TrackViewModel vm)
        {
            vm.SelectedTrack = track;
        }
    }
}