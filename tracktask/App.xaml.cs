using tracktask.Data;

namespace tracktask;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
 
        try
        {
            AppDatabase.InitializeDatabase();

            string dbPath = AppDatabase.GetDbPath();
            System.Diagnostics.Debug.WriteLine($"Database path: {dbPath}");

            bool exists = File.Exists(dbPath);
            System.Diagnostics.Debug.WriteLine($"Database created: {exists}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}