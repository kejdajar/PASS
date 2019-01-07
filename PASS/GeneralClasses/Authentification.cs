using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Windows;

namespace PASS.GeneralClasses
{
    /// <summary>
    /// Třída zajišťující uživatelské role a přihlašování.
    /// </summary>
    /// 
    static class Authentification
    {
        /// <summary>
        /// Provede ověření uživatele. Při úspěchu vrací true.
        /// </summary>
        /// <param name="username">Uživatelské jméno</param>
        /// <param name="password">Heslo</param>
        /// <exception cref="InvalidUsernameOrPasswordException"></exception>
        public static void Authentificate(string username, string password)
        {
            List<string> result = ValidateLoginInput(username, password);
            string _username = result[0];
            string _password = result[1];
            LinqToSqlDataContext db = DatabaseSetup.Database;
            IEnumerable<User> users = from u in db.Users
                                      where (_username == u.username.Trim())
                                      select u;
            if (users.Count() == 0) throw new InvalidUsernameOrPasswordException(); // nebyl nalezen uživatel s daným username

            if (ComparePassword(_password, users.First().pswd, users.First().salt))
            {
                AuthUser = new CurrentUser(users.First());
            }
            else
            {
                Errors.SaveError("Uživatel: " + username + " zadal špatně uživatelské heslo.");
                throw new InvalidUsernameOrPasswordException();
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidUsernameOrPasswordException"></exception>
        public static List<string> ValidateLoginInput(string username, string pswd)
        {
            string trimmedUsername = username.Trim();
            if (trimmedUsername.Contains(" ") || string.IsNullOrEmpty(trimmedUsername))
            {
                throw new InvalidUsernameOrPasswordException();
            }

            string trimmedPswd = pswd.Trim();
            if (trimmedPswd.Contains(" ") || string.IsNullOrEmpty(trimmedPswd))
            {
                throw new InvalidUsernameOrPasswordException();
            }

            return new List<string>() { trimmedUsername, trimmedPswd };
        }

        /// <summary>
        /// Slouží pro ověření aktuálně přihlášeného uživatele
        /// </summary>
        public static bool CheckUserPassword(string enteredPassword)
        {

            LinqToSqlDataContext db = DatabaseSetup.Database;
            CurrentUser currentUser = Authentification.AuthUser;

            if (currentUser == null)
            {
                throw new UserNotLoggedInException();
            }

            if (ComparePassword(enteredPassword, currentUser.Password, currentUser.Salt))
            {
                return true;
            }
            else return false;
        }


        public static bool IsLoggedIn
        {
            get
            {
                if (AuthUser != null) { return true; } else return false;

            }
        }


        public static CurrentUser AuthUser;

        /// <summary>
        /// Vytvoří nového uživatele. Při úspěchu vrací true.
        /// </summary>
        /// <param name="username">Unikátní uživatelské jméno</param>
        /// <param name="pswd">Heslo</param>
        /// <param name="role">Existující role</param>
        /// <returns></returns>
        public static bool NewUser(string username, string pswd, string role)
        {
            if (username == "" || pswd == "")
            {
                return false;
            }

            LinqToSqlDataContext db = DatabaseSetup.Database;
            PASS.User newUser = new PASS.User();
            string hashedPassword;
            string salt;
            HashPassword(pswd, out salt, out hashedPassword);

            newUser.salt = salt;
            newUser.pswd = hashedPassword;
            newUser.username = username;
            newUser.userRole = (from r in db.UserRoles where (r.name.Trim() == role) select r.id).Single();
            db.Users.InsertOnSubmit(newUser);

            try
            {
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message);
                DatabaseSetup.UndoChanges();
                return false;
            }


        }
        /// <summary>
        /// Změní heslo uživatele. V případě úspěchu vrací true.
        /// </summary>
        /// <param name="user">Uživatel</param>
        /// <param name="password">Nové heslo</param>
        /// <returns></returns>
        public static void ChangePassword(int userId, string password)
        {
            if (password == "" || userId < 0) throw new NotImplementedException();

            LinqToSqlDataContext db = DatabaseSetup.Database;
            User selectedUser = (from u in db.Users
                                 where u.id == userId
                                 select u).Single();


            string hashedPassword;
            string salt;
            HashPassword(password, out salt, out hashedPassword);

            try
            {
                selectedUser.pswd = hashedPassword;
                selectedUser.salt = salt;
                db.SubmitChanges();

            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message);
                DatabaseSetup.UndoChanges();
                throw new NotImplementedException();
            }

        }

        /// <summary>
        /// Změní uživatelské jméno.
        /// </summary>
        /// <param name="userId">ID uživatele.</param>
        /// <param name="username">Nové uživatelské jméno.</param>
        /// <returns></returns>
        public static void ChangeUsername(int userId, string username)
        {
            if (username == "" || userId < 0) throw new NotImplementedException();

            LinqToSqlDataContext db = DatabaseSetup.Database;
            User selectedUser = (from u in db.Users
                                 where u.id == userId
                                 select u).Single();

            selectedUser.username = username;

            try
            {
                db.SubmitChanges();

            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message);
                DatabaseSetup.UndoChanges();
                throw new NotImplementedException();
            }

        }

