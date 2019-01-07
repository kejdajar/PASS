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

namespace PASS
{
    public partial class MainWindow : Window
    {
        /*
          PARTIAL CLASS - ZÁLOŽKA SKLAD
         */

        private void InitializeStorage()
        {
            frStorage.Navigate(new StoragePage());
        }
      

    }
}
