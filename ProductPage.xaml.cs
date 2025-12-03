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
        private User _user;
        private Order currentOrder = null;
        public ProductPage(User user)
        {
            InitializeComponent();
            _user = user;
            UpdateProducts();
            var currentProduct = Tuhvatshin41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProduct;
            if (user.UserRole == 0)
            {
                TextBlockFIO.Text = "Вы авторизованы как: Гость";
                TextBlockRole.Text = "Роль: Гость";
            }
            else
            {
                TextBlockFIO.Text = $"Вы авторизованны как: {user.UserSurname} {user.UserName} {user.UserPatronymic}";
                switch (user.UserRole)
                {
                    case 1:
                        TextBlockRole.Text = "Роль: Клиент";
                        break;
                    case 2:
                        TextBlockRole.Text = "Роль: Менеджер";
                        break;
                    case 3:
                        TextBlockRole.Text = "Роль: Администратор";
                        break;
                }
            }
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
            TextBlockCount.Text = $"кол-во {currentProduct.Count} из {allProducts.Count}";

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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        public void UpdateOrderButtonVisibility()
        {
            OrderBtn.Visibility = (currentOrder != null && currentOrder.OrderProduct.Any()) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddToOrderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductListView.SelectedItem as Product;
            if (selectedProduct == null) return;

            if (currentOrder == null)
            {
                int nextOrderId = Tuhvatshin41Entities.GetContext().Order.Any()
                    ? Tuhvatshin41Entities.GetContext().Order.Max(o => o.OrderID) + 1
                    : 1;

                currentOrder = new Order
                {
                    OrderID = nextOrderId,
                    OrderDate = DateTime.Now,
                    OrderDeliveryDate = DateTime.Now.AddDays(3),
                    OrderPickupPoint = 0,
                    OrderClientID = _user.UserRole == 1 ? (int?)_user.UserID : null,
                    OrderCode = nextOrderId,
                    OrderStatus = "Новый"
                };
            }

            var existingOrderProduct = currentOrder.OrderProduct.FirstOrDefault(op => op.ProductArticleNumber == selectedProduct.ProductArticleNumber);
            if (existingOrderProduct != null)
            {
                existingOrderProduct.ProductCount += 1;
            }
            else
            {
                currentOrder.OrderProduct.Add(new OrderProduct
                {
                    OrderID = currentOrder.OrderID,
                    ProductArticleNumber = selectedProduct.ProductArticleNumber,
                    ProductCount = 1,
                    Product = selectedProduct
                });
            }
            UpdateOrderButtonVisibility();
        }
        private void ResetCurrentOrder()
        {
            currentOrder = null;               // обнуляем заказ
            UpdateOrderButtonVisibility();     // скрываем кнопку
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e) 
        {
            var orderWindow = new OrderWindow(currentOrder, _user, ResetCurrentOrder);
            orderWindow.Show();
        }

    }
}
