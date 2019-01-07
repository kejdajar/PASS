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
using PASS.GeneralClasses;
using PASS.Management;
using PASS.Storage;
using PASS.Profile;

namespace PASS
{
    public partial class MainWindow : Window
    {
        /*
        PARTIAL CLASS - ZÁLOŽKA PROFIL
       */

        private void IntializeProfile()
        {
            if (Authentification.IsLoggedIn)
            {
                SummaryPage profilePage = new SummaryPage();
                frSummary.Navigate(profilePage);

                PasswordChangePage pswdPage = new PasswordChangePage();
                frPasswordChange.Navigate(pswdPage);

                OtherSettingsPage otherSettingsPage = new OtherSettingsPage();
                frOtherOptions.Navigate(otherSettingsPage);
            }

        }

        // Aktualizace stránek při přechodu z jedné na druhou
        private void tabControlProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabProfileSummary.IsSelected && e.Source is TabControl)
            {
                SummaryPage profilePage = new SummaryPage();
                frSummary.Navigate(profilePage);

            }

            if (tabProfilePswdChange.IsSelected && e.Source is TabControl)
            {
                PasswordChangePage pswdPage = new PasswordChangePage();
                frPasswordChange.Navigate(pswdPage);
            }

            if (tabProfileOtherOptions.IsSelected && e.Source is TabControl)
            {
                OtherSettingsPage otherSettingsPage = new OtherSettingsPage();
                frOtherOptions.Navigate(otherSettingsPage);

            }
        }
    }
}
