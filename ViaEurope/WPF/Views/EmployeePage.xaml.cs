using System.Windows.Controls;
using ViaEurope.WPF.ViewModels;

namespace ViaEurope.WPF.Views
{
    public partial class EmployeePage : Page
    {
        private readonly EmployeeViewModel _vm;

        public EmployeePage()
        {
            InitializeComponent();
            _vm = (EmployeeViewModel)DataContext;
            Loaded += async (s, e) => await _vm.LoadAllAsync();
        }
    }
}