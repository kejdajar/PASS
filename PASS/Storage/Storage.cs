using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PASS.GeneralClasses;
using System.ComponentModel;


namespace PASS.Storage
{
    /// <summary>
    /// Třída pro nastavení skladu.
    /// </summary>  
    static class StorageSetup
    {
       
        public static void AddVATs()
        {
            AddVAT('A', 21);
            AddVAT('B', 15);
            AddVAT('C', 10);
            AddVAT('D', 0);
        }

        public static void AddVAT(char id, int rate)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Vat vat = new Vat();
            vat.id = id;
            vat.rate = rate;
            db.Vats.InsertOnSubmit(vat);
            db.SubmitChanges();
        }

        public static IEnumerable<Vat> GetAllVats()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            IEnumerable<Vat> vats = from v in db.Vats
                                    select v;
            return vats;

        }

        public static List<string> GetVats()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            List<string> vats = GetFormattedVats(from v in db.Vats
                                                 select v);

            return vats;

        }

        public static int GetNumberOfUnits()
        {

            LinqToSqlDataContext db = DatabaseSetup.Database;
            int count = (from u in db.Units
                         select u).Count();

            return count;

        }

        private static List<string> GetFormattedVats(IEnumerable<Vat> vats)
        {
            List<string> formattedVats = new List<string>();
            foreach (Vat v in vats)
            {
                formattedVats.Add(v.id + " - " + v.rate + " %");
            }
            return formattedVats;
        }

        private static string GetFormattedVat(Vat vat)
        {

            return vat.id + " - " + vat.rate + " %";
        }

        public static string GetProductVat(Product product)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Vat vat = (from p in db.Products
                       where p == product
                       join v in db.Vats
                       on p.vatId equals v.id
                       select v).Single();

            return GetFormattedVat(vat);

        }

        public static Vat GetProductVatType(Product product)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Vat vat = (from p in db.Products
                       where p == product
                       join v in db.Vats
                       on p.vatId equals v.id
                       select v).Single();

            return vat;

        }

        public static void DeleteAllProducts()
        {
            // Tabulka ProductImages má: ON DELETE CASCADE, ale 
            // obrázky v tabulce ImagesTable zůstanou --> je třeba je smazat 

            LinqToSqlDataContext db = DatabaseSetup.Database;

            // Mazání všech přiřazených obrázků
            List<ImagesTable> imagesToDelete = new List<ImagesTable>();
            IEnumerable<ImagesTable> allImages = from i in db.ImagesTables
                                                 join pi in db.ProductImages
                                                 on i.id equals pi.idImage
                                                 select i;
            db.ImagesTables.DeleteAllOnSubmit(allImages);
            db.SubmitChanges();

            // Mazání produktů
            IEnumerable<Product> allProducts = from p in db.Products
                                               select p;

            db.Products.DeleteAllOnSubmit(allProducts);
            db.SubmitChanges();
        }


        public static void DeleteProduct(int productId)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Product selectedProduct = (from p in db.Products
                                       where p.id == productId
                                       select p).Single();
            //Smazání obrazků
            //Nejdříve najdeme propojovací záznamy pro daný produkt

            IEnumerable<ProductImage> joinTable = from j in db.ProductImages
                                                  where j.idProduct == productId
                                                  select j;
            // Výsledek spojíme s uloženými obrázky, výsledkem je kolekce obrázků ke smazání
            IEnumerable<ImagesTable> imagesToDelete = from j in joinTable
                                                      join img in db.ImagesTables
                                                      on j.idImage equals img.id
                                                      select img;
            // Smazání
            db.ImagesTables.DeleteAllOnSubmit(imagesToDelete);
            db.Products.DeleteOnSubmit(selectedProduct);

            try
            {
                db.SubmitChanges();
            }
            catch
            {
                throw new NotImplementedException();
            }

        }

        /// <summary>
        /// Přidá testovací produkt do databáze.
        /// </summary>
        public static void AddDemoProducts()
        {


            StorageSetup.AddUnit("g"); // Kvůli cizímu klíči v Db

            LinqToSqlDataContext db = DatabaseSetup.Database;

            // DEMO PRODUCT1 *************************************

            Product demoProduct = new Product();
            demoProduct.name = "Rohlík tukový";
            demoProduct.quantity = 100;
            demoProduct.expirationDate = DateTime.Now.AddDays(3);
            demoProduct.code = 1;
            demoProduct.price = 1.50m;
            demoProduct.unit = (from u in db.Units where u.name.Trim() == "g" select u.id).Single();
            demoProduct.unitQuantity = 43.00m;
            demoProduct.priceForUnit = false;
            demoProduct.vatId = 'B';


            db.Products.InsertOnSubmit(demoProduct);

            // product1 images
            Images.AddImage("PASS.Images.roll.png", "Rohlík tukový obr.1");
            Images.AddImage("PASS.Images.roll2.png", "Rohlík tukový obr.2");


            // DEMO PRODUCT2 *************************************
            StorageSetup.AddUnit("kg"); // Kvůli cizímu klíči v Db


            Product demoProduct2 = new Product();
            demoProduct2.name = "Jablka červená";
            demoProduct2.quantity = 1;
            demoProduct2.expirationDate = DateTime.Now.AddDays(30);
            demoProduct2.code = 2;
            demoProduct2.price = 17.90m;
            demoProduct2.unit = (from u in db.Units where u.name.Trim() == "kg" select u.id).Single();
            demoProduct2.unitQuantity = 80.00m;
            demoProduct2.priceForUnit = true;
            demoProduct2.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct2);

            // Product 2 images
            Images.AddImage("PASS.Images.apple.png", "Jablko červené");



            db.SubmitChanges();

            Images.AssignImageToProduct(1, 1);
            Images.AssignImageToProduct(1, 2);
            Images.AssignImageToProduct(2, 3);

            //Demoproduct 3
            StorageSetup.AddUnit("l"); // Kvůli cizímu klíči v Db


            Product demoProduct3 = new Product();
            demoProduct3.name = "Mléko plnotučné trvanlivé";
            demoProduct3.quantity = 20;
            demoProduct3.expirationDate = DateTime.Now.AddMonths(2);
            demoProduct3.code = 3;
            demoProduct3.price = 19.90m;
            demoProduct3.unit = (from u in db.Units where u.name.Trim() == "l" select u.id).Single();
            demoProduct3.unitQuantity = 1.00m;
            demoProduct3.priceForUnit = false;
            demoProduct3.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct3);

            // Product 3 images
            Images.AddImage("PASS.Images.milk.png", "Mléko plnotučné trvanlivé");


            db.SubmitChanges();

            Images.AssignImageToProduct(3, 4);

            //Demoproduct 4
            
            Product demoProduct4 = new Product();
            demoProduct4.name = "Bramborový salát vážený";
            demoProduct4.quantity = 1;
            demoProduct4.expirationDate = DateTime.Now.AddMonths(1);
            demoProduct4.code = 4;
            demoProduct4.price = 70.0m;
            demoProduct4.unit = (from u in db.Units where u.name.Trim() == "kg" select u.id).Single();
            demoProduct4.unitQuantity = 8.00m;
            demoProduct4.priceForUnit = true;
            demoProduct4.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct4);

            // Product 4 images
            Images.AddImage("PASS.Images.salad.png", "Bramborový salát");


            db.SubmitChanges();

            Images.AssignImageToProduct(4, 5);

            //Demoproduct 5

            Product demoProduct5 = new Product();
            demoProduct5.name = "Chléb konzumní";
            demoProduct5.quantity = 7;
            demoProduct5.expirationDate = DateTime.Now.AddDays(3);
            demoProduct5.code = 5;
            demoProduct5.price = 23.90m;
            demoProduct5.unit = (from u in db.Units where u.name.Trim() == "g" select u.id).Single();
            demoProduct5.unitQuantity = 1200.00m;
            demoProduct5.priceForUnit = false;
            demoProduct5.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct5);

            // Product 5 images
            Images.AddImage("PASS.Images.bread.png", "Chléb konzumní");

            db.SubmitChanges();

            Images.AssignImageToProduct(5, 6);

            //Demoproduct 6

            Product demoProduct6 = new Product();
            demoProduct6.name = "Magnesia Perlivá přírodní minerální voda";
            demoProduct6.quantity = 15;
            demoProduct6.expirationDate = DateTime.Now.AddMonths(12);
            demoProduct6.code = 6;
            demoProduct6.price = 23.90m;
            demoProduct6.unit = (from u in db.Units where u.name.Trim() == "l" select u.id).Single();
            demoProduct6.unitQuantity = 1.50m;
            demoProduct6.priceForUnit = false;
            demoProduct6.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct6);

            // Product 6 images
            Images.AddImage("PASS.Images.magnesia.png", "Magnesia");


            db.SubmitChanges();

            Images.AssignImageToProduct(6, 7);

            //Demoproduct 7

            Product demoProduct7 = new Product();
            demoProduct7.name = "Bramborové hranolky";
            demoProduct7.quantity = 9;
            demoProduct7.expirationDate = DateTime.Now.AddMonths(24);
            demoProduct7.code = 7;
            demoProduct7.price = 21.90m;
            demoProduct7.unit = (from u in db.Units where u.name.Trim() == "kg" select u.id).Single();
            demoProduct7.unitQuantity = 1.00m;
            demoProduct7.priceForUnit = false;
            demoProduct7.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct7);

            // Product 7 images
            Images.AddImage("PASS.Images.chips.png", "Hranolky");


            db.SubmitChanges();

            Images.AssignImageToProduct(7, 8);

            //Demoproduct 8

            Product demoProduct8 = new Product();
            demoProduct8.name = "Pizza Ristorante Mozzarella";
            demoProduct8.quantity = 12;
            demoProduct8.expirationDate = DateTime.Now.AddMonths(24);
            demoProduct8.code = 8;
            demoProduct8.price = 64.90m;
            demoProduct8.unit = (from u in db.Units where u.name.Trim() == "g" select u.id).Single();
            demoProduct8.unitQuantity = 330.00m;
            demoProduct8.priceForUnit = false;
            demoProduct8.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct8);

            // Product 8 images
            Images.AddImage("PASS.Images.pizza.png", "Pizza");


            db.SubmitChanges();

            Images.AssignImageToProduct(8, 9);

            //Demoproduct 9

            Product demoProduct9 = new Product();
            demoProduct9.name = "Jacobs Velvet rozpustná 100% káva";
            demoProduct9.quantity = 8;
            demoProduct9.expirationDate = DateTime.Now.AddMonths(24);
            demoProduct9.code = 9;
            demoProduct9.price = 150.00m;
            demoProduct9.unit = (from u in db.Units where u.name.Trim() == "g" select u.id).Single();
            demoProduct9.unitQuantity = 200.00m;
            demoProduct9.priceForUnit = false;
            demoProduct9.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct9);

            // Product 9 images
            Images.AddImage("PASS.Images.coffee.png", "Jacobs Velvet káva");


            db.SubmitChanges();

            Images.AssignImageToProduct(9, 10);

            //Demoproduct 10

            Product demoProduct10 = new Product();
            demoProduct10.name = "Jihočeský jogurt jahoda";
            demoProduct10.quantity = 10;
            demoProduct10.expirationDate = DateTime.Now.AddMonths(6);
            demoProduct10.code = 10;
            demoProduct10.price = 19.90m;
            demoProduct10.unit = (from u in db.Units where u.name.Trim() == "g" select u.id).Single();
            demoProduct10.unitQuantity = 200.00m;
            demoProduct10.priceForUnit = false;
            demoProduct10.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct10);

            // Product 10 images
            Images.AddImage("PASS.Images.yoghurt.png", "Jogurt jahoda");


            db.SubmitChanges();

            Images.AssignImageToProduct(10, 11);

            //Demoproduct 11

            Product demoProduct11 = new Product();
            demoProduct11.name = "Pilsner Urquell Pivo ležák světlý";
            demoProduct11.quantity = 40;
            demoProduct11.expirationDate = DateTime.Now.AddMonths(12);
            demoProduct11.code = 11;
            demoProduct11.price = 23.90m;
            demoProduct11.unit = (from u in db.Units where u.name.Trim() == "l" select u.id).Single();
            demoProduct11.unitQuantity = 0.50m;
            demoProduct11.priceForUnit = false;
            demoProduct11.vatId = 'A';

            db.Products.InsertOnSubmit(demoProduct11);

            // Product 11 images
            Images.AddImage("PASS.Images.pilsner.png", "Pilsner Urquell");


            db.SubmitChanges();

            Images.AssignImageToProduct(11, 12);


            //Demoproduct 12

            Product demoProduct12 = new Product();
            demoProduct12.name = "Předměřická mouka pšeničná";
            demoProduct12.quantity = 50;
            demoProduct12.expirationDate = DateTime.Now.AddMonths(24);
            demoProduct12.code = 12;
            demoProduct12.price = 9.90m;
            demoProduct12.unit = (from u in db.Units where u.name.Trim() == "kg" select u.id).Single();
            demoProduct12.unitQuantity = 1.00m;
            demoProduct12.priceForUnit = false;
            demoProduct12.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct12);

            // Product 12 images
            Images.AddImage("PASS.Images.flour.png", "Mouka pšeničná");


            db.SubmitChanges();

            Images.AssignImageToProduct(12, 13);

            //Demoproduct 13

            Product demoProduct13 = new Product();
            demoProduct13.name = "Pickwick Zelený čaj mango a jasmín";
            demoProduct13.quantity = 30;
            demoProduct13.expirationDate = DateTime.Now.AddMonths(12);
            demoProduct13.code = 13;
            demoProduct13.price = 45.90m;
            demoProduct13.unit = (from u in db.Units where u.name.Trim() == "g" select u.id).Single();
            demoProduct13.unitQuantity = 30.00m;
            demoProduct13.priceForUnit = false;
            demoProduct13.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct13);

            // Product 13 images
            Images.AddImage("PASS.Images.pickwick.png", "Pickwick Zelený čaj mango a jasmín");


            db.SubmitChanges();

            Images.AssignImageToProduct(13, 14);

            //Demoproduct 14
            AddUnit("ml");
            Product demoProduct14 = new Product();
            demoProduct14.name = "Míša Jahodový ";
            demoProduct14.quantity = 20;
            demoProduct14.expirationDate = DateTime.Now.AddMonths(12);
            demoProduct14.code = 14;
            demoProduct14.price = 8.90m;
            demoProduct14.unit = (from u in db.Units where u.name.Trim() == "ml" select u.id).Single();
            demoProduct14.unitQuantity = 45.00m;
            demoProduct14.priceForUnit = false;
            demoProduct14.vatId = 'B';

            db.Products.InsertOnSubmit(demoProduct14);

            // Product 13 images
            Images.AddImage("PASS.Images.misa.png", "Míša Jahodový");

            db.SubmitChanges();

            Images.AssignImageToProduct(14, 15);



        }

        /// <summary>
        /// Pokusí se přidat novou jednotku. V případě neúspěchu vrací false.
        /// </summary>
        /// <param name="name">Jméno nové jednotky</param> 
        /// 
        public static void AddUnit(string name)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;

            // Kontrola unikátnosti jména
            IEnumerable<string> allNames = StorageSetup.GetUnitNames();
            foreach (string n in allNames)
            {
                if (n == name)
                    throw new ExistingUnitNameException();

            }

            Unit u = new Unit();
            u.name = name;
            db.Units.InsertOnSubmit(u);

            try
            {
                db.SubmitChanges();

            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message);
                throw new UnitAddExcepton();
            }
        }

        /// <summary>
        /// Odebere danou jednotku.
        /// </summary>
        /// <param name="unitName">Jméno jednotky k odebrání. </param>   
        public static void RemoveUnit(string unitName)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;            
            Unit unitToDelete = (from goner in db.Units
                                 where goner.name == unitName
                                 select goner).Single();

            // Zkontrolovat, jestli nějaký produkt tuto jednotku nepoužívá
            int unitGonerId = unitToDelete.id;
            IEnumerable<int?> productUnitIds = from p in db.Products
                                               select p.unit;
            foreach (int? singleId in productUnitIds)
            {
                if (singleId == unitGonerId)
                {
                    throw new UnitInUseException();
                }
            }



            db.Units.DeleteOnSubmit(unitToDelete);
            db.SubmitChanges();

        }

        /// <summary>
        /// Vytvoří nový produkt a přidá ho do databáze.
        /// </summary>
        /// <param name="name">Jméno produktu. Nemusí být unikátní.</param>
        /// <param name="quantity">Množství (počet kusů)</param>
        /// <param name="unitId">ID jednotky</param>
        /// <param name="unitQuantity">Množství zvolené jednotky</param>
        /// <param name="expirationDate">Datum expirace</param>
        /// <param name="code">Kód zboží</param>
        /// <param name="price">Cena za kus, popřípadě za jednoku množství</param>
        /// <param name="priceForUnit">Cena se účtuje za jednotku množství, ne za počet kusů.</param>
        /// <returns></returns>
        public static bool AddProduct(string name, int quantity, int unitId, decimal unitQuantity, DateTime? expirationDate, int code, decimal price, bool priceForUnit, char VAT)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Product product = new Product()
            {
                name = name,
                quantity = quantity,
                unit = unitId,
                unitQuantity = unitQuantity,
                expirationDate = expirationDate,
                code = code,
                price = price,
                priceForUnit = priceForUnit,
                vatId = VAT

            };

            db.Products.InsertOnSubmit(product);

            try
            {
                db.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                Errors.SaveError(ex.Message);
                return false;
            }

        }


        /// <summary>
        /// Přidá nový produkt do databáze
        /// </summary>
        /// <param name="product">Produkt pro přidání.</param>
        public static void AddProduct(Product product)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            db.Products.InsertOnSubmit(product);
            db.SubmitChanges();
        }

        public static Product GetLastAddedProduct()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Product lastAdded = (from last in db.Products
                                 select last).OrderByDescending(last => last.id).First();
            return lastAdded;

        }

        public static void InitializeStorageTable(DataGrid dataGrid)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            IEnumerable<StorageRecord> products = from p in db.Products
                                                  join u in db.Units
                                                  on p.unit equals u.id
                                                  select new StorageRecord
                                                  {
                                                      id = p.id,
                                                      name = StorageSetup.GetProductFullName(p),
                                                      quantity = p.quantity,
                                                      unit = u.name,
                                                      unitQuantity = p.unitQuantity,
                                                      expirationDate = GetFormattedString(p.expirationDate),
                                                      price = GetFormattedMoneyCrowns(Convert.ToDecimal(p.price), p.priceForUnit, u.name),
                                                      priceForUnit = p.priceForUnit == true ? "Ano" : "Ne",
                                                      code = p.code,
                                                  };



            dataGrid.ItemsSource = products;

        }

        public static string GetFormattedString(DateTime? date)
        {
            try
            {
                return ((DateTime)date).ToString("dd.MM.yyyy");
            }
            catch
            {
                return "";
            }
        }

        public static string GetFormattedMoney(decimal money)
        {
            return string.Format("{0:0.00}", money);
        }

        public static string GetFormattedMoneyCrowns(decimal money, bool priceForUnit, string unitName)
        {

            string price = string.Format("{0:0.00}", money) + " Kč";

            if (!priceForUnit)
            {
                price += "/Kus";
            }
            else
            {
                price += "/" + unitName.Trim();
            }
            return price;

        }

        /// <summary>
        /// Vrací produkt, jinak null.
        /// </summary>
        /// <param name="code">Kód produktu</param>
        /// <returns>Vrací produkt</returns>
        public static Product GetProduct(int code)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            try
            {
                Product product = (from p in db.Products
                                   where p.code == code
                                   select p).Single();
                return product;
            }
            catch
            {
                return null;
            }

        }

        public static Product GetProductById(int id)
        {
            return (from pr in DatabaseSetup.Database.Products where pr.id == id select pr).Single();
        }

        /// <summary>
        /// Vrací string ve tvaru productName_quantity_unitName.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static string GetProductFullName(Product product)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Unit unit = (from u in db.Units
                         where u.id == product.unit
                         select u).Single();
            return product.name.Trim() + " " + product.unitQuantity.ToString().Trim() + unit.name.Trim();
        }

        /// <summary>
        /// Vrací název jednotky, ve které je produkt veden.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static string GetProductUnitName(Product product)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            Unit unit = (from u in db.Units
                         where u.id == product.unit
                         select u).Single();
            return unit.name.Trim();
        }

        public static IEnumerable<string> GetUnitNames()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            var names = from i in db.Units
                        select i.name.Trim();

            return names;
        }

        public static IEnumerable<int> GetCodes()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            IEnumerable<int> codes = from c in db.Products
                                     select c.code;
            return codes;
        }
    }

    /// <summary>
    /// Záznam pro potřeby DataGridu.
    /// </summary>
    public struct StorageRecord
    {
        public int id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public string unit { get; set; }
        public decimal? unitQuantity { get; set; }
        public string expirationDate { get; set; }
        public string price { get; set; }
        public string priceForUnit { get; set; }
        public int code { get; set; }
    }

}
