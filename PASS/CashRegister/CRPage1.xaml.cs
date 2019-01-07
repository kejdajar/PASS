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

namespace PASS.CashRegister
{
    /// <summary>
    /// První okno pokladny - zadávání nových položek.
    /// </summary>
    public partial class CRPage1 : Page
    {
        public CRPage1()
        {
            InitializeComponent();
            InitializeCashRegister();
        }

        // Uživatel právě vyplňuje množství
        private bool _quantityFill = false;

        // Instance nákupního košíku
        public CashRegisterSetup ShoppingSession { get; set; } = null;


        // Pole, které má focus
        private TextBox FocusedTextBox
        {
            get { return numpad._focusedTextBox; }
            set { numpad._focusedTextBox = value; }
        }

        /// <summary>
        /// Tlačítko pro potvrzení zvoleného výrobku a jeho množství.
        /// </summary>        
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (ShoppingSession.CurrentProduct == null)
            {
                int code = 0;
                try
                {
                    code = Convert.ToInt32(tbProductCode.Text);

                }
                catch
                {
                    tbProductCode.Background = Brushes.LightPink;
                    return;
                }


                Product p = StorageSetup.GetProduct(code);
                ShoppingSession.CurrentProduct = p;

                if (p == null)
                {
                    tbProductCode.Background = Brushes.LightPink;
                    return;
                }
                else
                {
                    tbProductCode.ClearValue(TextBox.BackgroundProperty);

                    if (!ShoppingSession.CurrentProduct.priceForUnit)
                        tbProductCode.Text += " - " + StorageSetup.GetProductFullName(ShoppingSession.CurrentProduct);
                    else tbProductCode.Text += " - " + ShoppingSession.CurrentProduct.name.Trim();
                }


            }


            if (!_quantityFill)
            {
                tbQuantity.IsEnabled = true;
                tbProductCode.IsEnabled = false;
                tbQuantity.Focus();
                _quantityFill = true;

                if (ShoppingSession.CurrentProduct.priceForUnit)
                {
                    lblQuantity.Content = "Množství (" + StorageSetup.GetProductUnitName(ShoppingSession.CurrentProduct) + ")";
                }
                else { lblQuantity.Content = "Množství"; }

            }
            else
            {

                decimal quantity = 0;
                try
                {
                    // Při účtování za kus není možné zadat desetinné číslo
                    if (!ShoppingSession.CurrentProduct.priceForUnit && !HelperClass.IsInteger(tbQuantity.Text))
                    {
                        throw new NotImplementedException();
                    }


                    quantity = Convert.ToDecimal(tbQuantity.Text);

                    // Nenulovost a nezápornost množství 
                    if (quantity <= 0)
                    {
                        throw new NotImplementedException();
                    }


                    tbQuantity.ClearValue(TextBox.BackgroundProperty);


                }
                catch
                {
                    tbQuantity.Background = Brushes.LightPink;
                    return;
                }



                decimal? price = quantity * ShoppingSession.CurrentProduct.price;
                ShoppingSession.Sum += price;

                string productDesc = null;
                if (ShoppingSession.CurrentProduct.priceForUnit)
                {
                    productDesc = ShoppingSession.CurrentProduct.name.Trim() + " " + quantity.ToString() + StorageSetup.GetProductUnitName(ShoppingSession.CurrentProduct) + " - " + ShoppingSession.CurrentProduct.price + " Kč/" + StorageSetup.GetProductUnitName(ShoppingSession.CurrentProduct) + " [Celkem: " + price.ToString() + " Kč]";
                }
                else
                {
                    productDesc = StorageSetup.GetProductFullName(ShoppingSession.CurrentProduct) + " - " + ShoppingSession.CurrentProduct.price + " Kč/Kus " + "(" + tbQuantity.Text + "x)" + " [Celkem: " + price.ToString() + " Kč]";
                }
                lbProducts.Items.Add(productDesc);

                ScrollToBottom(lbProducts);

                ShoppingSession.AddItemToShoppingCart(ShoppingSession.CurrentProduct, quantity, productDesc, lbProducts.Items.Count - 1, price, StorageSetup.GetProductUnitName(ShoppingSession.CurrentProduct));


                tbProductCode.Text = "";
                tbQuantity.Text = "";
                ShoppingSession.CurrentProduct = null;
                _quantityFill = false;

                tbQuantity.IsEnabled = false;
                tbProductCode.IsEnabled = true;
                tbProductCode.Focus();
                tbPrice.Text = ShoppingSession.SumString;
                lblQuantity.Content = "Množství";
            }



            SetStornoButton();

        }

