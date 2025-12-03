using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace _41razmer
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class PickupPoint
    {
        public string FullAddress => $"{PickupPoint1},{PickupPointCity}, {PickupPointStreet}";
    }

    public partial class OrderWindow : Window
    {
        private Order _order;
        private User _user;
        private Action _onOrderWindowClosed;

        public OrderWindow(Order order, User user, Action onOrderWindowClosed)
        {
            InitializeComponent();
            _order = order;
            _user = user;
            _onOrderWindowClosed = onOrderWindowClosed;
            SetDeliveryDate();
            // Заполнение информации о заказе
            OrderId();
            OrderFormDP.SelectedDate = _order.OrderDate;

            // ФИО клиента
            if (_user != null && _user.UserRole == 1)
            {
                ClientTB.Text = $"{_user.UserSurname} {_user.UserName} {_user.UserPatronymic}";
            }
            else
            {
                ClientTB.Text = "Гость";
            }

            // Список товаров
            ProductListView.ItemsSource = _order.OrderProduct.Select(op => new
            {
                ProductPhotoPath = op.Product?.ProductPhotoPath,
                ProductName = op.Product?.ProductName,
                ProductDescription = op.Product?.ProductDescription,
                ProductCost = op.Product?.ProductCost,
                ProductDiscountAmount = op.Product?.ProductDiscountAmount,
                ProductCount = op.ProductCount,
                OrderProduct = op
            }).ToList();

            // Пункты выдачи
            PickupCombo.ItemsSource = Tuhvatshin41Entities.GetContext().PickupPoint.ToList();
            PickupCombo.DisplayMemberPath = "FullAddress";
            PickupCombo.SelectedValuePath = "PickupPointID";


            UpdateOrderSum();
        }
        private void OrderId()
        {
            TBOrderID.Text = _order.OrderID.ToString();
        }
        private void SetDeliveryDate()
        {
            bool allInStock = _order.OrderProduct.All(op => op.Product.ProductQuantityInStock > 3);
            _order.OrderDeliveryDate = DateTime.Now.AddDays(allInStock ? 3 : 6);
            OrderDeliveryDP.SelectedDate = _order.OrderDeliveryDate;
        }
        private void UpdateOrderSum()
        {
            decimal total = 0;
            decimal discount = 0;
            foreach (var op in _order.OrderProduct)
            {
                decimal price = op.Product.ProductCost * op.ProductCount;
                total += price;
                if (op.Product.ProductDiscountAmount > 0)
                {
                    discount += price * op.Product.ProductDiscountAmount / 100;
                }
            }
            TBOrderSum.Text = $"Сумма заказа: {(total - discount):0.00} руб.   Скидка: {discount:0.00} руб.";
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var data = btn.DataContext;
            var op = GetOrderProductFromDataContext(data);
            if (op != null)
            {
                op.ProductCount--;
                if (op.ProductCount <= 0)
                {
                    _order.OrderProduct.Remove(op);
                }
                if(!_order.OrderProduct.Any())
                {
                    MessageBox.Show("В заказе нет товаров. Окно заказа будет закрыто.");
                    _onOrderWindowClosed?.Invoke();
                    this.Close();
                    return;
                }
                RefreshProductList();
                SetDeliveryDate();
            }
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var data = btn.DataContext;
            var op = GetOrderProductFromDataContext(data);
            if (op != null)
            {
                op.ProductCount++;
                RefreshProductList();
            }
        }

        private void RemoveProductBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var data = btn.DataContext;
            var op = GetOrderProductFromDataContext(data);
            if (op != null)
            {
                _order.OrderProduct.Remove(op);
                RefreshProductList();
            }
            if (!_order.OrderProduct.Any())
            {
                MessageBox.Show("В заказе нет товаров. Окно заказа будет закрыто.");
                _onOrderWindowClosed?.Invoke();
                this.Close();
                return;
            }
            RefreshProductList();
            SetDeliveryDate();
        }

        private OrderProduct GetOrderProductFromDataContext(object data)
        {
            // DataContext - анонимный объект, OrderProduct внутри
            var prop = data.GetType().GetProperty("OrderProduct");
            return prop?.GetValue(data) as OrderProduct;
        }

        public void RefreshProductList()
        {
            ProductListView.ItemsSource = null;
            ProductListView.ItemsSource = _order.OrderProduct.Select(op => new
            {
                ProductPhotoPath = op.Product?.ProductPhotoPath,
                ProductName = op.Product?.ProductName,
                ProductDescription = op.Product?.ProductDescription,
                ProductCost = op.Product?.ProductCost,
                ProductDiscountAmount = op.Product?.ProductDiscountAmount,
                ProductCount = op.ProductCount,
                OrderProduct = op
            }).ToList();
            UpdateOrderSum();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_order.OrderProduct.Count == 0)
            {
                MessageBox.Show("В заказе нет товаров.");
                return;
            }

            // Пункт выдачи
            if (PickupCombo.SelectedItem is PickupPoint selectedPoint)
            {
                _order.OrderPickupPoint = selectedPoint.PickupPointID;
            }
            else
            {
                MessageBox.Show("Выберите пункт выдачи.");
                return;
            }

            // Сохраняем заказ и позиции
            var db = Tuhvatshin41Entities.GetContext();
            if (!db.Order.Any(o => o.OrderID == _order.OrderID))
            {
                db.Order.Add(_order);
            }
            foreach (var op in _order.OrderProduct)
            {
                if (!db.OrderProduct.Any(x => x.OrderID == op.OrderID && x.ProductArticleNumber == op.ProductArticleNumber))
                {
                    db.OrderProduct.Add(op);
                }
            }
            db.SaveChanges();
            MessageBox.Show("Заказ успешно сохранён!");
            _onOrderWindowClosed?.Invoke();
            this.Close();
        }
    }
}
