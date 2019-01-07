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
using PASS.CashRegister;

namespace PASS
{
    /// <summary>
    /// Hlavní okno programu.
    /// </summary>
    public partial class MainWindow : Window
    {



        public static MainWindow SelfReference { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SelfReference = this;

            InitializeInterface(); // Nastavení popisků GUI ...
            ApplyAuthentification(); // Nastevení přístupových práv

            IntializeProfile();    // Sekce profil uživatele
            InitializeManagement(); // Sekce management
            InitializeStorage();    // Sekce sklad           
            InitializeCashRegister(); // Sekce pokladna
        }


        /// <summary>
        /// Omezí přístup dle role
        /// </summary>
        private void ApplyAuthentification()
        {

            if (Authentification.IsInRole(Authentification.AuthUser.Id, "skladník"))
            {
                // Zakázat
                tabManagement.Visibility = Visibility.Collapsed;
                tabCashRegister.Visibility = Visibility.Collapsed;

                //Defaultně otevřená záložka
                tabMenu.SelectedItem = tabStorage;
            }

            if (Authentification.IsInRole(Authentification.AuthUser.Id, "pokladní"))
            {
                // Zakázat
                tabManagement.Visibility = Visibility.Collapsed;
                tabStorage.Visibility = Visibility.Collapsed;

                //Defaultně otevřená záložka
                tabMenu.SelectedItem = tabCashRegister;
            }

            if (Authentification.IsInRole(Authentification.AuthUser.Id, "manažer"))
            {
                // Defaultně otevřená záložka
                tabMenu.SelectedItem = tabManagement;
            }

        }


        private void InitializeInterface()
        {


            if (Authentification.IsLoggedIn)
            {
                lblUsername.Content = "Přihlášený uživatel: " + Authentification.AuthUser.Username + ", role: " + Authentification.AuthUser.UserRole;

            }

            // Pravé menu
            MenuPanelPage rightMenu = new MenuPanelPage();
            frMenuPanel.Navigate(rightMenu);


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
                this.Close();
            }
        }

        /// <summary>
        /// Nastavení focusu na žádoucí prvek jednotlivých záložek při přechodu.
        /// </summary>
        private void tabMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CashRegisterPage1 != null)
            {
                TextBox tbProductCode = CashRegisterPage1.tbProductCode;
                TextBox tbQuantity = CashRegisterPage1.tbQuantity;

                if (tabCashRegister.IsSelected && e.Source is TabControl)
                {
                    if (tbProductCode.IsEnabled)
                    {
                        Dispatcher.BeginInvoke(new Action(() => { Keyboard.Focus(tbProductCode); }));

                    }
                    else
                    {
                        Dispatcher.BeginInvoke(new Action(() => { Keyboard.Focus(tbQuantity); }));
                    }

                }
            }

            /* 
             Záložka profil neobsahuje žádný textbox a po zobrazení si ponechává focus, proto je nutné ho vyčistit
             */
            if (tabProfile.IsSelected)
            {
                Keyboard.ClearFocus();
            }

            // Při přechodu na záložku sklad - automaticky aktualizovat datagrid (například přechod pokladna -> sklad)
            if (tabStorage.IsSelected && e.Source is TabControl)
            {
                InitializeStorage();
            }

            if (tabManagement.IsSelected && e.Source is TabControl)
            {
                InitializeManagement();
            }

            if (tabProfile.IsSelected && e.Source is TabControl)
            {
                IntializeProfile();
            }


        }


        /// <summary>
        /// Zobrazení postranního menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuDisplaySideMenu_Checked(object sender, RoutedEventArgs e)
        {
            sideColumn.Width = new GridLength(200);
            sideColumn.MinWidth = 200;
        }

        /// <summary>
        /// Skrytí postranního menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuDisplaySideMenu_Unchecked(object sender, RoutedEventArgs e)
        {
            sideColumn.Width = new GridLength(0);
            sideColumn.MinWidth = 0;
        }

        /// <summary>
        /// Zobrazení okna: O programu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow ow = new AboutWindow();
            ow.ShowInTaskbar = false;
            ow.Owner = this;
            ow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ow.ShowDialog();
        }

        private void menuBtnExitProgram_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
