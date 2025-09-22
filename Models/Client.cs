using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryManagementPlatform.Models
{
    public class Client
    {
        public Client()
        {
            Commandes = new List<Commande>(); // Initialisation dans le constructeur
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire")]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Format de téléphone invalide")]
        [Display(Name = "Téléphone")]
        public string Telephone { get; set; }

        [Display(Name = "Adresse")]
        public string Adresse { get; set; }

        public List<Commande> Commandes { get; set; } // Déjà initialisée

        [NotMapped]
        [Display(Name = "Nom complet")]
        public string NomComplet => $"{Prenom} {Nom}";
        
    }
}