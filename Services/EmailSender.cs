using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace JewelryManagementPlatform.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implémentation factice - à remplacer par un vrai service d'email
            return Task.CompletedTask;
        }
    }
}