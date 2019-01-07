using PASS.GeneralClasses;
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

namespace PASS.Profile
{
    /// <summary>
    /// Interaction logic for PasswordChangePage.xaml
    /// </summary>
    public partial class PasswordChangePage : Page
    {
        public PasswordChangePage()
        {
            InitializeComponent();
            InitializeInterface();
        }

        private void pswdSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Zkontrolovat původní heslo
            bool isUserAuthentificated = false;
            try
            {
                string enteredPswd = ValidateUser.ValidatePassword(pbFormerPswd.Password);
                if (Authentification.CheckUserPassword(enteredPswd))
                {
                    // Heslo ověřeno, pokračujeme dále --> kontrola nového hesla 
                    isUserAuthentificated = true;
                }
                else
                {
                    DialogHelper.ShowWarning("Původní heslo nebylo zadáno správně.");
                    pbFormerPswd.Password = string.Empty;
                }
            }
            catch (UserNotLoggedInException ex)
            {
                DialogHelper.ShowError(ex.Message);
            }
            catch (InvalidAuthPasswordException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch
            {
                DialogHelper.ShowError("Uživatel nemohl být ověřen.");
            }

            // Validace nového hesla
            if (isUserAuthentificated)
            {
                try
                {
                    string newPswd = ValidateUser.ValidateNewPassword(pbNewPswd.Password);
                    string newPswdAgain = pbNewPswdAgain.Password;
                    if (newPswd == newPswdAgain)
                    {
                        Authentification.ChangePassword(Authentification.AuthUser.Id, newPswd);
                        DialogHelper.ShowInfo("Heslo bylo úspěšně změněno.");
                        InitializeInterface();
                    }
                    else throw new PasswordsDoNotMatchException();

                }
                catch (InvalidNewPasswordException ex)
                {
                    DialogHelper.ShowWarning(ex.Message);
                }
                catch (PasswordsDoNotMatchException ex)
                {
                    DialogHelper.ShowWarning(ex.Message);
                }
                catch
                {
                    DialogHelper.ShowError("Heslo nemohlo být změněno.");
                }
            }

        }

        private void InitializeInterface()
        {
            pbFormerPswd.Password = string.Empty;
            pbNewPswd.Password = string.Empty;
            pbNewPswdAgain.Password = string.Empty;
            pswdSubmit.IsEnabled = false;
            pbFormerPswd.PasswordChanged += Method;
            pbNewPswd.PasswordChanged += Method;
            pbNewPswdAgain.PasswordChanged += Method;
        }

        private void Method(object sender, RoutedEventArgs e)
        {
            pswdSubmit.IsEnabled = true;
        }
    }
}
