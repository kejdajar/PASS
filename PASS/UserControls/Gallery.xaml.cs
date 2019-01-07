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
using PASS.Storage;

namespace PASS.UserControls
{
    /// <summary>
    /// Interaction logic for Gallery.xaml
    /// </summary>
    public partial class Gallery : UserControl
    {
        public Gallery()
        {
            InitializeComponent();

        }

        public List<ImageStruct> Images { get; set; } = new List<ImageStruct>();


        public void Update()
        {
            lbContainer.Items.Clear();
            foreach (ImageStruct img in Images)
            {
                StackPanel wrapper = new StackPanel();


                Image imgControl = new Image();
                imgControl.Source = img.image;
                imgControl.Width = 120;
                imgControl.Height = 100;
                imgControl.Margin = new Thickness(5, 5, 5, 5);


                TextBlock label = new TextBlock();
                label.TextWrapping = TextWrapping.Wrap;

                label.Text = img.imgName.Trim();
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.MaxWidth = 120;
                wrapper.Children.Add(label);
                wrapper.Children.Add(imgControl);

                lbContainer.Items.Add(wrapper);
            }
        }

        public int GetSelectedImageDatabaseID()
        {
            int selectedIndex = lbContainer.SelectedIndex;
            int counter = 0;
            foreach (ImageStruct istr in Images)
            {
                if (counter == selectedIndex)
                {
                    return istr.databaseId;
                }
                counter++;
            }
            return -1;
        }

        public delegate void ClickDelegate(object sender, RoutedEventArgs e);
        public ClickDelegate AddImageButton { get; set; }
        public ClickDelegate RemoveImageButton { get; set; }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
        {
            if (AddImageButton != null)
                AddImageButton(sender, e);
        }

        private void btnRemoveImage_Click(object sender, RoutedEventArgs e)
        {
            if (RemoveImageButton != null)
                RemoveImageButton(sender, e);
        }
    }
}
