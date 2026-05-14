using System.Windows;
using System.Windows.Controls;
using ViaEurope.WPF.ViewModels;

namespace ViaEurope.WPF.Views
{
    public partial class RegisterPage : Page
    {
        private readonly RegisterViewModel _vm;

        public RegisterPage()
        {
            InitializeComponent();
            _vm = (RegisterViewModel)DataContext;
            _vm.OnRegisterSuccess = () =>
                Dispatcher.Invoke(() => NavigationService?.Navigate(new LoginPage()));
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
            => await _vm.RegisterAsync(PbPassword.Password, PbConfirm.Password);

        private void BtnGoLogin_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new LoginPage());
    }
}