        /// <summary>
        /// Posune listbox na konec.
        /// </summary>        
        private void ScrollToBottom(ListBox lbProducts)
        {
            ListBoxAutomationPeer svAutomation = (ListBoxAutomationPeer)ScrollViewerAutomationPeer.CreatePeerForElement(lbProducts);

            if (VisualTreeHelper.GetChildrenCount(lbProducts) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(lbProducts, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }




        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            CRPage2 page2 = new CRPage2(ShoppingSession);
            page2.PreviousPage = this;
            MainWindow.CashRegisterFrame.Navigate(page2);
        }


        /// <summary>
        /// Vymazání jednoho znaku z aktivního textboxu.
        /// </summary>       
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (FocusedTextBox.Text.Length > 0)
            {
                FocusedTextBox.Text = FocusedTextBox.Text.Remove(FocusedTextBox.Text.Length - 1);


                FocusedTextBox.Focus();

                FocusedTextBox.CaretIndex = FocusedTextBox.Text.Length;
            }

        }

        /// <summary>
        /// Událost pro obsluhu číselníku.
        /// </summary>       
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btnSender = sender as Button;
            FocusedTextBox.Text += btnSender.Content;
            FocusedTextBox.Focus();

            FocusedTextBox.CaretIndex = FocusedTextBox.Text.Length;
        }

        private void tbProductCode_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusedTextBox = (TextBox)sender;
        }

        private bool _isReturnFromPage2;
        public bool IsReturnFromPage2
        {
            get { return _isReturnFromPage2; }
            set { _isReturnFromPage2 = value; if (value) { btnStorno_Click(btnStorno, null); } }
        }



        private void InitializeCashRegister()
        {
            if (IsReturnFromPage2)
            {
                btnStorno_Click(btnStorno, null);
                IsReturnFromPage2 = false;
            }
            else
            {
                // Vytvoření objektu nového nákupu
                ShoppingSession = new CashRegisterSetup();
            }

            FocusedTextBox = tbProductCode;
            btnRemove.Visibility = Visibility.Hidden;
            btnStorno.Visibility = Visibility.Hidden;

            // Inicializace user contolu - číselník
            numpad.EventEnterClick += btnEnter_Click;
            numpad.EventClrClick += btnClr_Click;
        }

        private void tbQuantity_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusedTextBox = (TextBox)sender;
        }

        /// <summary>
        /// Vymazat vše.
        /// </summary>       
        private void btnClr_Click(object sender, RoutedEventArgs e)
        {
            tbQuantity.IsEnabled = false;
            tbProductCode.IsEnabled = true;
            lblQuantity.Content = "Množství";
            ShoppingSession.CurrentProduct = null;
            tbProductCode.Text = "";
            tbQuantity.Text = "";
            _quantityFill = false;
            tbProductCode.Focus();
            SetStornoButton();

        }


        private void lbProducts_GotFocus(object sender, RoutedEventArgs e)
        {

            btnRemove.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Odebrat aktuální výrobek.
        /// </summary>        
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            if (lbProducts.SelectedItem != null && (result == MessageBox.Show("Skutečně odstranit zvolenou položku? \n" + lbProducts.SelectedItem.ToString(), "Odstranění položky", MessageBoxButton.YesNo, MessageBoxImage.Question)))
            {
                ShoppingSession.DeleteItemFromShoppingCart(lbProducts.Items.IndexOf(lbProducts.SelectedItem));
                tbPrice.Text = ShoppingSession.SumString;
                lbProducts.Items.RemoveAt(lbProducts.Items.IndexOf(lbProducts.SelectedItem));

                lbProducts.SelectedIndex = -1;
                btnRemove.Visibility = Visibility.Hidden;
                SetStornoButton();

            }
        }

        /// <summary>
        /// Resetovat celý nákup.
        /// </summary>      
        private void btnStorno_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.Yes;
            if (!IsReturnFromPage2)
            {
                result = MessageBox.Show("Skutečně odstranit celý nákup?", "Odstranění nákupu", MessageBoxButton.YesNo, MessageBoxImage.Question);

            }
            else
            {
                IsReturnFromPage2 = false;
            }

            if (result == MessageBoxResult.Yes)
            {
                ResetShoppingCart();
            }

        }

        private void ResetShoppingCart()
        {
            // Nastavení GUI
            tbQuantity.IsEnabled = false;
            tbProductCode.IsEnabled = true;
            lblQuantity.Content = "Množství";
            tbProductCode.Text = "";
            tbQuantity.Text = "";
            _quantityFill = false;
            lbProducts.Items.Clear();
            tbPrice.Text = "";
            tbProductCode.Focus();
            tbProductCode.ClearValue(TextBox.BackgroundProperty);
            tbQuantity.ClearValue(TextBox.BackgroundProperty);

            SetStornoButton();
            btnRemove.Visibility = Visibility.Collapsed;

            // Nová instance nákupního košíku
            ShoppingSession = new CashRegisterSetup();

        }

        private void SetStornoButton()
        {
            if (lbProducts.Items.Count > 0)
            {
                btnStorno.Visibility = Visibility.Visible;
            }
            else
            {
                btnStorno.Visibility = Visibility.Collapsed;
            }
        }

    }

}
