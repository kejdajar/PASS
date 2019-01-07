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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PASS.GeneralClasses;
using System.IO;
using PASS.CashRegister;
using System.Diagnostics;
using System.Xml;

namespace PASS.Management
{
    /// <summary>
    /// Interaction logic for SummaryMangPage.xaml
    /// </summary>
    public partial class SummaryMangPage : Page
    {
        public SummaryMangPage()
        {
            InitializeComponent();
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            lbProductsNumber.Content = GetNumberOfProducts().ToString();
            lbUsersNumber.Content = GetNumberOfUsers().ToString();

            FillBillDatagrid();

            if (dgBills.Items.Count <= 0)
            {
                lblMessage.Visibility = Visibility.Visible;
                dgBills.Visibility = Visibility.Collapsed;

            }

        }


        public class BillRecord
        {
            public string FileName { get; set; }
            public string CreationTime { get; set; }

            public string Url { get; set; }
        }

        public void FillBillDatagrid()
        {

            // FillFromFolder(); - původní impelementace
            FillFromDatabase();

        }
        private void FillFromDatabase()
        {
           
            LinqToSqlDataContext db = DatabaseSetup.Database;
            IEnumerable<BillRecord> listOfBills = from order in db.Orders
                                                  select new BillRecord() { FileName = order.id.ToString(), CreationTime = HelperClass.GetFullDatetime(order.timeOfTransaction) };
            dgBills.ItemsSource = listOfBills;
        }

        /// <summary>
        /// ZASTARALÁ METODA: v nové implementaci se již plní daty z databáze, ne ze složky
        /// </summary>
        private void FillFromFolder()
        {
 List<BillRecord> bills = new List<BillRecord>();

            if (Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill")))
            {
                string path = (System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill"));
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                FileInfo[] files = dirInfo.GetFiles("*.xml", SearchOption.AllDirectories);
                foreach (FileInfo s in files)
                {
                    BillRecord singleBill = new BillRecord() { FileName = s.Name, CreationTime = HelperClass.GetFullDatetime(s.CreationTime), Url = s.FullName };
                    bills.Add(singleBill);
                }

                dgBills.ItemsSource = bills;
            }
        }

        private void dgBills_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Je potřeba na základě DB dočasně vytvořit XML účtenku
            CreateXmlBillFromDatabaseSource(((BillRecord)dgBills.SelectedItem).FileName);

            BillRecord record = (BillRecord)dgBills.SelectedItem;
            BillWindow billW = new BillWindow(record.FileName+".xml",true);
            billW.Owner = Window.GetWindow(this);
            billW.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            billW.Show();
        }

       private void CreateXmlBillFromDatabaseSource(string orderId)
        {
            

            // Načtení dat z DB
            LinqToSqlDataContext db = DatabaseSetup.Database;
            int orderIdInt = Convert.ToInt32(orderId);
            // Získat vybraný Order
            Order selectedOrder = (from o in db.Orders
                                  where o.id == orderIdInt
                                  select o).Single();
            // Získat výrobky
            IEnumerable<OrderItem> shoppingCartItemsFromDb =
                                          (from shopItm in db.OrderItems
                                           where shopItm.orderId == orderIdInt
                                           select shopItm) ;

            // Dovýpočet některých hodnot
            /*  decimal? totalPriceOfTransaction = 0;
              foreach(OrderItem item in shoppingCartItemsFromDb)
              {
                  if (item.priceForUnit) totalPriceOfTransaction += item.price * item.quantity;
                  if (!item.priceForUnit) totalPriceOfTransaction += item.price * item.unitQuantity;
              }

              decimal? change = selectedOrder.paid - totalPriceOfTransaction;
              */

            // VAT pro produkty
            IEnumerable<OrderItemsVat> listOfVat = from vat in db.OrderItemsVats
                                                   where vat.orderId == orderIdInt
                                                   select vat;

           

            #region hardcoded účtenka
            if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill"));

            }
            using (XmlWriter xmlWriter = XmlWriter.Create(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill", orderIdInt.ToString()+".xml")))
            {
                xmlWriter.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"../CashRegister/Bill.xslt\"?>");
                xmlWriter.WriteStartElement("bill");


                foreach (OrderItem  item in shoppingCartItemsFromDb)
                {
                    xmlWriter.WriteStartElement("product");
                    xmlWriter.WriteElementString("name", item.name.Trim());
                    xmlWriter.WriteElementString("quantity", item.quantity.ToString());
                    xmlWriter.WriteElementString("unit", item.unit.Trim());

                    if (!item.priceForUnit)
                        xmlWriter.WriteElementString("unitQuantity", item.unitQuantity.ToString());

                    xmlWriter.WriteElementString("expirationDate", ((DateTime)item.expirationDate).ToString("dd.MM.yyyy"));
                    xmlWriter.WriteElementString("code", item.code.ToString());
                    xmlWriter.WriteElementString("totalPrice", item.totalPrice.ToString());
                    xmlWriter.WriteElementString("priceForUnit", item.priceForUnit.ToString());
                    xmlWriter.WriteElementString("priceForSingleUnit", item.price.ToString());
                    xmlWriter.WriteElementString("vatType", item.vatId.ToString());
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteElementString("totalShoppingCartPrice", selectedOrder.totalShoppingCartPrice.ToString());
                xmlWriter.WriteElementString("paid", selectedOrder.paid.ToString());
                xmlWriter.WriteElementString("change", (selectedOrder.change).ToString()); 
                xmlWriter.WriteElementString("staff", selectedOrder.staff.Trim());
                xmlWriter.WriteElementString("time", ((DateTime)selectedOrder.timeOfTransaction).ToShortDateString() + " " + ((DateTime)selectedOrder.timeOfTransaction).ToShortTimeString());


                //DPH se počítá dohromady pro všechny výrobky dané kategorie (A,B,C,D)
               /* List<VAT> listOfVat = GetVat();
                decimal? vatSum = GetVatSum(listOfVat);
                decimal? vatSumSingle = GetVatSumSingle(listOfVat);*/
                xmlWriter.WriteElementString("vatSum", string.Format("{0:0.00}", selectedOrder.vatSum));
                xmlWriter.WriteElementString("vatSumSingle", string.Format("{0:0.00}", selectedOrder.vatSumSingle));
                foreach (OrderItemsVat singleVat in listOfVat)
                {

                    xmlWriter.WriteStartElement(singleVat.vatId.ToString());
                    xmlWriter.WriteAttributeString("percentage", singleVat.percentageLabel.Trim());
                    xmlWriter.WriteAttributeString("totalPrice", string.Format("{0:0.00}", singleVat.vatValueProducts));
                    xmlWriter.WriteString(string.Format("{0:0.00}", singleVat.vatValue));
                    xmlWriter.WriteEndElement();
                } 

                //Informace do hlavičky
               
                xmlWriter.WriteElementString("companyName", selectedOrder.companyName.Trim()); // Name je vždy vyplněné

                //Další nepovinné údaje, pokud chybí, tak se to do XML nebude přidávat
                if (!string.IsNullOrEmpty(selectedOrder.companyAdress))
                    xmlWriter.WriteElementString("companyAdress", selectedOrder.companyAdress.Trim());

                if (!string.IsNullOrEmpty(selectedOrder.companyCity))
                    xmlWriter.WriteElementString("companyCity", selectedOrder.companyCity.Trim());

                if (selectedOrder.companyPostalCode != null)
                    xmlWriter.WriteElementString("companyPostalCode", selectedOrder.companyPostalCode.ToString().Trim());

                if (!string.IsNullOrEmpty(selectedOrder.companyPhone))
                    xmlWriter.WriteElementString("companyPhone", selectedOrder.companyPhone.Trim());

                if (!string.IsNullOrEmpty(selectedOrder.companyWeb))
                    xmlWriter.WriteElementString("companyWeb", selectedOrder.companyWeb.Trim());

                Bill billInfo = BillInfo.GetBillInfo();
                if (!string.IsNullOrEmpty(billInfo.billText))
                    xmlWriter.WriteElementString("billText", billInfo.billText.Trim());

                xmlWriter.WriteEndElement();
            }
            #endregion

        }


        private int GetNumberOfProducts()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            int count = (from item in db.Products
                         select item).Count();
            return count;
        }

        private int GetNumberOfUsers()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            int count = (from item in db.Users
                         select item).Count();
            return count;
        }

