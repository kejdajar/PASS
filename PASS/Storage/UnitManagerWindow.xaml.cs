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
    /// Interaction logic for UnitManagerWindow.xaml
    /// </summary>
    public partial class UnitManagerWindow : Window
    {
        public UnitManagerWindow()
        {
            InitializeComponent();
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            lbUnits.ItemsSource = StorageSetup.GetUnitNames();
        }

        private void btnUnitAdd_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string unitName = ValidateUnit.Name(tbUnitName.Text.Trim());
                StorageSetup.AddUnit(unitName);

                // Refresh interface
                tbUnitName.Text = string.Empty;
                lbUnits.ItemsSource = StorageSetup.GetUnitNames();
            }
            catch (ExistingUnitNameException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch (UnitAddExcepton ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch (InvalidUnitNameException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch
            {
                DialogHelper.ShowError("Při vkládání jednotky nastala nerozpoznaná chyba.");
            }
        }

        private void btnUnitRemove_Click(object sender, RoutedEventArgs e)
        {
            string unitGonerName = lbUnits.SelectedItem.ToString().Trim();
            try
            {
                StorageSetup.RemoveUnit(unitGonerName);

                //Refresh interface
                lbUnits.ItemsSource = StorageSetup.GetUnitNames();
            }
            catch (UnitInUseException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch
            {
                DialogHelper.ShowError("Jednotka nemohla být odebrána");
            }

        }
    }
}
