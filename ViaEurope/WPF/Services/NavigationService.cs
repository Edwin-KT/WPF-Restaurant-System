using System.Windows.Controls;
using ViaEurope.WPF.ViewModels;

namespace ViaEurope.WPF.Services
{
    public class NavigationService
    {
        private Frame? _frame;

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public void NavigateTo(Page page)
        {
            _frame?.Navigate(page);
        }

        public void GoBack()
        {
            if (_frame?.CanGoBack == true)
                _frame.GoBack();
        }
    }
}