using System.Windows.Input;
using ViaEurope.WPF.Commands;
using ViaEurope.WPF.Services;

namespace ViaEurope.WPF.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _address = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private bool _isLoading;

        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
        public string Address { get => _address; set => SetProperty(ref _address, value); }
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
        public string SuccessMessage { get => _successMessage; set => SetProperty(ref _successMessage, value); }
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

        public Action? OnRegisterSuccess { get; set; }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(async _ => await RegisterAsync(),
                _ => !IsLoading);
        }

        public async Task RegisterAsync(string password = "", string confirmPassword = "")
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Completează toate câmpurile obligatorii.";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorMessage = "Parolele nu coincid.";
                return;
            }

            if (password.Length < 6)
            {
                ErrorMessage = "Parola trebuie să aibă minim 6 caractere.";
                return;
            }

            IsLoading = true;
            var (success, error) = await ServiceLocator.Instance.AuthService
                .RegisterAsync(FirstName, LastName, Email, Phone, Address, password);
            IsLoading = false;

            if (success)
            {
                SuccessMessage = "Cont creat cu succes! Te poți autentifica.";
                OnRegisterSuccess?.Invoke();
            }
            else
                ErrorMessage = error;
        }
    }
}