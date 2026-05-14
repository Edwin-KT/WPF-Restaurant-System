using System.Windows.Controls;
using ViaEurope.WPF.ViewModels;

namespace ViaEurope.WPF.Views
{
    public partial class OrdersPage : Page
    {
        private readonly OrdersViewModel _vm;

        public OrdersPage()
        {
            InitializeComponent();
            _vm = (OrdersViewModel)DataContext;
            Loaded += async (s, e) => await _vm.LoadOrdersAsync();
        }
    }
}