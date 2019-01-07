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

namespace PASS.Management
{
    /// <summary>
    /// Okno pro přidání uživatele.
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            FillRoles();

            btnSubmit.IsEnabled = false;
            tbUsername.TextChanged += Method;
            pbPassword.PasswordChanged += Method;
            pbPasswordAgain.PasswordChanged += Method;
            comboBoxRoles.SelectionChanged += Method;
        }

        private void FillRoles()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;

            try
            {
                IEnumerable<UserRole> result = from f in db.UserRoles
                                               select f;

                foreach (UserRole r in result)
                {
                    comboBoxRoles.Items.Add(r.name.Trim());
                }
                comboBoxRoles.Text = (from c in result
                                      select c.name).First().Trim();

            }
            catch
            {
                Errors.SaveError("Seznam rolí je prázdný.");
                DialogHelper.ShowError("Neexistují žádné role k přiřazení");
                btnSubmit.IsEnabled = false;
            }

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = ValidateUser.ValidateUsername(tbUsername.Text);
                string password = ValidateUser.ValidateNewPassword(pbPassword.Password);
                string passwordAgain = pbPasswordAgain.Password;
                if (password == passwordAgain)
                {

                    if (Authentification.NewUser(username, pbPassword.Password, comboBoxRoles.Text))
                    {
                        DialogHelper.ShowInfo("Uživatel úspěšně přidán.");
                        this.Close();
                    }
                    else throw new NotImplementedException();
                }
                else
                {
                    DialogHelper.ShowWarning("Hesla se neshodují");
                    pbPasswordAgain.Password = string.Empty;
                }

            }
            catch (InvalidUsernameException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch (InvalidAuthPasswordException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch
            {
                DialogHelper.ShowError("Uživatel nemohl být přidán.");
            }

        }

        private void Method(object sender, RoutedEventArgs e)
        {

            btnSubmit.IsEnabled = true;
        }


    }
}
