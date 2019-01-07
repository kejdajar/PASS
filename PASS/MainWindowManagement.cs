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

namespace PASS
{
    public partial class MainWindow : Window
    {

        /*
        PARTIAL CLASS - ZÁLOŽKA MANAGEMENT
       */

        private void InitializeManagement()
        {
            UsersPage usersPage = new UsersPage();
            frManagementUsers.Navigate(usersPage);

            SummaryMangPage summaryPage = new SummaryMangPage();
            frManagementSummary.Navigate(summaryPage);

            CompanyInfoPage coInfoPage = new CompanyInfoPage();
            frManagementCoInfo.Navigate(coInfoPage);

        }

        // Aktualizace stránek při přepnutí 
        private void tabControlManagement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabManagementUsers.IsSelected && e.Source is TabControl)
            {
                UsersPage usersPage = new UsersPage();
                frManagementUsers.Navigate(usersPage);

            }

            if (tabManagementSummary.IsSelected && e.Source is TabControl)
            {
                SummaryMangPage summaryPage = new SummaryMangPage();
                frManagementSummary.Navigate(summaryPage);

            }


            if (tabManagementCoInfo.IsSelected && e.Source is TabControl)
            {
                CompanyInfoPage coInfoPage = new CompanyInfoPage();
                frManagementCoInfo.Navigate(coInfoPage);

            }
        }

    }
}
