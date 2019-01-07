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

namespace PASS.Profile
{
    /// <summary>
    /// Interaction logic for OtherSettingsPage.xaml
    /// </summary>
    public partial class OtherSettingsPage : Page
    {
        public OtherSettingsPage()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (Authentification.Logout())
            {
                EntryWindow entryW = new EntryWindow();
                entryW.Show();
                MainWindow w = (MainWindow)Window.GetWindow(this);
                w.Close();
            }
        }
    }
}
