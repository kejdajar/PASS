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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PASS.GeneralClasses;

namespace PASS
{
    /// <summary>
    /// Interaction logic for MenuPanelPage.xaml
    /// </summary>
    public partial class MenuPanelPage : Page
    {
        public MenuPanelPage()
        {
            InitializeComponent();
            InitializeInterface();
        }

        // Časovač pro hodiny
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();

        private void InitializeInterface()
        {
            if (Authentification.IsLoggedIn)
            {

                btnLogout.IsEnabled = true;
            }

            //Hodiny v menu
            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }


        /// <summary>
        /// Časovač pro hodiny v menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Click(object sender, EventArgs e)
        {
            DateTime d;
            d = DateTime.Now;
            if (d.Minute >= 10)
            {
                lblClock.Content = d.Hour + " : " + d.Minute;
            }
            else
            {
                lblClock.Content = d.Hour + " : " + "0" + d.Minute;
            }
        }

        /// <summary>
        /// Událost tlačítka pro odhlášení uživatele.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (Authentification.Logout())
            {
                EntryWindow entryW = new EntryWindow();
                entryW.Show();
                Window.GetWindow(this).Close();
            }
        }
    }
}
