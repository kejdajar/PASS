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
using System.IO;

namespace PASS
{
    /// <summary>
    /// Okno nastavení programu.
    /// </summary>
    public partial class LoginWindowSettings : Window
    {
        public LoginWindowSettings()
        {
            InitializeComponent();
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            if (Properties.Settings.Default.IsFullDb == "0") //DB FILE
            {
                chb1.IsChecked = true;
                tbConnString.Text = DatabaseSetup.ConnString;
            }
            else // FULL MSSQL
            {
                chb2.IsChecked = true;
                tbConnString2.Text = DatabaseSetup.ConnString;
            }


            // Kdyby měl  tb zůstat prázdný, tak tam defaultně uložím minulý connection string
            if (string.IsNullOrEmpty(tbConnString2.Text))
            {
                tbConnString2.Text = Properties.Settings.Default.UserFullDbString;
            }

            if (string.IsNullOrEmpty(tbConnString.Text))
            {
                tbConnString.Text = Properties.Settings.Default.UserDbFileString;
            }

            // V případě že by ani minulý conn string nebyl k dispozici (první spuštní programu)
            if (string.IsNullOrEmpty(tbConnString2.Text))
            {
                tbConnString2.Text = DatabaseSetup.FullDbString;
            }

            if (string.IsNullOrEmpty(tbConnString.Text))
            {
                tbConnString.Text = DatabaseSetup.LocalDbString;
            }

        }

        /// <summary>
        /// Zkouška připojení ke zvolené databázi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestConnection_Click(object sender, RoutedEventArgs e)
        {

            if ((tbConnString.Text.Contains(".mdf")) && DatabaseSetup.TestDbConnection(tbConnString.Text))
            {
                DialogHelper.ShowInfo("Test připojení proběhl úspěšně, databáze byla změněna.");
                DatabaseSetup.ConnString = tbConnString.Text;

                Properties.Settings.Default.UserDbFileString = tbConnString.Text;
                Properties.Settings.Default.IsFullDb = "0";
                Properties.Settings.Default.Save();
            }
            else DialogHelper.ShowError("Pokus o připojení skončil neúspěchem.\nZpráva o chybě byla uložena do souboru errorLog.txt.");

        }

        /// <summary>
        /// Vyhledání databázového souboru.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.OpenFileDialog of = new System.Windows.Forms.OpenFileDialog();
            of.ValidateNames = false; // bez tohoto nastavení se při otevření soubor uzamkne a nelze otevřít opětovně
            of.Filter = "Databázový soubor |*.mdf";
            of.InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath((Environment.SpecialFolder.ApplicationData)), "PASS", "AppData");
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = of.FileName;
                tbConnString.Text = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + filename + ";Integrated Security=True";
            }

        }

        /// <summary>
        /// Zkouška připojení k FULL MSSQL DB.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestConnection2_Click(object sender, RoutedEventArgs e)
        {

            if (!tbConnString2.Text.Contains(".mdf") && DatabaseSetup.TestDbConnection(tbConnString2.Text))
            {
                DialogHelper.ShowInfo("Test připojení proběhl úspěšně, databáze byla změněna.");
                DatabaseSetup.ConnString = tbConnString2.Text;

                Properties.Settings.Default.UserFullDbString = tbConnString2.Text;
                Properties.Settings.Default.IsFullDb = "1";
                Properties.Settings.Default.Save();

            }
            else DialogHelper.ShowError("Pokus o připojení skončil neúspěchem.\nZpráva o chybě byla uložena do souboru errorLog.txt.");

        }

        private void btnNewLocalDb_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = fb.SelectedPath;
                try
                {
                    // Překopíruje prázdnou databázi do zvoleného adresáře
                    System.IO.File.Copy("AppData/Database.mdf", System.IO.Path.Combine(path, "Database.mdf"));
                    System.IO.File.Copy("AppData/Database_log.ldf", System.IO.Path.Combine(path, "Database_log.ldf"));

                    // Naplnit daty
                    string connStringBackup = DatabaseSetup.ConnString;
                    string newCreatedLocalDbConnString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + path + "\\Database.mdf; Integrated Security=True";
                    DatabaseSetup.ConnString = newCreatedLocalDbConnString;
                    DatabaseSetup.CreateNewDatabase();

                    //Obnovit staré připojení
                    DatabaseSetup.ConnString = connStringBackup;

                    // Zpráva o úspěchu
                    DialogHelper.ShowInfo("Místní databáze byla úspěšně vytvořena a naplněna daty.");
                }
                catch
                {
                    DialogHelper.ShowError("Místní databáze nemohla být vytvořena.");
                }

            }

        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.Reset(); 

                //   LoginWindow.RecreateAppData();
                // this.Close();
                //Properties.Settings.Default.Restarting = true;
                //Properties.Settings.Default.Save();
              
                Application.Current.Shutdown();
              //  System.Windows.Forms.Application.Restart();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
