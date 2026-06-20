using tracktask.Models;
using tracktask.ViewModels;

namespace tracktask.View.AdminView;

public partial class AddUsers : ContentPage
{
    public AddUsers()
    {
        InitializeComponent();
    }

    private void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button &&
            button.BindingContext is User user &&
            BindingContext is AdminViewModel vm)
        {
            vm.SelectedUser = user;
        }
    }
}