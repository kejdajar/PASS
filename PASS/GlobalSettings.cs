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
using PASS.Storage;
using PASS.GeneralClasses;

namespace PASS
{
    /// <summary>
    /// Třída pro nastavení chování programu.
    /// </summary>
    public static class GlobalSettings
    {
        /* 
           Nastavení se aplikuje ihned při načtení obrazovky s přihlášením.
           Kód níže se volá pouze jednou, při startu aplikace.
         
           Při přejmenování složky s programem se ztratí vazba na settings.settings a nastavení se resetuje.

           Při nasazení do provozu:
           1. vypnout testovací DB     
           2. vypnout ResetAppAndDatabase 
        

           Při vytváření nové fullDB nemusí existovat databáze s názvem PASS2016, 
           je potřeba ale upravit connection string na daný název.
                   
            */

        // Heslo k nastavení programu na přihlašovací stránce
        public static string AdminPassword = "admin";


        public static void ApplyGlobalSettings()
        {




            // Týká se databáze v APPDATA
           //  ResetAppAndDatabase();
        

            // Předvyplnit přihlašovací okno
            LoginWindow.SuperuserAsDefault = false;
           
            // Zvolit testovací databázi ve složce PASS/AppData (tzn. LocalDB, defaultní nastavení pro VS)
            //  DatabaseSetup.UseTestDatabase();

        }

        

        private static void ResetAppAndDatabase()
        {
            //Reset nastavení = aktivace překopírování prázdné databáze do APPDATA
            Properties.Settings.Default.Reset();      
        }

    }
}
