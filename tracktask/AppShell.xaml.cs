using tracktask.Pages;
using tracktask.View;
using tracktask.View.AdminView;
using tracktask.View.DriverView;

namespace tracktask
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
        }

        public void LoadAdminShell()
        {
            Items.Clear();
            FlyoutBehavior = FlyoutBehavior.Flyout;

            Items.Add(new FlyoutItem
            {
                Title = "Admin",
                FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
                Items =
                {
                    new ShellContent
                    {
                        Route = "AddUsers",
                        Title = "Add Users",
                        ContentTemplate = new DataTemplate(typeof(AddUsers))
                    },
                    new ShellContent
                    {
                        Route = "TrackPage",
                        Title = "Tracks",
                        ContentTemplate = new DataTemplate(typeof(TrackPage))
                    },
                    new ShellContent
                    {
                        Route = "TripPage",
                        Title = "Trips",
                        ContentTemplate = new DataTemplate(typeof(TripPage))
                    },
                    new ShellContent
                    {
                        Route = "RequestPage",
                        Title = "Requests",
                        ContentTemplate = new DataTemplate(typeof(Requsetpage))
                    }
                }
            });

            AddLogoutMenuItem();
        }

        public void LoadDriverShell()
        {
            Items.Clear();
            FlyoutBehavior = FlyoutBehavior.Flyout;

            Items.Add(new FlyoutItem
            {
                Title = "Driver",
                FlyoutDisplayOptions = FlyoutDisplayOptions.AsMultipleItems,
                Items =
                {
                    new ShellContent
                    {
                        Route = "TripsPage",
                        Title = "My Trips",
                        ContentTemplate = new DataTemplate(typeof(TripsPage))
                    },
                    new ShellContent
                    {
                        Route = "DriverRequestsPage",
                        Title = "Requests",
                        ContentTemplate = new DataTemplate(typeof(DriverRequestsPage))
                    }
                }
            });

            AddLogoutMenuItem();
        }

        private void AddLogoutMenuItem()
        {
            Items.Add(new MenuItem
            {
                Text = "🚪 Logout",
                Command = new Command(async () =>
                {
                    bool confirm = await Application.Current.MainPage.DisplayAlert(
                        "Logout",
                        "Are you sure you want to logout?",
                        "Yes",
                        "No");

                    if (confirm)
                    {
                        Application.Current.Windows[0].Page = new AppShell();
                    }
                })
            });
        }
    }
}