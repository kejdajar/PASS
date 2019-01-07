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
using System.IO;


namespace PASS.CashRegister
{
    /// <summary>
    /// Okno náhledu na účtenku.
    /// </summary>
    public partial class BillWindow : Window
    {
        public BillWindow(string xmlBillFileName,bool onCloseDeleteCurrentBill = false)
        {
            this._onCloseDeleteCurrentBill = onCloseDeleteCurrentBill;
            InitializeComponent();
            InitializeWindow(xmlBillFileName);
        }

        private string _xmlBillFilename = string.Empty;
        private bool _onCloseDeleteCurrentBill = false;
        private void InitializeWindow(string xmlBillFileName)
        {
            this._xmlBillFilename = xmlBillFileName;
            string url = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill", xmlBillFileName);
            Uri uri = new Uri(url);
            browser.Navigate(uri);
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            mshtml.IHTMLDocument2 doc = browser.Document as mshtml.IHTMLDocument2;
            doc.execCommand("Print", true, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Po uzavření okna účtenku smazat
            if (_onCloseDeleteCurrentBill)
            {
                string url = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill", _xmlBillFilename);
                File.Delete(url);
            }
            
        }
    }
}
