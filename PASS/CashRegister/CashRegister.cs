using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using PASS.Management;

namespace PASS.CashRegister
{
    /// <summary>
    /// Třída pro nastavení pokladny.
    /// </summary>
    public class CashRegisterSetup
    {
        /// <summary>
        /// Přidá položku do nákupního košíku.
        /// </summary>       
        public void AddItemToShoppingCart(Product product, decimal quantity, string label, int listBoxId, decimal? totalPrice, string unitName)
        {
            ShoppingCartItem item = new ShoppingCartItem(product, quantity, label, listBoxId, totalPrice, unitName);
            ShoppingCart.Add(item);
        }

        /// <summary>
        /// Odstranění položky z nákupního košíku.
        /// </summary>
        /// <param name="listBoxId">ID položky k odstranění.</param>
        public void DeleteItemFromShoppingCart(int listBoxId)
        {
            // 1. odstranění itemu z kolekce
            ShoppingCartItem goner = null;
            foreach (ShoppingCartItem item in ShoppingCart)
            {
                if (item.ListBoxId == listBoxId)
                {
                    goner = item;
                }
            }

            if (goner != null)
            {
                ShoppingCart.Remove(goner);
                Sum -= goner.TotalPrice;
            }

            // 2. přečíslování kolekce
            int followingItem = listBoxId + 1;
            foreach (ShoppingCartItem item in ShoppingCart)
            {
                if (item.ListBoxId >= followingItem)
                {
                    item.ListBoxId--;
                }
            }
        }

        // Kolik vrátit
        public decimal? Change { get; set; } = 0;
        // Kolik bylo zaplacno
        public decimal Paid { get; set; } = 0;
        // Celková cena nákupu
        public decimal? Sum { get; set; } = 0;

        public string SumString
        {
            get { return Sum.ToString() + " Kč"; }
            set { Sum = Convert.ToDecimal(value); }
        }

        // Aktuálně přidávaný produkt
        public Product CurrentProduct { get; set; } = null;

        /// <summary>
        /// Celý nákupní košík.
        /// </summary>
        public List<ShoppingCartItem> ShoppingCart = new List<ShoppingCartItem>();

        /* 
         *      
         * Účtenka v XML
         *      
         */

        /// <summary>
        /// Vytvoří XML soubor s účtem za nákup.
        /// </summary>
        /// <param name="filename">Název souboru.</param>
        public void CreateXmlBill(string filename, DateTime time,bool saveToDatatabase)
        {
           
            if (ShoppingCart.Count == 0)
            {
                return;
            }

            if (saveToDatatabase)
            SaveXmlBillToDatabase(time);
          
            if(!saveToDatatabase) // pokud zrovna neukládám do DB, tak chci uložit ve formě souboru
            GenerateXmlFile(filename, ShoppingCart,time);

           
        }

