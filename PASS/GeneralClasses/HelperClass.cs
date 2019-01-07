using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PASS.GeneralClasses
{
    /// <summary>
    /// Třída poskytující uživatečné metody.
    /// </summary>    
    public static class HelperClass
    {

        public static string GetFullDatetime(DateTime? date)
        {
            try
            {
                return ((DateTime)date).ToShortDateString() + " " + ((DateTime)date).ToShortTimeString();
            }
            catch
            {
                return "";
            }
        }
        public static bool IsInteger(string number)
        {
            if (number.Contains('.') || number.Contains(','))
            {
                return false;
            }
            else return true;
        }

        public static void RestartApplication()
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }


    }

    /// <summary>
    /// Třída pro validaci vstupních dat - produkt
    /// </summary>
    public static class ValidateProduct
    {

        public static string Name(string name)
        {
            if (string.IsNullOrEmpty(name) || (name.Length > 0 && name.Trim().Length == 0))
            {
                throw new InvalidNameException();
            }
            else return name;
        }

        public static int Quantity(string quantity)
        {
            try
            {
                int _quantity = Convert.ToInt32(quantity);
                if (_quantity <= 0)
                {
                    throw new InvalidQuantityException();
                }
                else return _quantity;
            }
            catch
            {
                throw new InvalidQuantityException();
            }
        }

        public static decimal UnitQuantity(string quantity)
        {
            try
            {
                decimal _quantity = Convert.ToDecimal(quantity);
                if (_quantity <= 0)
                {
                    throw new InvalidUnitQuantityException();
                }
                else return _quantity + 0.00m; // do databáze ukládat vždy i s desetinnou částí
            }
            catch
            {
                throw new InvalidUnitQuantityException();
            }
        }

        public static int Code(string code)
        {

            int _code = -1; // Nevalidní kód jako počáteční hodnota
            try
            {
                _code = Convert.ToInt32(code);
            }
            catch
            {
                throw new InvalidCodeException();
            }

            // Uživatel nesmí zadat kód menší než nula
            if (_code < 0)
            {
                throw new InvalidCodeException();
            }

            // Získání všech již existujících kódů
            IEnumerable<int> codes = Storage.StorageSetup.GetCodes();
            foreach (int c in codes)
            {
                if (c == _code)
                {
                    throw new ExistingCodeException();
                }
            }

            return _code;


        }

        public static int Code(string code, int currentCode)
        {
            int _code = -1; // Nevalidní kód jako počáteční hodnota
            try
            {
                _code = Convert.ToInt32(code);
            }
            catch
            {
                throw new InvalidCodeException();
            }

            // Uživatel nesmí zadat kód menší než nula
            if (_code < 0)
            {
                throw new InvalidCodeException();
            }

            // Získání všech již existujících kódů
            IEnumerable<int> codes = Storage.StorageSetup.GetCodes();
            foreach (int c in codes)
            {
                if (c == _code && _code != currentCode)
                {
                    throw new ExistingCodeException();
                }
            }

            return _code;

        }

        public static decimal Price(string price)
        {
            try
            {
                decimal _price = Convert.ToDecimal(price);
                if (_price < 0)
                {
                    throw new InvalidPriceException();
                }
                else return _price;
            }
            catch
            {
                throw new InvalidPriceException();
            }
        }

        public static DateTime? Date(DateTime? date)
        {

            if (date == null)
            {
                throw new NullDateTimeException();
            }
            else return date;

        }
    }

    public static class ValidateUnit
    {
        public static string Name(string unitName)
        {
            if (unitName.Contains(" ") || string.IsNullOrEmpty(unitName))
            {
                throw new InvalidUnitNameException();
            }
            else return unitName;
        }
    }


    public static class ValidateAmininistrator
    {
        public static string ValidateAdminPassword(string password)
        {
            string pswd = password;
            if (pswd.Contains(" ") || string.IsNullOrEmpty(pswd))
            {
                throw new InvalidAdminPasswordException();
            }
            else
            {
                return pswd;
            }
        }
    }


    public static class ValidateUser
    {
        public static string ValidateUsername(string username, string currentUsername)
        {
            string trimmedString = username.Trim();
            if (trimmedString.Contains(" ") || string.IsNullOrEmpty(trimmedString))
            {
                throw new InvalidUsernameException();
            }

            // Username musí být unikátní
            LinqToSqlDataContext db = DatabaseSetup.Database;
            IEnumerable<string> allUsernames = from u in db.Users
                                               select u.username;
            foreach (string singleUsername in allUsernames)
            {
                if (singleUsername.Trim() == trimmedString && trimmedString != currentUsername)
                {
                    throw new UsernameAlreadyExistsException();
                }
            }


            return trimmedString;
        }

        public static string ValidateUsername(string username)
        {
            string trimmedString = username.Trim();
            if (trimmedString.Contains(" ") || string.IsNullOrEmpty(trimmedString))
            {
                throw new InvalidUsernameException();
            }

            // Username musí být unikátní
            LinqToSqlDataContext db = DatabaseSetup.Database;
            IEnumerable<string> allUsernames = from u in db.Users
                                               select u.username;
            foreach (string singleUsername in allUsernames)
            {
                if (singleUsername.Trim() == trimmedString)
                {
                    throw new UsernameAlreadyExistsException();
                }
            }

            return trimmedString;
        }

        public static string ValidatePassword(string password)
        {
            string pswd = password;
            if (pswd.Contains(" ") || string.IsNullOrEmpty(pswd))
            {
                throw new InvalidAuthPasswordException();
            }
            else
            {
                return pswd;
            }
        }

        public static string ValidateNewPassword(string password)
        {
            string pswd = password;
            if (pswd.Contains(" ") || string.IsNullOrEmpty(pswd))
            {
                throw new InvalidNewPasswordException();
            }
            else
            {
                return pswd;
            }
        }
    }

    public static class DialogHelper
    {
        public static void ShowWarning(string text)
        {
            MessageBox.Show(text, "Varování", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        public static void ShowInfo(string text)
        {
            MessageBox.Show(text, "Upozornění", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
