using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PASS.Management
{
    /// <summary>
    /// Třída pro nastavení management záložky.
    /// </summary>
    static class ManagementSetup
    {
        public static void InitializeUserTable(DataGrid usersGrid)
        {

            LinqToSqlDataContext db = PASS.GeneralClasses.DatabaseSetup.Database;
            var userVariable = from u in db.Users
                               join r in db.UserRoles
                               on u.userRole equals r.id
                               select new UserRecord { id = u.id, username = u.username.Trim(), role = r.name.Trim() };

            usersGrid.ItemsSource = userVariable;

        }

    }

    public struct UserRecord
    {
        public int id { get; set; }
        public string username { get; set; }
        public string role { get; set; }
    }
}