        private void btnBillDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            int selectedBillId = Convert.ToInt32((((BillRecord)dgBills.SelectedItem).FileName).Trim());
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Order orderToDelete = (from o in db.Orders
                                   where o.id == selectedBillId
                                   select o).Single();
            db.Orders.DeleteOnSubmit(orderToDelete);
            db.SubmitChanges();
            InitializeInterface();
            }
            catch
            {
                DialogHelper.ShowError("Zvolenou účtenku nebylo možné odstranit.");
            }
        }

        private void btnDeleteAllBill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                LinqToSqlDataContext db = DatabaseSetup.Database;
                var ordersToDelete = from o in db.Orders                                       
                                    select o;

                db.Orders.DeleteAllOnSubmit(ordersToDelete);
                db.SubmitChanges();
                InitializeInterface();
            }
            catch
            {
                DialogHelper.ShowError("Zvolenou operaci nebylo možné dokončit.");
            }

        }

        private void btnSaveBill_Click(object sender, RoutedEventArgs e)
        {
            /* try
             {
                 Process.Start(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill"));
             }
             catch
             {
                 DialogHelper.ShowError("Umístění nemohlo být otevřeno.");
             } */
            string xmlBillName = (((BillRecord)dgBills.SelectedItem).FileName).Trim();
           
            System.Windows.Forms.SaveFileDialog fb = new System.Windows.Forms.SaveFileDialog();
             fb.FileName = "uctenka "+xmlBillName + ".xml";
            
            fb.Filter = "XML soubor (*.xml)|*.xml";
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = fb.FileName;
                try
                {
                    CreateXmlBillFromDatabaseSource(xmlBillName);
                    string tempBillLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill", xmlBillName + ".xml");
                    // Překopíruje prázdnou databázi do zvoleného adresáře
                    System.IO.File.Copy(tempBillLocation,path);
                    File.Delete(tempBillLocation);

                } catch {
                    DialogHelper.ShowError("Při exportu účtenky se vyskytla chyba.");
                }
                }
        }
    }
}
