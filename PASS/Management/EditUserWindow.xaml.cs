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
    /// Okno pro úpravu uživatele.
    /// </summary>
    public partial class EditUserWindow : Window
    {
        public EditUserWindow(int userId)
        {
            InitializeComponent();
            this._userId = userId;
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            User selectedUser = (from u in db.Users
                                where u.id == this._userId
                                select u).Single();

            this._selectedUser = selectedUser;

            UserRole selectedUserRole = (from r in db.UserRoles
                                        where r.id == selectedUser.userRole
                                        select r).Single();

          
            tbUsername.Text = selectedUser.username.Trim();
           

           IEnumerable<UserRole> allRoles = from ar in db.UserRoles
                                            select ar;

            if (cbRole.Items.Count != 0) cbRole.Items.Clear();
            foreach (UserRole r in allRoles)
            {
                cbRole.Items.Add(r.name.Trim());
            }
            cbRole.Text = selectedUserRole.name.Trim();

            //group1
            btnSubmitGroup1.IsEnabled = false;
            tbUsername.TextChanged += MethodGroup1;
            cbRole.SelectionChanged += MethodGroup1;

            //group2
            btnSubmitGroup2.IsEnabled = false;
            pbNewPswd.PasswordChanged += MethodGroup2;
            pbNewPswdAgain.PasswordChanged += MethodGroup2;

            //Aktuálně přihlášený uživatel (zde pouze manažer) sám sobě nemůže měnit roli
            if (_userId == Authentification.AuthUser.Id) 
            {
                dockPanelRole.Visibility = Visibility.Collapsed;
            }
        }

        private User _selectedUser = null;

        private int _userId;

        private void btnSubmitGroup1_Click(object sender, RoutedEventArgs e)
        {           
            try
            {

                string newUsername = ValidateUser.ValidateUsername(tbUsername.Text,_selectedUser.username.Trim());

                Authentification.ChangeUsername(_userId, tbUsername.Text);
                                
                Authentification.ChangeUserRole(_userId, cbRole.Text);  

                DialogHelper.ShowInfo("Požadované změny byly úspěšně provedeny.");   

                InitializeInterface();
            }
            catch(InvalidUsernameException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch (UsernameAlreadyExistsException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch
            {
                DialogHelper.ShowError("Kvůli neočekávané chybě operace selhala.");
            }

          
           
        }

        private void btnSubmitGroup2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string newPswd =ValidateUser.ValidatePassword(pbNewPswd.Password);
                string newPswdAgain =pbNewPswdAgain.Password;

                if (newPswd == newPswdAgain)
                {
                    Authentification.ChangePassword(_userId, newPswd);                    
                    pbNewPswd.Password = string.Empty;
                    pbNewPswdAgain.Password = string.Empty;
                    DialogHelper.ShowInfo("Heslo bylo úspěšně změněno.");
                    InitializeInterface();

                }
                else
                {
                    pbNewPswdAgain.Password = string.Empty;
                    throw new PasswordsDoNotMatchException();
                    
                }
            }
            catch(InvalidAuthPasswordException ex)
            {
                pbNewPswd.Password = string.Empty;pbNewPswd.Focus();
                DialogHelper.ShowWarning(ex.Message);
            }
            catch(PasswordsDoNotMatchException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch
            {
                DialogHelper.ShowError("Heslo nemohlo být změněno.");
            }
        }

        private void btnSubmitGroup3_Click(object sender, RoutedEventArgs e)
        {
            if (_userId != Authentification.AuthUser.Id)
            {
               if(  Authentification.DeleteUser(this._userId))
                {
                    DialogHelper.ShowInfo("Uživatel odstraněn.");
                    this.Close();
                }
               else
                {
                    DialogHelper.ShowWarning("Uživatel nemohl být odstraněn.");                    
                }
                
            }
            else
            {
                DialogHelper.ShowWarning("Není možné odstranit aktivního uživatele.");
            }
        }

        // Group 1
        private void MethodGroup1(object sender, RoutedEventArgs e)
        {

            btnSubmitGroup1.IsEnabled = true;
        }
       

        // Group2
        private void MethodGroup2(object sender, RoutedEventArgs e)
        {

            btnSubmitGroup2.IsEnabled = true;
        }
       

    }
}
