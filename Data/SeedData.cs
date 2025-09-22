using Microsoft.AspNetCore.Identity;
using JewelryManagementPlatform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace JewelryManagementPlatform.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Vérifie si la base contient déjà des données
                if (context.Products.Any())
                {
                    return; // La base a déjà été peuplée
                }

                // Ajouter des clients de démonstration
                context.Clients.AddRange(
                    new Client
                    {
                        Nom = "Dupont",
                        Prenom = "Marie",
                        Email = "marie.dupont@email.com",
                        Telephone = "0123456789",
                        Adresse = "123 Rue de la Paix, Paris"
                    },
                    new Client
                    {
                        Nom = "Martin",
                        Prenom = "Pierre",
                        Email = "pierre.martin@email.com",
                        Telephone = "0987654321",
                        Adresse = "456 Avenue des Champs, Lyon"
                    }
                );

                await context.SaveChangesAsync();

                // Ajouter des produits de démonstration
                context.Products.AddRange(
                    new Product
                    {
                        Nom = "Bague en or 18 carats",
                        Description = "Magnifique bague en or jaune avec saphir",
                        Prix = 299.99m,
                        Stock = 10,
                        Categorie = "Bagues",
                        Matiere = "Or",
                        ImageUrl = "/images/bague-or.jpg"
                    },
                    new Product
                    {
                        Nom = "Collier en argent",
                        Description = "Collier élégant en argent sterling",
                        Prix = 149.99m,
                        Stock = 15,
                        Categorie = "Colliers",
                        Matiere = "Argent",
                        ImageUrl = "/images/collier-argent.jpg"
                    },
                    new Product
                    {
                        Nom = "Boucles d'oreilles diamant",
                        Description = "Boucles d'oreilles avec diamants authentiques",
                        Prix = 499.99m,
                        Stock = 5,
                        Categorie = "Boucles d'oreilles",
                        Matiere = "Or blanc",
                        ImageUrl = "/images/boucles-diamant.jpg"
                    },
                    new Product
                    {
                        Nom = "Bracelet en perles",
                        Description = "Bracelet élégant en perles naturelles",
                        Prix = 89.99m,
                        Stock = 20,
                        Categorie = "Bracelets",
                        Matiere = "Perles",
                        ImageUrl = "/images/bracelet-perles.jpg"
                    }
                );

                await context.SaveChangesAsync();

                // Créer un administrateur par défaut
                var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Créer les rôles s'ils n'existent pas
                string[] roleNames = { "Admin", "Manager", "User" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Créer l'utilisateur admin
                var adminUser = new IdentityUser
                {
                    UserName = "admin@bijoux.com",
                    Email = "admin@bijoux.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // Créer un utilisateur manager
                var managerUser = new IdentityUser
                {
                    UserName = "manager@bijoux.com",
                    Email = "manager@bijoux.com",
                    EmailConfirmed = true
                };

                var resultManager = await userManager.CreateAsync(managerUser, "Manager123!");

                if (resultManager.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                }
            }
        }
    }
}