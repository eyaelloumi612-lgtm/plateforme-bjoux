using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryManagementPlatform.Models
{
    public class Product
    {
        public Product()
        {
            LignesCommande = new List<LigneCommande>(); // Initialisation
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du produit est obligatoire")]
        [Display(Name = "Nom du produit")]
        public string Nom { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Le prix est obligatoire")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0")]
        [Display(Name = "Prix")]
        [Column(TypeName = "decimal(10,2)")] 
        public decimal Prix { get; set; }

        [Required(ErrorMessage = "Le stock est obligatoire")]
        [Range(0, int.MaxValue, ErrorMessage = "Le stock ne peut pas être négatif")]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Display(Name = "Catégorie")]
        public string Categorie { get; set; }

        [Display(Name = "Matière")]
        public string Matiere { get; set; }

        [Display(Name = "URL de l'image")]
        public string ImageUrl { get; set; }

        public List<LigneCommande> LignesCommande { get; set; }
    }
}