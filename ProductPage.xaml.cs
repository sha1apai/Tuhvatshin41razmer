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
            UpdateProducts();
            var currentProduct = Tuhvatshin41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProduct;
        }
        private void UpdateProducts()
        {
            var allProducts = Tuhvatshin41Entities.GetContext().Product.ToList();
            var currentProduct = allProducts;

            if (ComboType.SelectedIndex == 0)
            {
                currentProduct = currentProduct.Where(p => p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount <= 100).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentProduct = currentProduct.Where(p => p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount < 9.99).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentProduct = currentProduct.Where(p => p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount < 14.99).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentProduct = currentProduct.Where(p => p.ProductDiscountAmount >= 15 && p.ProductDiscountAmount <= 100).ToList();
            }

            currentProduct = currentProduct.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            ProductListView.ItemsSource = currentProduct.ToList();
            if (RButtonDown.IsChecked.Value)
            {
                currentProduct = currentProduct.OrderByDescending(p => p.ProductCost).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
                currentProduct = currentProduct.OrderBy(p => p.ProductCost).ToList();
            }
            ProductListView.ItemsSource = currentProduct;
            TextBlockCount.Text = $"{currentProduct.Count} из {allProducts.Count}";
        }
        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }
    }
}
