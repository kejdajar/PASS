using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PASS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex myMutex;

        private void Application_Startup(object sender, StartupEventArgs e)
        {     
           bool isNewInstance = false;
            myMutex = new Mutex(true, "PASS", out isNewInstance);
            if (!isNewInstance)
            {
               PASS.GeneralClasses.DialogHelper.ShowError("Program je již spuštěný.");
                App.Current.Shutdown();
            } 
        
    }
    }
}