        /// <summary>
        /// Vygeneruje z nákupního košíku XML účtenku
        /// </summary>
        public void GenerateXmlFile(string filename, List<ShoppingCartItem> ShoppingCart, DateTime time)
        {
            if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill"));

            }
            using (XmlWriter xmlWriter = XmlWriter.Create(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "Bill", filename)))
            {
                xmlWriter.WriteRaw("<?xml-stylesheet type=\"text/xsl\" href=\"../CashRegister/Bill.xslt\"?>");
                xmlWriter.WriteStartElement("bill");


                foreach (ShoppingCartItem item in ShoppingCart)
                {
                    xmlWriter.WriteStartElement("product");
                    xmlWriter.WriteElementString("name", item.AddedProduct.name.Trim());
                    xmlWriter.WriteElementString("quantity", item.Quantity.ToString());
                    xmlWriter.WriteElementString("unit", item.UnitName.Trim());

                    if (!item.AddedProduct.priceForUnit)
                        xmlWriter.WriteElementString("unitQuantity", item.AddedProduct.unitQuantity.ToString());

                    xmlWriter.WriteElementString("expirationDate", ((DateTime)item.AddedProduct.expirationDate).ToString("dd.MM.yyyy"));
                    xmlWriter.WriteElementString("code", item.AddedProduct.code.ToString());
                    xmlWriter.WriteElementString("totalPrice", item.TotalPrice.ToString());
                    xmlWriter.WriteElementString("priceForUnit", item.AddedProduct.priceForUnit.ToString());
                    xmlWriter.WriteElementString("priceForSingleUnit", item.AddedProduct.price.ToString());
                    xmlWriter.WriteElementString("vatType", item.AddedProduct.vatId.ToString());
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteElementString("totalShoppingCartPrice", Sum.ToString());
                xmlWriter.WriteElementString("paid", Paid.ToString());
                xmlWriter.WriteElementString("change", Change.ToString());
                xmlWriter.WriteElementString("staff", PASS.GeneralClasses.Authentification.AuthUser.Username);
                xmlWriter.WriteElementString("time", time.ToShortDateString() + " " + time.ToShortTimeString());


                //DPH se počítá dohromady pro všechny výrobky dané kategorie (A,B,C,D)
                List<VAT> listOfVat = GetVat();
                decimal? vatSum = GetVatSum(listOfVat);
                decimal? vatSumSingle = GetVatSumSingle(listOfVat);
                xmlWriter.WriteElementString("vatSum", string.Format("{0:0.00}", vatSum));
                xmlWriter.WriteElementString("vatSumSingle", string.Format("{0:0.00}", vatSumSingle));
                foreach (VAT singleVat in listOfVat)
                {

                    xmlWriter.WriteStartElement(singleVat.id.ToString());
                    xmlWriter.WriteAttributeString("percentage", singleVat.percentage);
                    xmlWriter.WriteAttributeString("totalPrice", string.Format("{0:0.00}", singleVat.vatValueProducts));
                    xmlWriter.WriteString(string.Format("{0:0.00}", singleVat.vatValue));
                    xmlWriter.WriteEndElement();
                }

                //Informace do hlavičky
                Company company = CompanyInfo.GetCompanyInfo();
                xmlWriter.WriteElementString("companyName", company.name.Trim()); // Name je vždy vyplněné

                //Další nepovinné údaje, pokud chybí, tak se to do XML nebude přidávat
                if (!string.IsNullOrEmpty(company.adress))
                    xmlWriter.WriteElementString("companyAdress", company.adress.Trim());

                if (!string.IsNullOrEmpty(company.city))
                    xmlWriter.WriteElementString("companyCity", company.city.Trim());

                if (company.postalCode != null)
                    xmlWriter.WriteElementString("companyPostalCode", company.postalCode.ToString());

                if (!string.IsNullOrEmpty(company.phone))
                    xmlWriter.WriteElementString("companyPhone", company.phone.Trim());

                if (!string.IsNullOrEmpty(company.web))
                    xmlWriter.WriteElementString("companyWeb", company.web.Trim());

                Bill billInfo = BillInfo.GetBillInfo();
                if (!string.IsNullOrEmpty(billInfo.billText))
                    xmlWriter.WriteElementString("billText", billInfo.billText.Trim());

                xmlWriter.WriteEndElement();
            }

        }

        /// <summary>
        /// Uloží nezbytné informace o nákupu do databáze, ze kterých je možné
        /// rekonstruovat původní XML. Neukládají se všechny atributy XML, ty, které
        /// je možné vypočítat, se neukládají. 
        /// </summary>
        public void SaveXmlBillToDatabase(DateTime time)
        {
            LinqToSqlDataContext db = PASS.GeneralClasses.DatabaseSetup.Database;

            // Vytvořit nový nákup
            Order order = new Order();
            order.paid = (int)Paid;
            order.staff = PASS.GeneralClasses.Authentification.AuthUser.Username;
            order.timeOfTransaction = time;
            order.change = Change;
            // Na účtence je název firmy. Ten se může změnit a proto archivujeme i starší jména.
            Company company = CompanyInfo.GetCompanyInfo();
            order.companyName = company.name;
            order.companyPhone = company.phone;
            order.companyPostalCode = company.postalCode.ToString();
            order.companyWeb = company.web;
            order.companyCity = company.city;
            order.companyAdress = company.adress;
            order.totalShoppingCartPrice = Sum;
            Bill billInfo = BillInfo.GetBillInfo();
            order.billText = billInfo.billText.Trim();

            db.Orders.InsertOnSubmit(order);
            db.SubmitChanges();
            // VAT
            List<VAT> listOfVat = GetVat();
            decimal? vatSum = GetVatSum(listOfVat);
            decimal? vatSumSingle = GetVatSumSingle(listOfVat);
            order.vatSum = vatSum;
            order.vatSumSingle = vatSumSingle;
            foreach (VAT singleVat in listOfVat)
            {
                OrderItemsVat vatForDb = new OrderItemsVat();
                vatForDb.vatId = singleVat.id;
                vatForDb.orderId = order.id;
                vatForDb.percentageLabel = singleVat.percentage;
                vatForDb.vatValue = singleVat.vatValue;
                vatForDb.vatValueProducts = singleVat.vatValueProducts;
                db.OrderItemsVats.InsertOnSubmit(vatForDb);
            }

             
            db.SubmitChanges();

            // Pro každou položku nákupu vytvořit záznam v DB a přiřadit jí k Orderu
            foreach (ShoppingCartItem item in ShoppingCart)
            {
                OrderItem orderItem = new OrderItem();
                orderItem.name = item.AddedProduct.name.Trim();
                orderItem.quantity = (int)item.Quantity; 
                orderItem.unit = item.UnitName.Trim();
                orderItem.unitQuantity = item.AddedProduct.unitQuantity;
                orderItem.expirationDate = item.AddedProduct.expirationDate;
                orderItem.code = item.AddedProduct.code;
                orderItem.priceForUnit = item.AddedProduct.priceForUnit;
                orderItem.totalPrice = item.TotalPrice;
                orderItem.price = item.AddedProduct.price;
                orderItem.vatId = item.AddedProduct.vatId;

                // Přiřadit k orderu
                orderItem.orderId = order.id;
                db.OrderItems.InsertOnSubmit(orderItem);
            }
            db.SubmitChanges();
        }

