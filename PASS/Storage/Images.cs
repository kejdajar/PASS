using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PASS;
using PASS.GeneralClasses;
using PASS.Storage;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace PASS.Storage
{
    public static class Images
    {

        public static BitmapImage ResourceToBitmapImage(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            byte[] imageBinary = null;
            using (BinaryReader br = new BinaryReader(assembly.GetManifestResourceStream(resourceName)))
            {
                imageBinary = br.ReadBytes((int)br.BaseStream.Length);
            }

            using (var stream = new MemoryStream(imageBinary.ToArray()))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }

        }

        public static void AddImage(string resourceName, string imageName)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            ImagesTable img = new ImagesTable();
            var assembly = Assembly.GetExecutingAssembly();
            byte[] imageBinary = null;


            using (BinaryReader br = new BinaryReader(assembly.GetManifestResourceStream(resourceName)))
            {
                imageBinary = br.ReadBytes((int)br.BaseStream.Length);
            }

            img.name = imageName;
            img.img = imageBinary;

            db.ImagesTables.InsertOnSubmit(img);
            db.SubmitChanges();

        }


        public static void AddImage(ImageStruct imageStruct)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            ImagesTable img = new ImagesTable();
            byte[] array = ImageToByte(imageStruct.image);
            img.name = imageStruct.imgName;
            img.img = array;
            db.ImagesTables.InsertOnSubmit(img);
            db.SubmitChanges();
        }


        public static byte[] ImageToByte(BitmapImage img)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
                return data;
            }
        }



        public static ImagesTable FindLastAddedImage()
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            ImagesTable last = (from l in db.ImagesTables
                                select l).OrderByDescending(l => l.id).First();
            return last;
        }

        public static ImagesTable GetImageById(int id)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            ImagesTable image = (from i in db.ImagesTables
                                 where i.id == id
                                 select i).Single();
            return image;
        }



        public static bool AssignImageToProduct(Product product, ImagesTable image)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            ProductImage assignTable = new ProductImage();
            try
            {
                assignTable.idImage = image.id;
                assignTable.idProduct = product.id;

                db.ProductImages.InsertOnSubmit(assignTable);
                db.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public static void RemoveRelationship(int imageId, int productId)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;

            // Vztah týkající se pouze aktuálního produktu a zvoleného obrázku
            ProductImage productImage = (from pi in db.ProductImages
                                         where pi.idProduct == productId && pi.idImage == imageId
                                         select pi).Single();
            db.ProductImages.DeleteOnSubmit(productImage);
            db.SubmitChanges();


        }

        public static void RemoveImage(int imageId)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            ImagesTable imgGoner = (from goner in db.ImagesTables
                                    where goner.id == imageId
                                    select goner).Single();
            db.ImagesTables.DeleteOnSubmit(imgGoner);
            db.SubmitChanges();
        }


        public static void AssignImageToProduct(int productId, int imageId)
        {
            LinqToSqlDataContext db = DatabaseSetup.Database;
            ProductImage assignTable = new ProductImage();

            assignTable.idImage = imageId;
            assignTable.idProduct = productId;

            db.ProductImages.InsertOnSubmit(assignTable);
            db.SubmitChanges();
        }
        public static List<ImageStruct> GetImages(Product product)
        {
            List<ImageStruct> list = new List<ImageStruct>();

            LinqToSqlDataContext db = DatabaseSetup.Database;
            IEnumerable<ProductImage> filtered = from f in db.ProductImages
                                                 where f.idProduct == product.id
                                                 select f;

            IEnumerable<ImagesTable> query = from pi in filtered
                                             join imagesT in db.ImagesTables
                                             on pi.idImage equals imagesT.id
                                             select imagesT;
            foreach (ImagesTable it in query)
            {
                using (var stream = new MemoryStream(it.img.ToArray()))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    list.Add(new ImageStruct() { imgName = it.name.Trim(), image = bitmap, databaseId = it.id });
                }
            }

            return list;
        }
    }
    public struct ImageStruct
    {
        public int databaseId { get; set; }
        public string imgName { get; set; }
        public BitmapImage image { get; set; }
    }
}
