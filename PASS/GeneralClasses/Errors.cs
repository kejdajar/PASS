using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PASS.GeneralClasses
{
    /// <summary>
    /// Třída poskytuje nástroje pro registraci chyb
    /// </summary>
    static class Errors
    {
        public static string ErrorLogLocation { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "errorLog.txt");

        /// <summary>
        /// Uloží zprávu o chybě do souboru errorLog.txt
        /// </summary>
        /// <param name="errorMsg"></param>
        public static void SaveError(string errorMsg)
        {
            int numberOfLines = 0;

            using (StreamReader sr = new StreamReader(File.Open(ErrorLogLocation, FileMode.OpenOrCreate)))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    numberOfLines++;
                }
            }

            using (StreamWriter sw = new StreamWriter(ErrorLogLocation, true))
            {
                DateTime time = DateTime.Now;
                sw.WriteLine(numberOfLines.ToString() + ". " + errorMsg.Replace(System.Environment.NewLine, " ") + " " + time.ToShortDateString() + " " + time.ToShortTimeString());
            }
        }

        public static bool DeleteErrorLog()
        {
            if (File.Exists(ErrorLogLocation))
            {
                File.Delete(ErrorLogLocation);
                return true;
            }
            else return false;
        }
    }

    public class InvalidNameException : Exception
    {
        public override string Message
        {
            get
            {
                return "Jméno produktu je neplatné.";
            }
        }
    }

    public class InvalidQuantityException : Exception
    {
        public override string Message
        {
            get
            {
                return "Množství je neplatné.";
            }
        }
    }

    public class InvalidCodeException : Exception
    {
        public override string Message
        {
            get
            {
                return "Kód je neplatný.";
            }
        }
    }

    public class InvalidPriceException : Exception
    {
        public override string Message
        {
            get
            {
                return "Cena je neplatná.";
            }
        }
    }

    public class InvalidUnitQuantityException : Exception
    {
        public override string Message
        {
            get
            {
                return "Zadané množství jednotky je neplatné.";
            }
        }
    }

    public class NullDateTimeException : Exception
    {
        public override string Message
        {
            get
            {
                return "Zadané datum je neplatné.";
            }
        }
    }

    public class ExistingCodeException : Exception
    {
        public override string Message
        {
            get
            {
                return "Produkt s daným kódem již existuje. Zvolte jiný kód.";
            }
        }
    }

    public class ExistingUnitNameException : Exception
    {
        public override string Message
        {
            get
            {
                return "Jednotka s daným jménem již existuje.";
            }
        }
    }

    public class UnitAddExcepton : Exception
    {
        public override string Message
        {
            get
            {
                return "Jednotka nemohla být přidána.";
            }
        }
    }

    public class UnitInUseException : Exception
    {
        public override string Message
        {
            get
            {
                return "Jednotka je přiřazena nějakému produktu a proto ji nelze odebrat.";
            }
        }
    }

    public class InvalidUsernameException : Exception
    {
        public override string Message
        {
            get
            {
                return "Zadané uživatelské jméno nesmí obsahovat mezery a nesmí být prázdné.";
            }
        }
    }

    public class InvalidAuthPasswordException : Exception
    {
        public override string Message
        {
            get
            {
                return "Zadané původní heslo nemá správný formát.";
            }
        }
    }

    public class InvalidNewPasswordException : Exception
    {
        public override string Message
        {
            get
            {
                return "Nové heslo nemá správný formát";
            }
        }
    }
    public class UsernameAlreadyExistsException : Exception
    {
        public override string Message
        {
            get
            {
                return "Dané uživatelské jméno již existuje.";
            }
        }
    }

    public class PasswordsDoNotMatchException : Exception
    {
        public override string Message
        {
            get
            {
                return "Hesla se neshodují.";
            }
        }
    }

    public class UserNotLoggedInException : Exception
    {
        public override string Message
        {
            get
            {
                return "Žádný uživatel není aktuálně přihlášný.";
            }
        }
    }

    public class InvalidUnitNameException : Exception
    {
        public override string Message
        {
            get
            {
                return "Název jednotky je neplatný.";
            }
        }
    }

    public class InvalidUsernameOrPasswordException : Exception
    {
        public override string Message
        {
            get
            {
                return "Přihlašovací jméno nebo heslo je neplatné.";
            }
        }
    }

    public class InvalidAdminPasswordException : Exception
    {
        public override string Message
        {
            get
            {
                return "Zadané heslo správce je neplatné.";
            }
        }
    }

    public class AdministratorSetupFailedException : Exception
    {
        public override string Message
        {
            get
            {
                return "Účet administrátora nebylo možné vytvořit.";
            }
        }
    }
    public class UserNoException : Exception
    {
        public override string Message
        {
            get
            {
                return "Žádné změny nebyly provedeny.";
            }
        }
    }

    public class InvalidCompanyNameException : Exception
    {
        public override string Message
        {
            get
            {
                return "Název společnosti je chybně vyplěn.";
            }
        }
    }

    public class InvalidPostalCodeException : Exception
    {
        public override string Message
        {
            get
            {
                return "PSČ má nesprávný formát.";
            }
        }
    }
}
