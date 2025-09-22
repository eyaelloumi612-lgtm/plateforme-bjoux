using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryManagementPlatform.Models
{
    public class Facture
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Commande")]
        public int CommandeId { get; set; }

        [ForeignKey("CommandeId")]
        public Commande Commande { get; set; }

        [Display(Name = "Date de facturation")]
        public DateTime DateFacturation { get; set; } = DateTime.Now;

        [Display(Name = "Montant TTC")]
        public decimal MontantTTC { get; set; }

        [NotMapped]
        public string Reference => $"FAC{Id:00000}";
    }
}