        /// <summary>
        /// Změní uživatelskou roli.
        /// </summary>
        /// <param name="userId">ID uživatele</param>
        /// <param name="userRole">Nová uživatelská role</param>
        /// <returns></returns>
        public static void ChangeUserRole(int userId, string userRole)
        {
            if (userRole == "" || userId < 0) throw new NotImplementedException();

            LinqToSqlDataContext db = DatabaseSetup.Database;

            User selectedUser = (from u in db.Users
                                 where u.id == userId
                                 select u).Single();

            UserRole r = (from role in db.UserRoles
                          where role.name == userRole
                          select role).Single();

            selectedUser.userRole = r.id;

            try
            {
                db.SubmitChanges();

            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message);
                DatabaseSetup.UndoChanges();
                throw new NotImplementedException();
            }

        }
        /// <summary>
        /// Vytvoří novou uživatelskou roli.
        /// </summary>
        /// <param name="name">Unikátní jméno role</param>
        /// <returns></returns>
        public static bool NewRole(string name)
        {

            if (name == "") return false;

            LinqToSqlDataContext db = DatabaseSetup.Database;
            UserRole role = new UserRole();
            role.name = name;
            db.UserRoles.InsertOnSubmit(role);

            try
            {
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Ověří příslušnost uživatele k roli.
        /// </summary>
        /// <param name="userId">Uživatel k otestování.</param>
        /// <param name="role">Očekávaná role.</param>
        /// <returns></returns>
        public static bool IsInRole(int userId, string role)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            string roleNameFromDb = (from user in db.Users
                                     where user.id == userId
                                     join r in db.UserRoles
                                     on user.userRole equals r.id
                                     select r.name).Single();
            if (roleNameFromDb.Trim() == role)
            {
                return true;
            }
            else return false;
        }



        /// <summary>
        /// Odstraněné uživatele.
        /// </summary>
        /// <param name="userID">ID uživatele</param>
        /// <returns></returns>
        public static bool DeleteUser(int userID)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;

            User selectedUser = (from u in db.Users
                                 where u.id == userID
                                 select u).Single();
            db.Users.DeleteOnSubmit(selectedUser);

            try
            {
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message);
                return false;
            }

        }


        /// <summary>
        /// Odhlásí auktuálně přihlášeného uživatele.
        /// </summary>
        /// <returns></returns>
        public static bool Logout()
        {
            if (IsLoggedIn)
            {
                AuthUser = null;
                return true;
            }
            else return false;
        }


        /****************************
         * 
         * 
         *  ZABEZEPEČENÍ PŘIHLAŠOVÁNÍ
         * 
         * 
         ****************************/


        /// <summary>
        /// Vytvoří HASH z hesla a saltu.
        /// </summary>
        /// <param name="password">Heslo k zahashování</param>
        /// <param name="salt">Salt</param>
        /// <param name="hashedPassword">Výstupní zahashované heslo</param>
        public static void HashPassword(string password, out string salt, out string hashedPassword)
        {

            // salt
            byte[] _salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(_salt);

            //hash
            var h = new Rfc2898DeriveBytes(password, _salt, 10000);
            var hash = h.GetBytes(20);

            salt = Convert.ToBase64String(_salt);
            hashedPassword = Convert.ToBase64String(hash);

        }

        /// <summary>
        /// Porovná zadané heslo s heslem v databázi.
        /// </summary>
        /// <param name="inputPassword">Heslo pro ověření.</param>
        /// <param name="dbHashedPassword">Heslo z databáze.</param>
        /// <param name="dbSalt">Salt z databáze.</param>
        /// <returns></returns>
        public static bool ComparePassword(string inputPassword, string dbHashedPassword, string dbSalt)
        {

            var salt = Convert.FromBase64String(dbSalt);
            var v = new Rfc2898DeriveBytes(inputPassword, salt, 10000);
            var vHash = v.GetBytes(20);

            var pswdFromDb = Convert.FromBase64String(dbHashedPassword);

            for (int i = 0; i < 20; i++)
            {
                if (vHash[i] != pswdFromDb[i])
                {
                    return false;
                }
            }

            return true;
        }

        // Administrátorské heslo
        public static void CreateAdministratorPassword(string password)
        {
            string dir = (System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS"));
            // Ověření existence adresáře
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);

            }

            string salt = null;
            string hashedPswd = null;
            try
            {

                HashPassword(password, out salt, out hashedPswd);
                using (StreamWriter writer = new StreamWriter(File.Create(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "PassData.bin"))))
                {
                    writer.WriteLine(hashedPswd);
                    writer.WriteLine(salt);
                }
            }
            catch
            {
                throw new AdministratorSetupFailedException();
            }

        }

        public static bool AuthAdministrator(string password)
        {
            using (StreamReader reader = new StreamReader(File.Open(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "PassData.bin"), FileMode.Open)))
            {
                string pswd = reader.ReadLine();
                string salt = reader.ReadLine();

                bool result = ComparePassword(password, pswd, salt);
                return result;
            }

        }

    }

    public class CurrentUser
    { /// <summary>
      /// Třída představující aktuálně přihlášeného uživatele.
      /// </summary>
      /// <param name="user">LINQ TO SQL Databázový objekt uživatele</param>
        public CurrentUser(User user)
        {
            this._user = user;
        }

        private User _user;

        public int Id
        {
            get { return _user.id; }
        }

        public string Username
        {
            get { return _user.username.Trim(); }
        }

        public string Password
        {
            get { return _user.pswd; }
        }

        public string Salt
        {
            get { return _user.salt; }
        }

        public string UserRole
        {
            get
            {
                LinqToSqlDataContext db = DatabaseSetup.Database;
                string result = (from r in db.UserRoles
                                 where r.id == _user.userRole
                                 select r.name).Single();
                return result;
            }
        }

    }


}
