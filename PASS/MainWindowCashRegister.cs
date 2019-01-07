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
using PASS.Management;
using PASS.Storage;
using System.Windows.Automation.Peers;
using PASS.CashRegister;

namespace PASS
{
    public partial class MainWindow : Window
    {

        /*
        PARTIAL CLASS - ZÁLOŽKA POKLADNA
       */

        private void InitializeCashRegister()
        {
            CashRegisterPage1 = new CRPage1();
            cashRegisterFrame.Navigate(CashRegisterPage1);
            CashRegisterFrame = cashRegisterFrame;

            // Nastavení focusu na textbox v pokladně

            if (Authentification.IsInRole(Authentification.AuthUser.Id, "pokladní"))
            {
                CashRegisterPage1.tbProductCode.Focus();
            }

        }

        public CRPage1 CashRegisterPage1;
        public static Frame CashRegisterFrame;
    }
}
