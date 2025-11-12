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

namespace _41razmer
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public ProductPage()
        {
            InitializeComponent();
            var currentProduct = Tuhvatshin41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProduct;
        }
        private void UpdateProducts()
        {
            var currentProduct = Tuhvatshin41Entities.GetContext().Product.ToList();
            if (ComboType.SelectedIndex == 0)
            {
                currentProduct = currentProduct.Where(p=>(p.ProductDiscountAmount) >=0 && (p.ProductDiscountAmount)<=100).ToList();
            }
        }
        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
