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
using PASS.Storage;

namespace PASS
{
    /// <summary>
    /// Přihlašovací okno.
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            InitializeWindow();
        }

        public static bool SuperuserAsDefault { get; set; } = false;
        private bool _isAuthentificated = false;
        public bool IsAuthentificated
        {
            get { return _isAuthentificated; }
        }

        private void InitializeWindow()
        {
            if (SuperuserAsDefault)
            {
                tbUsername.Text = "manažer";
                tbPassword.Password = "manažer";
            }
            tbUsername.Focus();

        }

        // Po zavření se zavře i okno na pozadí
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {

                if (!_isAuthentificated)
                    Owner.Close();
            }
            catch
            {

            }
        }

        // Přihlášení
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            DatabaseSetup.InitializeLinqToSql();



            if (Properties.Settings.Default.IsFullDb == "1")
            {

                try
                {
                    DatabaseSetup.SetupDatabase(); DialogHelper.ShowInfo("Databáze byla automaticky naplněna daty.");
                }
                catch
                {
                    DialogHelper.ShowInfo("Zvolená databáze nemohla být naplněna daty, neboť již nějaké obsahuje.");
                }
            }

            try
            {
                Authentification.Authentificate(tbUsername.Text, tbPassword.Password);
                _isAuthentificated = true;
                this.Close();
            }
            catch (InvalidUsernameOrPasswordException ex)
            {
                tbPassword.Password = string.Empty;
                DialogHelper.ShowWarning(ex.Message);
            }
        }



        //  Zavření okna
        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Otevření okna s nastavením.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsLoginWindow lws = new SettingsLoginWindow();
            lws.Owner = this;
            lws.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            lws.ShowInTaskbar = false;
            lws.ShowDialog();


            if (lws.Auth)
            {
                LoginWindowSettings s = new LoginWindowSettings();
                s.Owner = this;
                s.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                s.ShowInTaskbar = false;
                s.ShowDialog();
            }

            // Vrátí focus 
            tbUsername.Focus();

        }


    }
}
