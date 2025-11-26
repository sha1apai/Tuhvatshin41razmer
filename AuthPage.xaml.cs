using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _41razmer
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private string _captchaText = "";
        private bool _captchaRequired = false;

        public AuthPage()
        {
            InitializeComponent();
            SetCaptchaVisibility(false);
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = TBoxLogin.Text;
            string password = TBoxPassword.Text;

            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            if (_captchaRequired)
            {
                if (TBoxCaptcha.Text.Trim() != _captchaText)
                {
                    MessageBox.Show("Неверная капча");
                    LoginBtn.IsEnabled = false;
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    LoginBtn.IsEnabled = true;
                    GenerateCaptcha();
                    TBoxCaptcha.Text = "";
                    return;
                }
            }

            User user = Tuhvatshin41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                Manager.MainFrame.Navigate(new ProductPage(user));
                TBoxLogin.Text = "";
                TBoxPassword.Text = "";
                SetCaptchaVisibility(false);
                _captchaRequired = false;
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
                if (!_captchaRequired)
                {
                    _captchaRequired = true;
                    SetCaptchaVisibility(true);
                    GenerateCaptcha();
                }
                else
                {
                    LoginBtn.IsEnabled = false;
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    LoginBtn.IsEnabled = true;
                    GenerateCaptcha();
                    TBoxCaptcha.Text = "";
                }
            }
        }

        private void SetCaptchaVisibility(bool visible)
        {
            CaptchaOneWord.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            CaptchaTwoWord.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            CaptchaThreeWord.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            CaptchaFourWord.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            TBoxCaptcha.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GenerateCaptcha()
        {
            _captchaText = GenerateRandomText(4);
            CaptchaOneWord.Text = _captchaText[0].ToString();
            CaptchaTwoWord.Text = _captchaText[1].ToString();
            CaptchaThreeWord.Text = _captchaText[2].ToString();
            CaptchaFourWord.Text = _captchaText[3].ToString();
        }

        private string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var rand = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[rand.Next(s.Length)]).ToArray());
        }

        private void GuestLoginBtn_Click(object sender, RoutedEventArgs e)
        {
            var guestUser = new User
            {
                UserRole = 0 // Гость
            };
            Manager.MainFrame.Navigate(new ProductPage(guestUser));
            TBoxLogin.Text = "";
            TBoxPassword.Text = "";
            SetCaptchaVisibility(false);
            _captchaRequired = false;
        }
    }
}
