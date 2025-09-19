using Grocery.Core.Helpers;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IClientService _clientService;
        public AuthService(IClientService clientService)
        {
            _clientService = clientService;
        }
        public Client? Login(string email, string password)
        {
            var client = _clientService.Get(email);
            bool validClient =  PasswordHelper.VerifyPassword(password, client.Password);
            if(!validClient)
                return null;
            else
                return client;
            //Vraag de klantgegevens [Client] op die je zoekt met het opgegeven emailadres / gedaan
            //Als je een klant gevonden hebt controleer dan of het password matcht --> PasswordHelper.VerifyPassword(password, passwordFromClient)
            //Als alles klopt dan klantgegveens teruggeven, anders null / gedaan 
            return null;
        }
    }
}
