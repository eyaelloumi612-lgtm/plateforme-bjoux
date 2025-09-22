using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryManagementPlatform.Models
{
    public class Commande
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        [Display(Name = "Date de commande")]
        public DateTime DateCommande { get; set; } = DateTime.Now;

        [Display(Name = "Statut")]
        public string Statut { get; set; } = "En attente";

        [Display(Name = "Total")]
        public decimal Total { get; set; }

        public List<LigneCommande> LignesCommande { get; set; }

        [NotMapped]
        public string Reference => $"CMD{Id:00000}";
    }

    public class LigneCommande
    {
        public int Id { get; set; }

        [Required]
        public int CommandeId { get; set; }

        [ForeignKey("CommandeId")]
        public Commande Commande { get; set; }

        [Required]
        [Display(Name = "Produit")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être au moins 1")]
        [Display(Name = "Quantité")]
        public int Quantite { get; set; }

        [Display(Name = "Prix unitaire")]
        public decimal PrixUnitaire { get; set; }

        [NotMapped]
        public decimal SousTotal => Quantite * PrixUnitaire;
    }
}