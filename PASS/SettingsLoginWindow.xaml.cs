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
    /// Interaction logic for SettingsLoginWindow.xaml
    /// </summary>
    public partial class SettingsLoginWindow : Window
    {
        public SettingsLoginWindow()
        {
            InitializeComponent();
            tbPswd.Focus();
            if (LoginWindow.SuperuserAsDefault)
            {
                tbPswd.Password = GlobalSettings.AdminPassword;
            }
        }

        public bool Auth { get; set; } = false;

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string pswd = ValidateAmininistrator.ValidateAdminPassword(tbPswd.Password);


                if (Authentification.AuthAdministrator(pswd))
                {

                    Auth = true;
                    this.Close();
                }
                else
                {

                    tbPswd.Password = string.Empty;
                    throw new InvalidAdminPasswordException();
                }
            }
            catch (InvalidAdminPasswordException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }


        }


    }


}
