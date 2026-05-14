using System.Windows;
using System.Windows.Controls.Primitives;
using ViaEurope.Business.Services;
using ViaEurope.WPF.Services;
using ViaEurope.WPF.Views;

namespace ViaEurope.WPF
{
    public partial class MainWindow : Window
    {
        private readonly AuthService _auth;
        private readonly NavigationService _nav;

        public MainWindow()
        {
            InitializeComponent();
            _auth = ServiceLocator.Instance.AuthService;
            _nav = ServiceLocator.Instance.NavigationService;
            _nav.Initialize(MainFrame);

            NavigateToMenu();
        }

        private void NavigateToMenu()
        {
            MainFrame.Navigate(new MenuPage());
        }

        public void NavigateToMenuPublic()
            => MainFrame.Navigate(new MenuPage());

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
            => NavigateToMenu();

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new LoginPage());

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new RegisterPage());

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new OrdersPage());

        private void BtnEmployee_Click(object sender, RoutedEventArgs e)
            => MainFrame.Navigate(new EmployeePage());

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            _auth.Logout();
            UpdateNavBar();
            NavigateToMenu();
        }


        public void UpdateNavBar()
        {
            var isAuth = _auth.IsAuthenticated;
            var isClient = _auth.IsClient;
            var isEmployee = _auth.IsEmployee;

            BtnLogin.Visibility = isAuth ? Visibility.Collapsed : Visibility.Visible;
            BtnRegister.Visibility = isAuth ? Visibility.Collapsed : Visibility.Visible;
            BtnLogout.Visibility = isAuth ? Visibility.Visible : Visibility.Collapsed;
            BtnOrders.Visibility = isClient ? Visibility.Visible : Visibility.Collapsed;
            BtnEmployee.Visibility = isEmployee ? Visibility.Visible : Visibility.Collapsed;
            TxtUser.Text = isAuth
                ? $"👤 {AuthService.CurrentUser!.FirstName}"
                : string.Empty;
        }
    }
}