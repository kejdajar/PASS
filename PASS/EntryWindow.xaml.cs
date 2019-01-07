using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PASS.Storage;

using PASS.GeneralClasses;
using System.Threading;

namespace PASS
{
    /// <summary>
    /// Okno, které je vstupním bodem programu.
    /// </summary>
    public partial class EntryWindow : Window
    {
        public static bool OnlyOnce { get; set; } = false;

        public EntryWindow()
        {
            InitializeComponent();
            
           

            if (!OnlyOnce) // Provede se pouze jednou
            {
                GlobalSettings.ApplyGlobalSettings();
            }

            OnlyOnce = true;
        }

        
      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (!Properties.Settings.Default.IsFirstRun)
            {
                // Otevření přihlašovacího okna
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Owner = this;
                loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                loginWindow.ShowDialog();

                if (loginWindow.IsAuthentificated)
                {
                    MainWindow mainWindow = new MainWindow();
                    this.Hide();
                    mainWindow.Show();
                    this.Owner = mainWindow;
                }
            }
            else // první spuštění, welcome okno
            {
                WelcomeWindow welcomeWindow = new WelcomeWindow();
                welcomeWindow.Owner = this;
                welcomeWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                welcomeWindow.ShowDialog();

                if (welcomeWindow.Success)
                {
                    // Otevření přihlašovacího okna
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Owner = this;
                    loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    loginWindow.ShowDialog();

                    if (loginWindow.IsAuthentificated)
                    {
                        MainWindow mainWindow = new MainWindow();
                        this.Hide();
                        mainWindow.Show();
                        this.Owner = mainWindow;
                    }
                }
            }

        }

    }
}
