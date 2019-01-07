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


namespace PASS.Management
{
    /// <summary>
    /// Interaction logic for CompanyInfoPage.xaml
    /// </summary>
    public partial class CompanyInfoPage : Page
    {
        public CompanyInfoPage()
        {
            InitializeComponent();
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            Company company = CompanyInfo.GetCompanyInfo();
            tbBillText.Text = BillInfo.GetBillInfo().billText.Trim() ;
            tbCompanyAdress.Text = company.adress.Trim();
            tbCompanyCity.Text = company.city.Trim();
            tbCompanyName.Text = company.name.Trim();
            tbCompanyPhone.Text = company.phone.Trim();
            tbCompanyPostalCode.Text = company.postalCode.ToString();
            tbCompanyWebSite.Text = company.web.Trim();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string companyName = ValidateCompany.Name(tbCompanyName.Text);
                string adress = ValidateCompany.Adress(tbCompanyAdress.Text);
                string city = ValidateCompany.City(tbCompanyCity.Text);
                int? postalCode = ValidateCompany.PostalCode(tbCompanyPostalCode.Text);
                string phone = ValidateCompany.Phone(tbCompanyPhone.Text);
                string web = ValidateCompany.Web(tbCompanyWebSite.Text);

                string billText = ValidateCompany.BillText(tbBillText.Text);

                CompanyInfo.UpdateCompanyInfo(companyName,adress,city,postalCode,phone,web);
                BillInfo.UpdateBillInfo(billText);

                DialogHelper.ShowInfo("Změny byly provedeny.");
                InitializeInterface();
            }
            catch(InvalidCompanyNameException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch (InvalidPostalCodeException ex)
            {
                DialogHelper.ShowWarning(ex.Message);
            }
            catch
            {
                DialogHelper.ShowError("Změny nemohly být provedeny.");
                InitializeInterface();
            }
        }
    }
}
