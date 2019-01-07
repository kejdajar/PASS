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

namespace PASS.UserControls
{
    /// <summary>
    /// User control - číselník
    /// </summary>
    public partial class Numpad : UserControl
    {
        public Numpad()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Vymazání jednoho znaku z aktivního textboxu.
        /// </summary>       
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (_focusedTextBox.Text.Length > 0)
            {
                _focusedTextBox.Text = _focusedTextBox.Text.Remove(_focusedTextBox.Text.Length - 1);


                _focusedTextBox.Focus();

                _focusedTextBox.CaretIndex = _focusedTextBox.Text.Length;
            }

        }

        public delegate void ClickDelegate(object sender, RoutedEventArgs e);
        public ClickDelegate EventClrClick;
        public ClickDelegate EventEnterClick;

        /// <summary>
        /// Událost pro obsluhu číselníku.
        /// </summary>       
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btnSender = sender as Button;
            if (_focusedTextBox != null)
            {
                _focusedTextBox.Text += btnSender.Content;
                _focusedTextBox.Focus();

                _focusedTextBox.CaretIndex = _focusedTextBox.Text.Length;
            }
        }

        public TextBox _focusedTextBox { get; set; } = null;

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            EventEnterClick(sender, e);
        }
        private void btnClr_Click(object sender, RoutedEventArgs e)
        {
            EventClrClick(sender, e);
        }

    }
}
