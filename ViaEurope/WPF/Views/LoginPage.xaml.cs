using System.Windows;
using System.Windows.Controls;
using ViaEurope.WPF.ViewModels;

namespace ViaEurope.WPF.Views
{
    public partial class LoginPage : Page
    {
        private readonly LoginViewModel _vm;

        public LoginPage()
        {
            InitializeComponent();
            _vm = (LoginViewModel)DataContext;
            _vm.OnLoginSuccess = _ =>
            {
                var main = (MainWindow)Application.Current.MainWindow!;
                main.UpdateNavBar();
                main.NavigateToMenuPublic();
            };
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
            => await _vm.LoginAsync(PbPassword.Password);

        private void BtnGoRegister_Click(object sender, RoutedEventArgs e)
            => NavigationService?.Navigate(new RegisterPage());
    }
}