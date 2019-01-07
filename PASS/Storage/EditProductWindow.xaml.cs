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
using PASS.Storage;
using PASS.Management;

namespace PASS.Storage
{
    /// <summary>
    /// Okno pro úpravu produktu.
    /// </summary>
    public partial class EditProductWindow : Window
    {
        public EditProductWindow(int productId)
        {
            InitializeComponent();
            this._id = productId;
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Product selectedProduct = (from p in db.Products
                                       where p.id == this._id
                                       select p).Single();
            Unit selectedProductUnit = (from u in db.Units
                                        where u.id == selectedProduct.unit
                                        select u).Single();


            tbName.Text = selectedProduct.name.Trim();
            tbQuantity.Text = selectedProduct.quantity.ToString();
            cbUnit.ItemsSource = StorageSetup.GetUnitNames();
            cbUnit.Text = selectedProductUnit.name.Trim();
            tbUnitQuantity.Text = selectedProduct.unitQuantity.ToString();
            dpExpirationDate.SelectedDate = selectedProduct.expirationDate;
            tbCode.Text = selectedProduct.code.ToString();
            cbPriceForUnit.Text = selectedProduct.priceForUnit == true ? "Ano" : "Ne";
            tbPrice.Text = StorageSetup.GetFormattedMoney(Convert.ToDecimal(selectedProduct.price));
            cbVAT.ItemsSource = StorageSetup.GetVats();
            cbVAT.Text = StorageSetup.GetProductVat(selectedProduct);

            gallery.Images = Images.GetImages(selectedProduct);
            gallery.Update();
            gallery.RemoveImageButton = BtnRemoveImage_Click;
            gallery.AddImageButton = BtnAddImage_Click;

            tbName.TextChanged += Method;
            tbQuantity.TextChanged += Method;
            tbUnitQuantity.TextChanged += Method;
            tbCode.TextChanged += Method;
            tbPrice.TextChanged += Method;
            cbPriceForUnit.SelectionChanged += Method;
            cbUnit.SelectionChanged += Method;
            dpExpirationDate.SelectedDateChanged += Method;
            cbVAT.SelectionChanged += Method;

        }
        

        private int _id;


        private void Method(object sender, TextChangedEventArgs e)
        {

            btnSubmit.IsEnabled = true;
        }
        private void Method(object sender, SelectionChangedEventArgs e)
        {

            btnSubmit.IsEnabled = true;
        }



        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;

            try
            {
                string name = ValidateProduct.Name(tbName.Text);
                int quantity = ValidateProduct.Quantity(tbQuantity.Text);
                string unitName = cbUnit.Text;
                decimal unitQuantity = ValidateProduct.UnitQuantity(tbUnitQuantity.Text);
                DateTime? date = ValidateProduct.Date(dpExpirationDate.SelectedDate);

                decimal price = ValidateProduct.Price(tbPrice.Text);
                price = Convert.ToDecimal(Math.Round(Convert.ToDouble(price), 1, MidpointRounding.AwayFromZero));
                bool priceForUnit = cbPriceForUnit.Text == "Ano" ? true : false;
                char VAT = cbVAT.SelectedItem.ToString()[0];

                Product selectedProduct = (from p in db.Products
                                           where p.id == this._id
                                           select p).Single();

                int code = ValidateProduct.Code(tbCode.Text, selectedProduct.code);


                int uId = (from u in db.Units
                           where u.name == unitName
                           select u.id).Single();

                selectedProduct.name = name;
                selectedProduct.quantity = quantity;
                selectedProduct.unit = uId;
                selectedProduct.unitQuantity = unitQuantity;
                selectedProduct.expirationDate = date;
                selectedProduct.code = code;
                selectedProduct.price = price;
                selectedProduct.priceForUnit = priceForUnit;
                selectedProduct.vatId = VAT;

                db.SubmitChanges();
                DialogHelper.ShowInfo("Produkt byl upraven.");
                btnSubmit.IsEnabled = false;

            }
            catch (InvalidNameException ex)
            {
                DialogHelper.ShowError(ex.Message);
            }
            catch (InvalidQuantityException ex)
            {
                DialogHelper.ShowError(ex.Message);

            }
            catch (InvalidUnitQuantityException ex)
            {
                DialogHelper.ShowError(ex.Message);

            }
            catch (ExistingCodeException ex)
            {

                DialogHelper.ShowError(ex.Message);
            }
            catch (InvalidCodeException ex)
            {
                DialogHelper.ShowError(ex.Message);
            }

            catch (InvalidPriceException ex)
            {
                DialogHelper.ShowError(ex.Message);
            }
            catch (NullDateTimeException ex)
            {
                DialogHelper.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message);
                DialogHelper.ShowError("Některé z polí není správně vyplněno.");
            }            

        }



        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {


            MessageBoxResult result = MessageBox.Show("Opravdu odstranit zvolený produkt (" + StorageSetup.GetProductFullName(StorageSetup.GetProductById(_id)) + ") ?", "Upozornění", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {

                try
                {
                    StorageSetup.DeleteProduct(_id);
                    DialogHelper.ShowInfo("Produkt byl odstraněn.");

                    this.Close();
                }
                catch (Exception ex)
                {
                    Errors.SaveError(ex.Message);
                    DialogHelper.ShowError("Produkt nemohl být odstraněn.");
                }
            }
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

        private void BtnAddImage_Click(object sender, RoutedEventArgs e)
        {

            // Nahrání obrázků do databáze


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

                foreach (ImageStruct istr in imagesAdded)
                {
                    // Práce s databází
                    Images.AddImage(istr);
                    Product p = StorageSetup.GetProductById(_id);
                    ImagesTable imt = Images.FindLastAddedImage();

                    Images.AssignImageToProduct(p, imt);
                }

                // Získání obrázků z databáze - i s ID's
                gallery.Images = Images.GetImages(StorageSetup.GetProductById(_id));
                gallery.Update();
            }
        }

        private void BtnRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            int gonerId = gallery.GetSelectedImageDatabaseID();
            ImagesTable gonerImage = Images.GetImageById(gonerId);

            // odstranit propojení
            Images.RemoveRelationship(gonerId, _id);
            // odstranit obrázek z databáze
            Images.RemoveImage(gonerId);

            //Refresh gallery
            gallery.Images = Images.GetImages(StorageSetup.GetProductById(_id));
            gallery.Update();
        }
    }
}
