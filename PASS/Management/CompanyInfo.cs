using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PASS.GeneralClasses;

namespace PASS.Management
{
    public static class CompanyInfo
    {
        public static void InsertCompanyInfo(string name, string adress, string city, int postalCode, string phone, string web)
        {

            LinqToSqlDataContext db = DatabaseSetup.Database;
            int numberOfCompanies = (from n in db.Companies
                                     select n).Count();

            if (numberOfCompanies == 0) // Zatím pouze podpora pouze jedné společnosti v DB
            {
                Company company = new Company();
                company.name = name;
                company.adress = adress;
                company.city = city;
                company.postalCode = postalCode;
                company.phone = phone;
                company.web = web;

                db.Companies.InsertOnSubmit(company);
                db.SubmitChanges();
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        public static void UpdateCompanyInfo(string name, string adress, string city, int? postalCode, string phone, string web)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Company company = (from c in db.Companies
                               select c).First();

            company.name = name;
            company.adress = adress;
            company.city = city;
            company.postalCode = postalCode;
            company.phone = phone;
            company.web = web;


            db.SubmitChanges();
        }

        public static Company GetCompanyInfo()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Company company = (from c in db.Companies
                               select c).First();

            return company;
        }

    }

    public static class BillInfo
    {
        public static void InsertBillInfo(string billText)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Bill bill = new Bill();
            bill.id = 1; //Zatím podpora pouze jedné společnosti
            bill.billText = billText;
            db.Bills.InsertOnSubmit(bill);
            db.SubmitChanges();
        }

        public static void UpdateBillInfo(string billText)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Bill bill = (from b in db.Bills
                         select b).First();

            bill.billText = billText;

            db.SubmitChanges();
        }

        public static Bill GetBillInfo()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Bill bill = (from b in db.Bills
                         select b).First();

            return bill;
        }
    }

    public static class ValidateCompany
    {
        public static string Name(string companyName)
        {
            // Prázdný řetězec nebo pouze mezery
            if (string.IsNullOrEmpty(companyName) || (companyName.Length > 0 && companyName.Trim().Length == 0))
            {
                throw new InvalidCompanyNameException();
            }

            return companyName.Trim();
        }


        public static string Adress(string adress)
        {

            return adress.Trim();
        }

        public static string City(string city)
        {

            return city.Trim();
        }

        public static int? PostalCode(string postalCode)
        {

            if (string.IsNullOrEmpty(postalCode))
                return null;

            try
            {
                int code = Convert.ToInt32(postalCode);
                return code;
            }
            catch
            {
                throw new InvalidPostalCodeException();
            }

        }

        public static string Phone(string phone)
        {

            return phone.Trim();
        }

        public static string Web(string web)
        {

            return web.Trim();
        }

        public static string BillText(string billText)
        {
            return billText.Trim();
        }

    }

}