        /// <summary>
        /// Při zavolání vždy přepíše Bill.xslt
        /// </summary>
        public static void CopyXsltBillToAppData()
        {
            if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "CashRegister")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "CashRegister"));

            }
            System.IO.File.Copy("CashRegister/Bill.xslt", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PASS", "CashRegister", "Bill.xslt"), true);
        }

      public static decimal? GetVatSum(List<VAT> vats)
        {
            decimal? sum = 0.00m;
            foreach (VAT v in vats)
            {
                sum += v.vatValue;
            }
            return sum;
        }

        public static decimal? GetVatSumSingle(List<VAT> vats)
        {
            decimal? sum = 0.00m;
            foreach (VAT v in vats)
            {
                sum += v.vatValueProducts;
            }
            return sum;
        }

        private List<VAT> GetVat()
        {
            List<VAT> vatList = new List<VAT>();
            // Zjistíme, jaké sazby jsou v systému uloženy
            IEnumerable<Vat> allVats = PASS.Storage.StorageSetup.GetAllVats();
            foreach (Vat v in allVats)
            {
                VAT singleVAT = new VAT(v.id, v.rate + " %", 0.00m, 0.00m);
                vatList.Add(singleVAT);
            }

            foreach (ShoppingCartItem item in ShoppingCart)
            {
                Vat itemDbVat = PASS.Storage.StorageSetup.GetProductVatType(item.AddedProduct);
                decimal? totalPrice = item.TotalPrice;
                decimal? itemDbVatRate = Convert.ToDecimal(itemDbVat.rate);
                decimal? divide = itemDbVatRate / 100.00m;

                decimal? currentVatPrice = totalPrice * divide;


                // Přidáme do celkové kolekce
                foreach (VAT vatFromMainCollection in vatList)
                {
                    if (vatFromMainCollection.id == itemDbVat.id)
                    {
                        vatFromMainCollection.vatValue += currentVatPrice;
                        vatFromMainCollection.vatValueProducts += item.TotalPrice;
                    }
                }

            }
            return vatList;
        }

    }

    public class VAT
    {
        public VAT(char id, string percentage, decimal? vatValue, decimal? vatValueProducts)
        {
            this.id = id;
            this.percentage = percentage;
            this.vatValue = vatValue;
            this.vatValueProducts = vatValueProducts;
        }

        public char id { get; set; }
        public string percentage { get; set; }
        public decimal? vatValue { get; set; } = 0.00m;

        public decimal? vatValueProducts { get; set; } = 0.0m; // kumulativní cena výrobků zatížených tímto DPH
    }

    public class ShoppingCartItem
    {
        public ShoppingCartItem(Product product, decimal quantity, string label, int cartId, decimal? totalPrice, string unitName)
        {
            this.AddedProduct = product;
            this.Quantity = quantity;
            this.Label = label;
            this.ListBoxId = cartId;
            this.TotalPrice = totalPrice;
            this.UnitName = unitName;
        }

        public Product AddedProduct { get; set; }
        public decimal Quantity { get; set; }
        public string Label { get; set; }
        public int ListBoxId { get; set; }
        public decimal? TotalPrice { get; set; }
        public string UnitName { get; set; }

    }

}
