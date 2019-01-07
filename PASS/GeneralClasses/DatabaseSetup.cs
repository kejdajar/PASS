using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using PASS.Storage;
using System.Windows;

namespace PASS.GeneralClasses
{
    /// <summary>
    /// Třída zabezpečující práci s databází.
    /// </summary>   

    static class DatabaseSetup
    {

        static DatabaseSetup()
        {
            InitializeConnectionStrings();
        }

        // Výstupní databáze v provozu ve složce s .exe souborem. Má copy to output directory = always
        // Velmi důležitý je zde timeout na 60s - na pomalejších strojích se totiž často nestihne dogenerovat DB včas
        public static string LocalDbString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)) + "\\PASS\\AppData\\Database.mdf; Integrated Security=True;Connect Timeout=60";

        // Full instalace sql databáze
        public static string FullDbString = "Data Source=" + System.Environment.MachineName + "\\SQLEXPRESS;Initial Catalog=PASS2016;Integrated Security=True";

        // Připojovací řetězec, který se skutečně použije
        public static string ConnString { get; set; }


        public static void RecreateAppData()
        {

            if (Properties.Settings.Default.IsFirstRun)
            {
                try
                {// Při prvním spuštění vytvoří v AppData databázi - LocalDb, ptá se na přepsání
                    DatabaseSetup.CreateLocalDatabaseFile();

                    // Překopírovat Bill.xslt, vše přepisuje
                    CashRegister.CashRegisterSetup.CopyXsltBillToAppData();

                    Properties.Settings.Default.IsFirstRun = false;
                    Properties.Settings.Default.Save();


                }
                catch (Exception ex)
                {
                    DialogHelper.ShowError(ex.Message);
                }


               
            }
        }


        public static void CreateLocalDatabaseFile()
        {


            // Nějaké pozůstatky z předchozí instalace
            string path = (System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS"));
            if (Directory.Exists(path))
            {
                System.Windows.MessageBoxResult result = MessageBox.Show("Na tomto počítači byla nalezena databáze předchozí instalace této aplikace - adresář: " + System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS") + Environment.NewLine + "Pokračováním budou tato data ztracena. Volbou 'ne' budou všechna  data zachována.", "Varování", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    // Smazat všechen obsah složky PASS
                    DirectoryInfo dir = new DirectoryInfo(path);
                    dir.Delete(true);

                    //Vytvořit složku AppData
                    Directory.CreateDirectory(System.IO.Path.Combine(path, "AppData"));

                    // Vytvořit data v adresáři
                    System.IO.File.Copy("AppData/Database.mdf", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "AppData", "Database.mdf"));
                    System.IO.File.Copy("AppData/Database_log.ldf", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "AppData", "Database_log.ldf"));

                    try
                    {
                        DatabaseSetup.SetupDatabase();
                    }
                    catch (Exception ex)
                    {
                        // DialogHelper.ShowWarning("Nová databázová struktura nebyla vytvořena. Původní data zůstala nezměněna.");
                        DialogHelper.ShowError(ex.Message);
                    }

                }

            }
            else // Čistá instalace
            {
                Directory.CreateDirectory(System.IO.Path.Combine(path, "AppData")); // Vytvoří nadsložku PASS a podsložku AppData zároveň
                // Vytvořit data v adresáři
                System.IO.File.Copy("AppData/Database.mdf", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "AppData", "Database.mdf"));
                System.IO.File.Copy("AppData/Database_log.ldf", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "AppData", "Database_log.ldf"));

                try
                {
                    DatabaseSetup.SetupDatabase();
                }
                catch (Exception ex)
                {
                    // DialogHelper.ShowWarning("Nová databázová struktura nebyla vytvořena. Původní data zůstala nezměněna.");
                    DialogHelper.ShowError(ex.Message);
                }
            }

        }

        private static void InitializeConnectionStrings()
        {
            // Nastavení správného connection stringu
            if (Properties.Settings.Default.IsFullDb == "0") // Local db
            {
                if (String.IsNullOrEmpty(Properties.Settings.Default.UserDbFileString))
                {
                    DatabaseSetup.ConnString = LocalDbString;
                    Properties.Settings.Default.UserDbFileString = DatabaseSetup.ConnString;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    DatabaseSetup.ConnString = Properties.Settings.Default.UserDbFileString;
                }
            }
            else //Full MSSQL
            {
                if (String.IsNullOrEmpty(Properties.Settings.Default.UserFullDbString))
                {
                    DatabaseSetup.ConnString = FullDbString;
                    Properties.Settings.Default.UserFullDbString = DatabaseSetup.ConnString;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    DatabaseSetup.ConnString = Properties.Settings.Default.UserFullDbString;
                }

            }
        }

        public static void UseTestDatabase()
        {
            string testDbLocation = DatabaseSetup.GetTestDbLocation();
            DatabaseSetup.ConnString = "Data Source =(LocalDB)\\MSSQLLocalDB; AttachDbFilename =" + testDbLocation + "; Integrated Security = True";
            Properties.Settings.Default.UserDbFileString = DatabaseSetup.ConnString;
            Properties.Settings.Default.Save();
        }


        public static void InitializeLinqToSql()
        {
            Database = new LinqToSqlDataContext(ConnString);
        }

        /// <summary>
        /// Při chybě zůstávají ve frontě pro zápis do DB objekty, které nebyly přidány.
        /// Tato funkce je odstraní.
        /// </summary>
        public static void UndoChanges()
        {
            Database = new LinqToSqlDataContext(ConnString);
        }

        public static LinqToSqlDataContext Database { get; set; }

        public static void CreateUsers()
        {
            string connectionStr = ConnString;
            using (SqlConnection connection = new SqlConnection())
            {

                connection.ConnectionString = connectionStr;
                connection.Open();


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "PASS.SqlScripts.users.sql";

                StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(resourceName));
                string row = "";
                while ((row = sr.ReadLine()) != null)
                {
                    cmd.CommandText = row;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void CreateStorage()
        {
            string connectionStr = ConnString;
            using (SqlConnection connection = new SqlConnection())
            {

                connection.ConnectionString = connectionStr;
                connection.Open();


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "PASS.SqlScripts.storage.sql";

                StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(resourceName));
                string row = "";
                while ((row = sr.ReadLine()) != null)
                {
                    cmd.CommandText = row;
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public static void CreateImages()
        {
            string connectionStr = ConnString;
            using (SqlConnection connection = new SqlConnection())
            {

                connection.ConnectionString = connectionStr;
                connection.Open();


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "PASS.SqlScripts.images.sql";

                StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(resourceName));
                string row = "";
                while ((row = sr.ReadLine()) != null)
                {
                    cmd.CommandText = row;
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public static void CreateCompanyInfo()
        {
            string connectionStr = ConnString;
            using (SqlConnection connection = new SqlConnection())
            {

                connection.ConnectionString = connectionStr;
                connection.Open();


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "PASS.SqlScripts.companyInfo.sql";

                StreamReader sr = new StreamReader(assembly.GetManifestResourceStream(resourceName));
                string row = "";
                while ((row = sr.ReadLine()) != null)
                {
                    cmd.CommandText = row;
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static bool TestDbConnection(string conn)
        {
            string connString = conn;
            LinqToSqlDataContext test = null;
            try
            {
                test = new LinqToSqlDataContext(conn);
                test.Connection.Open();

            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message); return false;
            }
            finally
            {
                if (test != null)
                    test.Connection.Close();
            }
            return true;

        }
        /// <summary>
        /// Vrací absolutní cestu k adresáři s testovací databází (pouze pro vývoj)
        /// </summary>
        /// <returns></returns>
        public static string GetTestDbLocation()
        {
            string currDir = System.Environment.CurrentDirectory;
            string newPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(currDir, @"..\..\"));
            newPath += "AppData\\Database.mdf";
            return newPath;
        }

        //*******************
        // Vytvoření databáze
        //*******************

        public static void SetupDatabase()
        {

            DatabaseSetup.Database = new LinqToSqlDataContext(ConnString);
            DatabaseSetup.CreateNewDatabase();
            Errors.DeleteErrorLog();
        }


        public static void CreateNewDatabase()
        {
            // Vytvoření datové struktury v databázi

            DatabaseSetup.CreateUsers();
            DatabaseSetup.CreateStorage();
            DatabaseSetup.CreateImages();
            DatabaseSetup.CreateCompanyInfo();
            DatabaseSetup.InitializeLinqToSql();
            FillDatabase();
        }

        // Až po vytvoření datové struktury a inicializaci LINQ to SQL
        private static void FillDatabase()
        {
            // Sekce management

            Authentification.NewRole("manažer");
            Authentification.NewRole("skladník");
            Authentification.NewRole("pokladní");

            Authentification.NewUser("manažer", "manažer", "manažer");
            Authentification.NewUser("pokladní", "pokladní", "pokladní");
            Authentification.NewUser("skladník", "skladník", "skladník");

            // VAT (Value Added Tax) = DPH
            StorageSetup.AddVATs();

            // Sekce sklad             
            StorageSetup.AddDemoProducts();

            //Company
            PASS.Management.CompanyInfo.InsertCompanyInfo("Večerka Na Rohu", "Hvězdova 1500", "Pankrác", 14000, "+420 732 251 420", "www.vecerkanarohu.cz");
            PASS.Management.BillInfo.InsertBillInfo("Děkujeme Vám za nákup. Veškeré dotazy zasílejte na adresu info@vecerkanarohu.cz.");
        }


    }
}
