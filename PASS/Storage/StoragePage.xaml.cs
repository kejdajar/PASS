using PASS.GeneralClasses;
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

namespace PASS.Storage
{
    /// <summary>
    /// Interaction logic for StoragePage.xaml
    /// </summary>
    public partial class StoragePage : Page
    {
        public StoragePage()
        {
            InitializeComponent();
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            StorageSetup.InitializeStorageTable(dgStorage);
        }

        private void btnEditStorage_Click(object sender, RoutedEventArgs e)
        {
            int ID = (int)((Button)sender).CommandParameter;
            ShowStorageEditWindow(ID);
        }

        private void ShowStorageEditWindow(int ID)
        {
            EditProductWindow epw = new EditProductWindow(ID);
            epw.Owner = Window.GetWindow(this);
            epw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            epw.ShowInTaskbar = false;
            epw.ShowDialog();
            StorageSetup.InitializeStorageTable(dgStorage);

        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow apw = new AddProductWindow();
            apw.Owner = Window.GetWindow(this);
            apw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            apw.ShowInTaskbar = false;

            if (StorageSetup.GetNumberOfUnits() == 0)
            {
                DialogHelper.ShowWarning("Podmínkou pro přidání výrobku je existence alespoň jedné jednotky." + Environment.NewLine + "Přidejte jí pomocí Správce jednotek.");
            }
            else
            {
                apw.ShowDialog();
            }

            StorageSetup.InitializeStorageTable(dgStorage);

        }

        private void dgStorage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgStorage.SelectedItem == null) return;
            StorageRecord record = (StorageRecord)dgStorage.SelectedItem;

            ShowStorageEditWindow(record.id);
        }

        private void btnShowUnitManager_Click(object sender, RoutedEventArgs e)
        {
            UnitManagerWindow unitManager = new UnitManagerWindow();
            unitManager.Owner = Window.GetWindow(this);
            unitManager.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            unitManager.ShowInTaskbar = false;
            unitManager.ShowDialog();
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgStorage.SelectedItem == null) return;
            StorageRecord record = (StorageRecord)dgStorage.SelectedItem;

            ShowStorageEditWindow(record.id);
        }

        private void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Skutečně odstranit všechny výrobky?", "Varování", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Storage.StorageSetup.DeleteAllProducts();
                StorageSetup.InitializeStorageTable(dgStorage);
                DialogHelper.ShowInfo("Všechny produkty byly odstraněny.");
            }

        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dgStorage.SelectedItem == null) return;
            StorageRecord record = (StorageRecord)dgStorage.SelectedItem;

            try
            {
                StorageSetup.DeleteProduct(record.id);
                DialogHelper.ShowError("Produkt byl odstraněn");
                StorageSetup.InitializeStorageTable(dgStorage);
            }
            catch
            {
                DialogHelper.ShowError("Produkt nemohl být odstraněn");
            }

        }
    }
}
