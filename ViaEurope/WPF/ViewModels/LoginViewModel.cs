using System.Windows;
using System.Windows.Input;
using ViaEurope.WPF.Commands;
using ViaEurope.WPF.Services;

namespace ViaEurope.WPF.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }

        // Parola vine din code-behind (PasswordBox nu suporta binding)
        public Action<string>? OnLoginSuccess { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async _ => await LoginAsync(),
                _ => !IsLoading);
        }

        public async Task LoginAsync(string password = "")
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Completează email-ul și parola.";
                return;
            }

            IsLoading = true;
            var success = await ServiceLocator.Instance.AuthService.LoginAsync(Email, password);
            IsLoading = false;

            if (success)
                OnLoginSuccess?.Invoke(string.Empty);
            else
                ErrorMessage = "Email sau parolă incorecte.";
        }
    }
}