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

namespace PASS
{
    /// <summary>
    /// Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            pbAdminPswd.Focus();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow ow = new AboutWindow();
            ow.ShowInTaskbar = false;
            ow.Owner = this;
            ow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ow.ShowDialog();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Uložení hesla
            try
            {
                string pswd = ValidateAmininistrator.ValidateAdminPassword(pbAdminPswd.Password);
                if (pswd == pbAdminPswdAgain.Password)
                {
                    DatabaseSetup.RecreateAppData();
                    Authentification.CreateAdministratorPassword(pbAdminPswd.Password);
                }
                else
                {
                    DialogHelper.ShowWarning("Hesla se neshodují.");
                    pbAdminPswdAgain.Password = string.Empty;
                    pbAdminPswdAgain.Focus(); return;

                }

            }
            catch (InvalidAdminPasswordException ex)
            {
                DialogHelper.ShowWarning(ex.Message); pbAdminPswd.Password = string.Empty; pbAdminPswd.Focus(); return;
            }
            catch (AdministratorSetupFailedException ex)
            {
                DialogHelper.ShowError(ex.Message); return;
            }
            catch
            {
                return;
            }


            Success = true;
            this.Close();

        }
        public bool Success { get; set; } = false;

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (!Success)
                    Owner.Close();

            }
            catch
            {

            }
        }
    }
}
