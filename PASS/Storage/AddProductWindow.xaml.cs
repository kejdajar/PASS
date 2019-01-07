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

namespace PASS.Storage
{
    /// <summary>
    /// Okno pro přidání nového produktu do databáze.
    /// </summary>
    public partial class AddProductWindow : Window
    {
        public AddProductWindow()
        {
            InitializeComponent();
            InitializeInterface();
        }

        public bool CloseWindow { get; set; } = false;

        private void InitializeInterface()
        {
            cbPriceForUnit.Text = "Ne";

            LinqToSqlDataContext db = DatabaseSetup.Database;
            IEnumerable<string> units = from u in db.Units
                                        select u.name.Trim();
            cbUnit.ItemsSource = units;
            if (units.Count() > 0)
            {
                cbUnit.Text = units.First();
            }


            cbVAT.ItemsSource = StorageSetup.GetVats();
            if (cbVAT.Items.Count > 0)
                cbVAT.Text = cbVAT.Items[0].ToString();


            btnSubmit.IsEnabled = false;

            // Změna v polích
            tbName.TextChanged += Method;
            tbQuantity.TextChanged += Method;
            tbUnitQuantity.TextChanged += Method;
            tbCode.TextChanged += Method;
            tbPrice.TextChanged += Method;
            cbPriceForUnit.SelectionChanged += Method;
            cbUnit.SelectionChanged += Method;
            dpExpirationDate.SelectedDateChanged += Method;
            cbVAT.SelectionChanged += Method;

            // Gallerie
            gallery.AddImageButton += BtnAddImage_Click;
            gallery.RemoveImageButton += BtnRemoveImage_Click;

        }

        private void BtnAddImage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Obrázky|*.jpg;*.bmp;*.png;*.gif";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] paths = ofd.FileNames;
                Uri[] uris = new Uri[paths.Count()];
                for (int i = 0; i <= paths.Count() - 1; i++)
                {
                    uris[i] = new Uri(paths[i]);
                }

                List<ImageStruct> imagesAdded = new List<ImageStruct>();
                for (int u = 0; u <= uris.Count() - 1; u++)
                {
                    ImageStruct s = new ImageStruct()
                    {
                        imgName = System.IO.Path.GetFileNameWithoutExtension(paths[u]),
                        image = new BitmapImage(uris[u])
                    };
                    imagesAdded.Add(s);
                }

                foreach (ImageStruct imageAdded in imagesAdded)
                {
                    gallery.Images.Add(imageAdded);
                }
                gallery.Update();
            }


        }

        private void BtnRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            int gonerListboxId = gallery.lbContainer.SelectedIndex;
            gallery.Images.RemoveAt(gonerListboxId);
            gallery.Update();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                string name = ValidateProduct.Name(tbName.Text);
                int quantity = ValidateProduct.Quantity(tbQuantity.Text);
                string unitName = cbUnit.Text;
                decimal unitQuantity = ValidateProduct.UnitQuantity(tbUnitQuantity.Text);
                DateTime? date = ValidateProduct.Date(dpExpirationDate.SelectedDate);
                int code = ValidateProduct.Code(tbCode.Text);
                decimal price = ValidateProduct.Price(tbPrice.Text);
                price = Convert.ToDecimal(Math.Round(Convert.ToDouble(price), 1, MidpointRounding.AwayFromZero)); // cena se zaokrouhlí na jedno desetinné místo
                bool priceForUnit = cbPriceForUnit.Text == "Ano" ? true : false;
                char VAT = cbVAT.SelectedItem.ToString()[0];

                LinqToSqlDataContext db = DatabaseSetup.Database;
                int i = (from u in db.Units
                         where u.name == unitName
                         select u.id).Single();
                int unitId = i;

                if (StorageSetup.AddProduct(name, quantity, unitId, unitQuantity, date, code, price, priceForUnit, VAT))
                {
                    // ještě je potřeba uložit do databáze obrázky a asociovat je s produktem
                    Product lastAddedProduct = StorageSetup.GetLastAddedProduct();
                    if (gallery.lbContainer.HasItems)
                    {
                        foreach (ImageStruct istr in gallery.Images)
                        {
                            Images.AddImage(istr);
                            ImagesTable lastAddedImage = Images.FindLastAddedImage();
                            Images.AssignImageToProduct(lastAddedProduct, lastAddedImage);
                        }
                    }

                    DialogHelper.ShowInfo("Produkt přidán.");
                    this.Close();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (InvalidNameException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch (InvalidQuantityException ex)
            {
                DialogHelper.ShowWarning(ex.Message);

            }
            catch (InvalidUnitQuantityException ex)
            {
                DialogHelper.ShowWarning(ex.Message);

            }
            catch (ExistingCodeException ex)
            {

                DialogHelper.ShowWarning(ex.Message);
            }
            catch (InvalidCodeException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }

            catch (InvalidPriceException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch (NullDateTimeException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch
            {
                DialogHelper.ShowError("Produkt nebylo možné přidat.");
            }

        }


        private void Method(object sender, TextChangedEventArgs e)
        {

            btnSubmit.IsEnabled = true;
        }
        private void Method(object sender, SelectionChangedEventArgs e)
        {

            btnSubmit.IsEnabled = true;
        }

        private void cbPriceForUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPriceForUnit.SelectedIndex == 1)
            {
                tbQuantity.IsEnabled = false;
                tbQuantity.Text = "1";
            }
            else
            {
                tbQuantity.IsEnabled = true;
            }
        }
    }
}
