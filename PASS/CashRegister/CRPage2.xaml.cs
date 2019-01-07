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

namespace PASS.CashRegister
{
    /// <summary>
    /// Druhé okno pokladny - dokončení nákupu
    /// </summary>
    public partial class CRPage2 : UserControl
    {
        public CRPage2(CashRegisterSetup shoppingSession)
        {
            InitializeComponent();
            this._shoppingSession = shoppingSession;
            InitializePage2();
        }

        private CashRegisterSetup _shoppingSession;

        private void InitializePage2()
        {
            numpad.EventClrClick = btnClr_Click;
            numpad.EventEnterClick = btnEnter_Click;
            tbPricePay.Text = _shoppingSession.Sum.ToString() + " Kč";
            _focusedTextBox = tbPaid;
            tbPaid.Focus();
            btnShowBill.IsEnabled = false;
            btnSaveAndExit.IsEnabled = false;

            DateTime time = DateTime.Now;
            this._billFilename = time.ToString("dd.MM.yy.hh.mm.ss") + ".xml";
            this._billTime = time;
        }

        private string _billFilename = string.Empty;
        private DateTime _billTime;

        // Pole, které má focus
        private TextBox _focusedTextBox
        {
            get { return numpad._focusedTextBox; }
            set { numpad._focusedTextBox = value; }
        }


        private void btnClr_Click(object sender, RoutedEventArgs e)
        {
            tbChange.Text = string.Empty;
            tbPaid.Text = string.Empty;
            _focusedTextBox = tbPaid;
            tbPaid.IsEnabled = true;
            tbPaid.Focus();
            numpad.btnDel.IsEnabled = true;
            numpad.btnEnter.IsEnabled = true;
            btnShowBill.IsEnabled = false;
            btnSaveAndExit.IsEnabled = false;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            int paid = 0;
            decimal? change = 0;
            decimal? sum = 0;

            try
            {
                sum = Convert.ToDecimal(Math.Round(Convert.ToDouble(_shoppingSession.Sum), 0, MidpointRounding.AwayFromZero)); // cenu celeho nakupu zaokrouhlit na cele cislo
                paid = Convert.ToInt32(tbPaid.Text);
                change = Convert.ToDecimal(Math.Round(Convert.ToDouble(paid - sum), 0, MidpointRounding.AwayFromZero)); // vysledek zaokrouhlit na cele cislo
            }
            catch
            {
                tbPaid.Background = Brushes.LightPink;
                return;
            }

            if (change >= 0)
            {
                tbPaid.ClearValue(TextBox.BackgroundProperty);
                _shoppingSession.Change = change;
                _shoppingSession.Paid = paid + 0.00m;

                change += 0.00m;
                tbChange.Text = change.ToString() + " Kč";

                tbPaid.Text = _shoppingSession.Paid + " Kč";
                tbPaid.IsEnabled = false;
                numpad.btnDel.IsEnabled = false;
                numpad.btnEnter.IsEnabled = false;
                _focusedTextBox = null;
                btnShowBill.IsEnabled = true;
                btnSaveAndExit.IsEnabled = true;
            }
            else
            {
                tbPaid.Background = Brushes.LightPink;
                return;
            }

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CashRegisterFrame.GoBack();
        }

        private void btnSaveAndExit_Click(object sender, RoutedEventArgs e)
        {
            // vytvořit účtenku - ta z náhledu je pouze dočasná
            _shoppingSession.CreateXmlBill(_billFilename, _billTime,true);

            List<ShoppingCartItem> ShoppingCart = _shoppingSession.ShoppingCart;
            foreach (ShoppingCartItem item in ShoppingCart)
            {

                if (!item.AddedProduct.priceForUnit)
                {
                    item.AddedProduct.quantity -= Convert.ToInt32(item.Quantity);
                    PASS.GeneralClasses.DatabaseSetup.Database.SubmitChanges();
                }
                else
                {
                    item.AddedProduct.unitQuantity -= item.Quantity;
                    PASS.GeneralClasses.DatabaseSetup.Database.SubmitChanges();
                }

                MainWindow.CashRegisterFrame.Navigate(PreviousPage);
                PreviousPage.IsReturnFromPage2 = true;
            }

            IsBillCreated = false;
        }

        public CRPage1 PreviousPage { get; set; } = null;

        public static bool IsBillCreated { get; set; } = false;
        private void btnShowBill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // param. false - zde se ještě do DB nic neukládá, je to jenom náhled
                _shoppingSession.CreateXmlBill(_billFilename, _billTime,false);
                IsBillCreated = true;

                BillWindow billW = new BillWindow(_billFilename, true);
                billW.Owner = MainWindow.SelfReference;
                billW.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                billW.ShowInTaskbar = false;
                billW.ShowDialog();
            }
            catch (Exception ex)
            {
                PASS.GeneralClasses.DialogHelper.ShowError(ex.Message);
            }

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.PrintDialog pd = new System.Windows.Controls.PrintDialog();
            pd.ShowDialog();
        }
    }
}
