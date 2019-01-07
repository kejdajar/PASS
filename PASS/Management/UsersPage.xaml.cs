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

namespace PASS.Management
{
    /// <summary>
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            ManagementSetup.InitializeUserTable(dgUsers);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            int ID = (int)((Button)sender).CommandParameter;
            ShowUserEditWindow(ID);

        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow newUserWindow = new AddUserWindow();

            newUserWindow.Owner = Window.GetWindow(this);
            newUserWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            newUserWindow.ShowDialog();

            ManagementSetup.InitializeUserTable(dgUsers);

        }

        private void ShowUserEditWindow(int ID)
        {
            EditUserWindow euw = new EditUserWindow(ID);
            euw.Owner = Window.GetWindow(this);
            euw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            euw.ShowDialog();
            ManagementSetup.InitializeUserTable(dgUsers);
        }

        private void dgUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgUsers.SelectedItem == null) return;
            UserRecord record = (UserRecord)dgUsers.SelectedItem;
            ShowUserEditWindow(record.id);
        }


        private void btnDelAllUsers_Click(object sender, RoutedEventArgs e)
        {

            LinqToSqlDataContext db = PASS.GeneralClasses.DatabaseSetup.Database;
            IEnumerable<User> allUsers = from u in db.Users
                                         select u;


            foreach (User u in allUsers)
            {
                if (u.id != PASS.GeneralClasses.Authentification.AuthUser.Id) // Aktuální uživatel nepůjde smazat
                {
                    db.Users.DeleteOnSubmit(u);
                }
            }

            try
            {
                db.SubmitChanges();
                ManagementSetup.InitializeUserTable(dgUsers);
                DialogHelper.ShowInfo("Všichni uživatelé kromě aktuálně přihlášeného byli odstraněni.");
            }
            catch
            {
                throw new NotImplementedException();
            }

        }
    }
}
